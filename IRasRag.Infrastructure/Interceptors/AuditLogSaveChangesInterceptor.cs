using System.Text.Json;
using IRasRag.Application.Common.Interfaces.Auth;
using IRasRag.Domain.Common;
using IRasRag.Domain.Entities;
using IRasRag.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;

namespace IRasRag.Infrastructure.Interceptors
{
    public sealed class AuditLogSaveChangesInterceptor : SaveChangesInterceptor
    {
        private static readonly HashSet<string> IgnoredEntityNames = new(
            StringComparer.OrdinalIgnoreCase
        )
        {
            nameof(AuditLog),
            nameof(RefreshToken),
            nameof(Verification),
        };

        private static readonly string[] IgnoredProperties =
        {
            nameof(BaseEntity.Id),
            nameof(BaseEntity.CreatedAt),
            nameof(BaseEntity.ModifiedAt),
        };

        private readonly ICurrentUserAccessor _currentUserAccessor;
        private readonly ILogger<AuditLogSaveChangesInterceptor> _logger;

        public AuditLogSaveChangesInterceptor(
            ICurrentUserAccessor currentUserAccessor,
            ILogger<AuditLogSaveChangesInterceptor> logger
        )
        {
            _currentUserAccessor = currentUserAccessor;
            _logger = logger;
        }

        public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(
            DbContextEventData eventData,
            InterceptionResult<int> result,
            CancellationToken cancellationToken = default
        )
        {
            try
            {
                if (eventData.Context is not AppDbContext dbContext)
                    return await base.SavingChangesAsync(eventData, result, cancellationToken);

                dbContext.ChangeTracker.DetectChanges();
                var userId = _currentUserAccessor.GetUserId();
                if (userId is null)
                {
                    _logger.LogDebug(
                        "Skipping audit generation because no authenticated user was found."
                    );
                    return await base.SavingChangesAsync(eventData, result, cancellationToken);
                }

                var auditLogs = await BuildAuditLogsAsync(
                    dbContext,
                    userId.Value,
                    cancellationToken
                );
                if (auditLogs.Count > 0)
                    dbContext.AuditLogs.AddRange(auditLogs);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to prepare audit log entries before saving changes.");
            }

            return await base.SavingChangesAsync(eventData, result, cancellationToken);
        }

        private async Task<List<AuditLog>> BuildAuditLogsAsync(
            AppDbContext dbContext,
            Guid userId,
            CancellationToken cancellationToken
        )
        {
            var trackedEntries = dbContext
                .ChangeTracker.Entries<BaseEntity>()
                .Where(entry =>
                    entry.State is EntityState.Added or EntityState.Modified or EntityState.Deleted
                    && !IgnoredEntityNames.Contains(entry.Metadata.ClrType.Name)
                )
                .ToList();

            if (trackedEntries.Count == 0)
                return [];

            var user = await dbContext
                .Set<User>()
                .IgnoreQueryFilters()
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

            if (user == null)
            {
                _logger.LogWarning(
                    "Skipping audit generation because the current user {UserId} could not be resolved.",
                    userId
                );
                return [];
            }

            var auditLogs = new List<AuditLog>();
            foreach (var entry in trackedEntries)
            {
                var action = ResolveAction(entry);
                if (action is null)
                    continue;

                var (oldValue, newValue) = ResolveValues(entry, action);
                if (oldValue is null && newValue is null)
                    continue;

                auditLogs.Add(
                    new AuditLog
                    {
                        UserId = user.Id,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        Email = user.Email,
                        Action = action,
                        EntityType = entry.Metadata.ClrType.Name,
                        EntityId = ResolveEntityId(entry),
                        OldValue = oldValue,
                        NewValue = newValue,
                        Timestamp = DateTime.UtcNow,
                    }
                );
            }

            return auditLogs;
        }

        private static string? ResolveAction(EntityEntry<BaseEntity> entry)
        {
            if (entry.State == EntityState.Added)
                return "CREATE";

            if (entry.State == EntityState.Deleted)
                return "DELETE";

            if (entry.State == EntityState.Modified)
            {
                if (IsSoftDelete(entry))
                    return "DELETE";

                return HasMeaningfulChanges(entry) ? "UPDATE" : null;
            }

            return null;
        }

        private static (string? OldValue, string? NewValue) ResolveValues(
            EntityEntry<BaseEntity> entry,
            string action
        )
        {
            return action switch
            {
                "CREATE" => (null, SerializeValues(entry.CurrentValues)),
                "DELETE" => (SerializeValues(entry.OriginalValues), null),
                _ => (
                    SerializeValues(entry.OriginalValues, entry.CurrentValues, true),
                    SerializeValues(entry.CurrentValues, entry.OriginalValues, false)
                ),
            };
        }

        private static string ResolveEntityId(EntityEntry<BaseEntity> entry)
        {
            var id = entry.Property(nameof(BaseEntity.Id)).CurrentValue;
            return id?.ToString() ?? string.Empty;
        }

        private static bool IsSoftDelete(EntityEntry<BaseEntity> entry)
        {
            var isDeletedProperty = entry.Properties.FirstOrDefault(p =>
                string.Equals(
                    p.Metadata.Name,
                    nameof(ISoftDeletable.IsDeleted),
                    StringComparison.OrdinalIgnoreCase
                )
            );

            return entry.Entity is ISoftDeletable
                && isDeletedProperty?.CurrentValue is bool isDeleted
                && isDeleted;
        }

        private static bool HasMeaningfulChanges(EntityEntry<BaseEntity> entry)
        {
            return entry.Properties.Any(property =>
                !IgnoredProperties.Contains(property.Metadata.Name) && property.IsModified
            );
        }

        private static string? SerializeValues(
            PropertyValues values,
            PropertyValues? comparisonValues = null,
            bool oldValues = false
        )
        {
            var payload = new Dictionary<string, object?>();

            foreach (var property in values.Properties)
            {
                if (IgnoredProperties.Contains(property.Name))
                    continue;

                if (comparisonValues is not null)
                {
                    var currentValue = values[property];
                    var comparisonValue = comparisonValues[property];
                    if (Equals(currentValue, comparisonValue))
                        continue;

                    payload[property.Name] = oldValues ? comparisonValue : currentValue;
                    continue;
                }

                payload[property.Name] = values[property];
            }

            return payload.Count == 0 ? null : JsonSerializer.Serialize(payload);
        }
    }
}

using System.Globalization;
using System.Text.Encodings.Web;
using System.Text.Json;
using IRasRag.Domain.Entities;

namespace IRasRag.Application.Common.Utils
{
    public static class AuditLogHelper
    {
        public static AuditLog Create(
            User user,
            string action,
            string entityType,
            string entityId,
            object? oldValue = null,
            object? newValue = null
        )
        {
            return new AuditLog
            {
                UserId = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Action = action,
                EntityType = entityType,
                EntityId = entityId,
                OldValue = SerializePayload(oldValue),
                NewValue = SerializePayload(newValue),
                Timestamp = DateTime.UtcNow,
            };
        }

        public static string? SerializePayload(object? value)
        {
            if (value == null)
                return null;

            return value switch
            {
                string text => text,
                DateTime dateTime => dateTime
                    .ToUniversalTime()
                    .ToString("O", CultureInfo.InvariantCulture),
                DateTimeOffset dateTimeOffset => dateTimeOffset
                    .ToUniversalTime()
                    .ToString("O", CultureInfo.InvariantCulture),
                _ => JsonSerializer.Serialize(
                    value,
                    new JsonSerializerOptions
                    {
                        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                    }
                ),
            };
        }
    }
}

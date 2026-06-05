using System.Text.RegularExpressions;
using IRasRag.Application.Common.Exceptions;
using IRasRag.Application.Common.Interfaces.Persistence;
using IRasRag.Application.Common.Interfaces.Persistence.Repositories;
using IRasRag.Domain.Common;
using IRasRag.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Npgsql;

namespace IRasRag.Infrastructure.Persistence
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;
        private IDbContextTransaction _transaction;
        private readonly Dictionary<Type, object> _repositories = [];

        public UnitOfWork(AppDbContext context)
        {
            _context = context;
            SensorLogs = new SensorLogRepository(_context);
        }

        public ISensorLogRepository SensorLogs { get; private set; }

        public IRepository<T> GetRepository<T>()
            where T : BaseEntity
        {
            if (!_repositories.ContainsKey(typeof(T)))
            {
                var repositoryInstance = new Repository<T>(_context);
                _repositories.Add(typeof(T), repositoryInstance);
            }
            return (IRepository<T>)_repositories[typeof(T)];
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                return await _context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateException ex)
            {
                throw ResolveDbConflict(ex);
            }
        }

        public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
        {
            if (_transaction != null)
                return;

            _transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
        }

        public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                await _context.SaveChangesAsync(cancellationToken);

                if (_transaction != null)
                {
                    await _transaction.CommitAsync(cancellationToken);
                    await _transaction.DisposeAsync();
                    _transaction = null;
                }
            }
            catch (DbUpdateException ex)
            {
                await RollbackTransactionAsync(cancellationToken);
                throw ResolveDbConflict(ex);
            }
            catch
            {
                await RollbackTransactionAsync(cancellationToken);
                throw;
            }
        }

        // Constraint name → human-readable description for unique violations.
        // Keys are snake_case because UseSnakeCaseNamingConvention() is active.
        private static readonly Dictionary<string, string> UniqueConstraintMessages = new()
        {
            ["ix_users_email"] = "Email này đã được sử dụng.",
            ["ix_master_boards_mac_address"] = "Địa chỉ MAC đã được đăng ký cho thiết bị khác.",
            ["ix_sensors_master_board_id_pin_code"] = "Mã pin này đã được sử dụng trên board này.",
            ["ix_control_devices_master_board_id_pin_code"] =
                "Mã pin này đã được sử dụng trên board này.",
            ["ix_growth_stages_name"] = "Tên giai đoạn tăng trưởng đã tồn tại.",
            ["ix_job_types_name"] = "Tên loại công việc đã tồn tại.",
            ["ix_job_control_mappings_job_id_control_device_id"] =
                "Thiết bị này đã được gán cho công việc này.",
            ["ix_sensor_logs_sensor_id_period_start"] =
                "Đã tồn tại log cảm biến cho khoảng thời gian này.",
            ["ix_species_stage_configs_species_id_growth_stage_id"] =
                "Đã tồn tại cấu hình cho loài và giai đoạn tăng trưởng này.",
            ["ix_species_thresholds_species_id_growth_stage_id_sensor_type_id"] =
                "Đã tồn tại ngưỡng cho loài, giai đoạn và loại cảm biến này.",
            ["ix_user_farms_user_id_farm_id"] = "Người dùng đã được gán vào trại này.",
            ["ix_alerts_fish_tank_id_farming_batch_id_sensor_type_id"] =
                "Đã tồn tại cảnh báo đang mở cho bể, lứa nuôi và loại cảm biến này.",
            ["ix_alerts_fish_tank_id_sensor_type_id"] =
                "Đã tồn tại cảnh báo đang mở cho bể và loại cảm biến này.",
        };

        private static Exception ResolveDbConflict(DbUpdateException ex)
        {
            if (ex.InnerException is not PostgresException pgEx)
                return new DbConflictException(
                    FallbackFromMessage(ex.InnerException?.Message ?? ex.Message)
                );

            return pgEx.SqlState switch
            {
                PostgresErrorCodes.UniqueViolation => new DbConflictException(
                    ResolveUniqueViolation(pgEx)
                ),
                PostgresErrorCodes.ForeignKeyViolation => ResolveForeignKeyViolation(pgEx),
                _ => new DbConflictException(
                    "Lỗi ràng buộc cơ sở dữ liệu. Vui lòng kiểm tra lại dữ liệu."
                ),
            };
        }

        private static string ResolveUniqueViolation(PostgresException pgEx)
        {
            if (
                pgEx.ConstraintName is not null
                && UniqueConstraintMessages.TryGetValue(pgEx.ConstraintName, out var msg)
            )
                return msg;

            return "Dữ liệu đã tồn tại. Vui lòng kiểm tra lại thông tin.";
        }

        private static Exception ResolveForeignKeyViolation(PostgresException pgEx)
        {
            var detail = pgEx.Detail ?? string.Empty;

            if (detail.Contains("is not present in table", StringComparison.OrdinalIgnoreCase))
            {
                // Insert/update with a FK pointing to a non-existent row → client error (400)
                var referenced = ExtractTableFromDetail(
                    detail,
                    @"is not present in table ""([^""]+)"""
                );
                var message = referenced is not null
                    ? $"Không tìm thấy {referenced} được tham chiếu."
                    : "Thực thể được tham chiếu không tồn tại.";
                return new DbReferenceException(message);
            }

            // Trying to delete/update a row that other rows still reference → conflict (409)
            var referencingTable = ExtractTableFromDetail(
                detail,
                @"still referenced from table ""([^""]+)"""
            );
            var deleteMessage = referencingTable is not null
                ? $"Không thể xóa do dữ liệu đang được tham chiếu bởi {referencingTable}."
                : "Không thể xóa do dữ liệu đang được sử dụng bởi bản ghi khác.";
            return new DbConflictException(deleteMessage);
        }

        // Postgres Detail already contains the table name in quotes — parse it directly.
        // Works regardless of naming convention.
        private static string? ExtractTableFromDetail(string detail, string pattern)
        {
            var match = Regex.Match(detail, pattern);
            return match.Success ? match.Groups[1].Value : null;
        }

        private static string FallbackFromMessage(string msg)
        {
            if (
                msg.Contains("duplicate", StringComparison.OrdinalIgnoreCase)
                || msg.Contains("unique", StringComparison.OrdinalIgnoreCase)
            )
                return "Dữ liệu đã tồn tại. Vui lòng kiểm tra lại thông tin.";

            if (
                msg.Contains("foreign key", StringComparison.OrdinalIgnoreCase)
                || msg.Contains("violates", StringComparison.OrdinalIgnoreCase)
            )
                return "Không thể thực hiện do dữ liệu liên quan đang được sử dụng hoặc không tồn tại.";

            return "Lỗi ràng buộc cơ sở dữ liệu. Vui lòng kiểm tra lại dữ liệu.";
        }

        public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync(cancellationToken);
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public void Dispose()
        {
            _transaction?.Dispose();
            _context.Dispose();
        }

        public async ValueTask DisposeAsync()
        {
            if (_transaction != null)
                await _transaction.DisposeAsync();

            await _context.DisposeAsync();
        }
    }
}

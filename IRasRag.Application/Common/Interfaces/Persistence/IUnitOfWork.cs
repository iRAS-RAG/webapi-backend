using IRasRag.Application.Common.Interfaces.Persistence.Repositories;
using IRasRag.Domain.Common;

namespace IRasRag.Application.Common.Interfaces.Persistence
{
    public interface IUnitOfWork : IDisposable, IAsyncDisposable
    {
        IRepository<T> GetRepository<T>()
            where T : BaseEntity;
        ISensorLogRepository SensorLogs { get; }
        IMoralityLogRepository MoralityLogs { get; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
        Task BeginTransactionAsync(CancellationToken cancellationToken = default);
        Task CommitTransactionAsync(CancellationToken cancellationToken = default);
        Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
    }
}

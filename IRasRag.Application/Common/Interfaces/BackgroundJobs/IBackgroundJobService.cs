using System.Linq.Expressions;

namespace IRasRag.Application.Common.Interfaces.BackgroundJobs
{
    public interface IBackgroundJobService
    {
        string Enqueue<T>(Expression<Func<T, Task>> methodCall);
        string Enqueue<T>(Expression<Action<T>> methodCall);
        string Schedule<T>(Expression<Func<T, Task>> methodCall, TimeSpan delay);
        string Schedule<T>(Expression<Action<T>> methodCall, TimeSpan delay);
    }
}

using System.Linq.Expressions;
using Hangfire;
using IRasRag.Application.Common.Interfaces.BackgroundJobs;

namespace IRasRag.Infrastructure.Services.BackgroundJobs
{
    public class HangfireBackgroundJobService : IBackgroundJobService
    {
        public string Enqueue<T>(Expression<Func<T, Task>> methodCall) =>
            BackgroundJob.Enqueue(methodCall);

        public string Enqueue<T>(Expression<Action<T>> methodCall) =>
            BackgroundJob.Enqueue(methodCall);

        public string Schedule<T>(Expression<Func<T, Task>> methodCall, TimeSpan delay) =>
            BackgroundJob.Schedule(methodCall, delay);

        public string Schedule<T>(Expression<Action<T>> methodCall, TimeSpan delay) =>
            BackgroundJob.Schedule(methodCall, delay);
    }
}

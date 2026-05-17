using Hangfire;
using Hangfire.Common;
using IRasRag.Application.Common.Interfaces.Advisory;
using IRasRag.Application.Common.Interfaces.BackgroundJobs;
using IRasRag.Application.Common.Interfaces.CloudFileStorage;

namespace IRasRag.Infrastructure.HangFireJobFilters
{
    public class MethodRetryFilterProvider : IJobFilterProvider
    {
        private static readonly int[] DocumentIngestDelays = [10, 30, 60, 120];
        private static readonly int[] ThresholdSyncCreateDelays = [5, 10, 20, 40, 80];
        private static readonly int[] CloudDeleteDelays = [30, 60, 120, 300, 600];

        public IEnumerable<JobFilter> GetFilters(Job job)
        {
            if (job.Method.DeclaringType == typeof(IDocumentIngestJob))
            {
                if (job.Method.Name == nameof(IDocumentIngestJob.RunAsync))
                {
                    yield return BuildRetryFilter(
                        attempts: 4,
                        delays: DocumentIngestDelays,
                        exceeded: AttemptsExceededAction.Fail);
                }
            }

            if (job.Method.DeclaringType == typeof(IThresholdSyncJob))
            {
                if (job.Method.Name == nameof(IThresholdSyncJob.SyncCreateAsync))
                {
                    yield return BuildRetryFilter(
                        attempts: 5,
                        delays: ThresholdSyncCreateDelays,
                        exceeded: AttemptsExceededAction.Fail);
                }
            }

            if (job.Method.DeclaringType == typeof(ICloudFileStorageService))
            {
                if (job.Method.Name == nameof(ICloudFileStorageService.DeleteAsync))
                {
                    yield return BuildRetryFilter(
                        attempts: 5,
                        delays: CloudDeleteDelays,
                        exceeded: AttemptsExceededAction.Fail);
                }
            }
        }

        private static JobFilter BuildRetryFilter(
            int attempts,
            int[]? delays,
            AttemptsExceededAction exceeded)
        {
            return new JobFilter(
                new AutomaticRetryAttribute
                {
                    Attempts = attempts,
                    DelaysInSeconds = delays,
                    OnAttemptsExceeded = exceeded
                },
                JobFilterScope.Method,
                null);
        }
    }
}

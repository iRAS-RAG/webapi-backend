using Hangfire;
using Hangfire.States;
using Hangfire.Storage;
using IRasRag.Application.Common.Interfaces.BackgroundJobs;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace IRasRag.Infrastructure.Filters
{
    public class DocumentIngestFailedFilter : IApplyStateFilter
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<DocumentIngestFailedFilter> _logger;

        public DocumentIngestFailedFilter(IServiceScopeFactory scopeFactory, ILoggerFactory loggerFactory)
        {
            _scopeFactory = scopeFactory;
            _logger = loggerFactory.CreateLogger<DocumentIngestFailedFilter>();
        }

        public void OnStateApplied(ApplyStateContext context, IWriteOnlyTransaction transaction)
        {
            if (context.NewState is not FailedState) return;

            var job = context.BackgroundJob?.Job;
            if (job == null)
            {
                _logger.LogWarning("DocumentIngestFailedFilter: BackgroundJob or Job was null");
                return;
            }

            if (job.Type != typeof(IDocumentIngestJob))
            {
                _logger.LogDebug("DocumentIngestFailedFilter: skipping job type {Type}", job.Type.Name);
                return;
            }

            if (job.Method.Name != nameof(IDocumentIngestJob.RunAsync) &&
                job.Method.Name != nameof(IDocumentIngestJob.RunAlwaysFailAsync))
            {
                _logger.LogDebug("DocumentIngestFailedFilter: skipping method {Method}", job.Method.Name);
                return;
            }

            if (job.Args.Count != 1 || !Guid.TryParse(job.Args[0]?.ToString(), out var documentId))
            {
                _logger.LogWarning(
                    "DocumentIngestFailedFilter: unexpected args (count={Count}, value={Value})",
                    job.Args.Count,
                    job.Args.Count > 0 ? job.Args[0] : "none");
                return;
            }

            _logger.LogInformation(
                "DocumentIngestFailedFilter: enqueueing MarkAsFailedAsync for document {DocumentId}",
                documentId);

            using var scope = _scopeFactory.CreateScope();
            var client = scope.ServiceProvider.GetRequiredService<IBackgroundJobClient>();

            client.Enqueue<IDocumentIngestJob>(j => j.MarkAsFailedAsync(documentId));
        }

        public void OnStateUnapplied(ApplyStateContext context, IWriteOnlyTransaction transaction) { }
    }
}

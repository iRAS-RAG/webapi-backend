using IRasRag.Application.Common.Interfaces.Advisory;
using IRasRag.Application.Common.Interfaces.BackgroundJobs;
using Microsoft.Extensions.Logging;

namespace IRasRag.Infrastructure.Services.Advisory.Jobs
{
    public class DocumentRagDeleteJob : IDocumentRagDeleteJob
    {
        private readonly IRagChatClient _ragChatClient;
        private readonly ILogger<DocumentRagDeleteJob> _logger;

        public DocumentRagDeleteJob(
            IRagChatClient ragChatClient,
            ILogger<DocumentRagDeleteJob> logger
        )
        {
            _ragChatClient = ragChatClient;
            _logger = logger;
        }

        public async Task RunAsync(string fileUrl)
        {
            var result = await _ragChatClient.DeleteDocumentByUrlAsync(fileUrl);

            if (result == null)
            {
                _logger.LogWarning("RAG delete: no result for url={Url}, will retry", fileUrl);
                throw new InvalidOperationException(
                    $"RAG delete returned no result for url={fileUrl}."
                );
            }

            _logger.LogInformation(
                "RAG delete: removed {Deleted} document(s) for url={Url}",
                result.Deleted,
                fileUrl
            );
        }
    }
}

using IRasRag.Application.Common.Interfaces.Advisory;
using IRasRag.Application.Common.Interfaces.BackgroundJobs;
using IRasRag.Application.Common.Interfaces.Persistence;
using IRasRag.Domain.Entities;
using IRasRag.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace IRasRag.Infrastructure.Services.Advisory
{
    public class DocumentIngestJob : IDocumentIngestJob
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRagChatClient _ragChatClient;
        private readonly ILogger<DocumentIngestJob> _logger;

        public DocumentIngestJob(
            IUnitOfWork unitOfWork,
            IRagChatClient ragChatClient,
            ILogger<DocumentIngestJob> logger)
        {
            _unitOfWork = unitOfWork;
            _ragChatClient = ragChatClient;
            _logger = logger;
        }

        public async Task RunAsync(Guid documentId)
        {
            var document = await _unitOfWork.GetRepository<Document>().GetByIdAsync(documentId);

            if (document == null)
            {
                _logger.LogWarning("RAG ingest: document {DocumentId} not found, skipping", documentId);
                return;
            }

            if (document.RagStatus != DocumentRagStatus.Processing)
            {
                document.RagStatus = DocumentRagStatus.Processing;
                await _unitOfWork.SaveChangesAsync();
            }

            var response = await _ragChatClient.IngestDocumentByUrlAsync(document.FileUrl, document.Title);

            if (response == null || (response.Status != "ok" && response.Status != "exists"))
            {
                _logger.LogWarning(
                    "RAG ingest: failed for document {DocumentId} (status={Status}), will retry",
                    documentId,
                    response?.Status);

                throw new InvalidOperationException(
                    $"RAG ingest returned non-ok status for document {documentId}.");
            }

            document.RagStatus = DocumentRagStatus.Indexed;
            await _unitOfWork.SaveChangesAsync();

            if (response.Status == "exists")
                _logger.LogInformation(
                    "RAG ingest: document {DocumentId} already indexed (exists), marked as Indexed",
                    documentId);
            else
                _logger.LogInformation(
                    "RAG ingest: document {DocumentId} indexed ({Chunks} chunks)",
                    documentId,
                    response.Chunks);
        }

        public async Task MarkAsFailedAsync(Guid documentId)
        {
            var document = await _unitOfWork.GetRepository<Document>().GetByIdAsync(documentId);

            if (document == null)
            {
                _logger.LogWarning(
                    "RAG ingest filter: document {DocumentId} not found when marking Failed",
                    documentId);
                return;
            }

            document.RagStatus = DocumentRagStatus.Failed;
            _unitOfWork.GetRepository<Document>().Update(document);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogWarning(
                "RAG ingest: all retries exhausted for document {DocumentId}, marked as Failed",
                documentId);
        }
    }
}

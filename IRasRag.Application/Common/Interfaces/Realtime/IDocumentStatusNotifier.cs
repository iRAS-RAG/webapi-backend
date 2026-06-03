using IRasRag.Domain.Enums;

namespace IRasRag.Application.Common.Interfaces.Realtime
{
    public interface IDocumentStatusNotifier
    {
        Task NotifyRagStatusUpdatedAsync(Guid documentId, DocumentRagStatus status);
    }
}

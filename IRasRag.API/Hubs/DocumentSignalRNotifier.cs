using IRasRag.Application.Common.Interfaces.Realtime;
using IRasRag.Domain.Enums;
using Microsoft.AspNetCore.SignalR;

namespace IRasRag.API.Hubs
{
    public class DocumentSignalRNotifier : IDocumentStatusNotifier
    {
        private readonly IHubContext<DocumentHub> _hub;

        public DocumentSignalRNotifier(IHubContext<DocumentHub> hub)
        {
            _hub = hub;
        }

        public Task NotifyRagStatusUpdatedAsync(Guid documentId, DocumentRagStatus status)
        {
            var group = DocumentHub.DocumentGroup(documentId.ToString());
            return _hub.Clients.Group(group).SendAsync("RagStatusUpdated", new
            {
                documentId,
                ragStatus = (int)status,
            });
        }
    }
}

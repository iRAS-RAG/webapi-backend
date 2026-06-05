using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace IRasRag.API.Hubs
{
    [Authorize]
    public class DocumentHub : Hub
    {
        public Task JoinDocument(string documentId)
        {
            return Groups.AddToGroupAsync(Context.ConnectionId, DocumentGroup(documentId));
        }

        public Task LeaveDocument(string documentId)
        {
            return Groups.RemoveFromGroupAsync(Context.ConnectionId, DocumentGroup(documentId));
        }

        public static string DocumentGroup(string documentId) => $"document-{documentId}";
    }
}

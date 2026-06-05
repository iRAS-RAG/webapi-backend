using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace IRasRag.API.Hubs
{
    [Authorize(Roles = "Supervisor")]
    public class SupervisorMetricsHub : Hub
    {
        public override Task OnConnectedAsync()
        {
            return base.OnConnectedAsync();
        }

        public Task JoinFarmGroup(string farmId)
        {
            return Groups.AddToGroupAsync(Context.ConnectionId, FarmGroup(farmId));
        }

        public Task LeaveFarmGroup(string farmId)
        {
            return Groups.RemoveFromGroupAsync(Context.ConnectionId, FarmGroup(farmId));
        }

        public static string FarmGroup(string farmId) => $"farm-{farmId}";
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace IRasRag.API.Hubs
{
    [Authorize]
    public class TelemetryHub : Hub
    {
        public async Task JoinTank(string tankId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, TankGroup(tankId));
        }

        public async Task LeaveTank(string tankId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, TankGroup(tankId));
        }

        public static string TankGroup(string tankId) => $"tank-{tankId}";
    }
}
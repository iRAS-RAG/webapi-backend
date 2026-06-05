using System.Security.Claims;
using IRasRag.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace IRasRag.API.Hubs
{
    [Authorize]
    public class TelemetryHub : Hub
    {
        private readonly IUserTankAccessService _tankAccess;

        public TelemetryHub(IUserTankAccessService tankAccess)
        {
            _tankAccess = tankAccess;
        }

        public override async Task OnConnectedAsync()
        {
            var userId = GetUserId();
            if (userId == null)
            {
                Context.Abort();
                return;
            }

            var allowedTanks = await _tankAccess.GetAllowedTankIdsAsync(userId.Value);
            foreach (var tankId in allowedTanks)
                await Groups.AddToGroupAsync(Context.ConnectionId, TankGroup(tankId.ToString()));

            await base.OnConnectedAsync();
        }

        // Opt-in to a single tank's live telemetry stream (e.g. when user navigates to a tank page).
        public async Task JoinTank(string tankId)
        {
            if (!Guid.TryParse(tankId, out var tankGuid))
            {
                await Clients.Caller.SendAsync("Error", "Invalid tank ID.");
                return;
            }

            var userId = GetUserId();
            if (userId == null)
            {
                Context.Abort();
                return;
            }

            if (!await _tankAccess.CanAccessTankAsync(userId.Value, tankGuid))
            {
                await Clients.Caller.SendAsync("Error", "Access denied to this tank.");
                return;
            }

            await Groups.AddToGroupAsync(Context.ConnectionId, TankGroup(tankId));
        }

        public async Task LeaveTank(string tankId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, TankGroup(tankId));
        }

        public static string TankGroup(string tankId) => $"tank-{tankId}";

        private Guid? GetUserId()
        {
            var claim = Context.User?.FindFirst(ClaimTypes.NameIdentifier);
            return Guid.TryParse(claim?.Value, out var id) ? id : null;
        }
    }
}

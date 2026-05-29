using IRasRag.Application.Common.Interfaces.Realtime;
using Microsoft.AspNetCore.SignalR;

namespace IRasRag.API.Hubs
{
    public class SupervisorSignalRNotifier : ISupervisorNotifier
    {
        private readonly IHubContext<TelemetryHub> _hub;

        public SupervisorSignalRNotifier(IHubContext<TelemetryHub> hub)
        {
            _hub = hub;
        }

        public Task NotifyFeedingLogAsync(Guid farmId, object payload)
        {
            var group = SupervisorMetricsHub.FarmGroup(farmId.ToString());
            return _hub.Clients.Group(group).SendAsync("FeedingLogCreated", payload);
        }

        public Task NotifyMortalityLogAsync(Guid farmId, object payload)
        {
            var group = SupervisorMetricsHub.FarmGroup(farmId.ToString());
            return _hub.Clients.Group(group).SendAsync("MortalityLogCreated", payload);
        }
    }
}

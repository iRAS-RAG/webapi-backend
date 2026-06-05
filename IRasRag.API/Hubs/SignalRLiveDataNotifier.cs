using System.Threading.Channels;
using IRasRag.Application.Common.Interfaces.Realtime;
using IRasRag.Application.Common.Models.Realtime;
using Microsoft.AspNetCore.SignalR;

namespace IRasRag.API.Hubs
{
    public class SignalRLiveDataNotifier : ILiveDataNotifier
    {
        private readonly IHubContext<TelemetryHub> _telemetryHub;
        private readonly IHubContext<AlertHub> _alertHub;
        private readonly Channel<TelemetryPush> _channel;

        public SignalRLiveDataNotifier(
            IHubContext<TelemetryHub> telemetryHub,
            IHubContext<AlertHub> alertHub
        )
        {
            _telemetryHub = telemetryHub;
            _alertHub = alertHub;
            _channel = Channel.CreateBounded<TelemetryPush>(
                new BoundedChannelOptions(2000)
                {
                    FullMode = BoundedChannelFullMode.DropOldest,
                    SingleReader = true,
                    SingleWriter = false,
                }
            );
        }

        public ChannelReader<TelemetryPush> Reader => _channel.Reader;

        public void EnqueueTelemetry(TelemetryPush push) => _channel.Writer.TryWrite(push);

        public Task PushTelemetryAsync(TelemetryPush push)
        {
            var group = TelemetryHub.TankGroup(push.TankId.ToString());
            return _telemetryHub.Clients.Group(group).SendAsync("ReceiveTelemetry", push);
        }

        public async Task PushAlertAsync(AlertPush push)
        {
            var group = AlertHub.TankGroup(push.TankId.ToString());
            var clients = _alertHub.Clients.Group(group);

            await clients.SendAsync("ReceiveAlert", push);
            await clients.SendAsync(
                "AlertCreated",
                new AlertCreatedNotification(push.AlertId, push.TankId)
            );
        }
    }
}

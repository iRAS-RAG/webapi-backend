using IRasRag.Application.Common.Interfaces.Realtime;
using IRasRag.Application.Common.Models.Realtime;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Channels;

namespace IRasRag.API.Hubs
{
    public class SignalRLiveDataNotifier : ILiveDataNotifier
    {
        private readonly IHubContext<TelemetryHub> _hub;
        private readonly Channel<TelemetryPush> _channel;

        public SignalRLiveDataNotifier(IHubContext<TelemetryHub> hub)
        {
            _hub = hub;
            _channel = Channel.CreateBounded<TelemetryPush>(new BoundedChannelOptions(2000)
            {
                FullMode = BoundedChannelFullMode.DropOldest,
                SingleReader = true,
                SingleWriter = false
            });
        }

        public ChannelReader<TelemetryPush> Reader => _channel.Reader;

        public void EnqueueTelemetry(TelemetryPush push)
            => _channel.Writer.TryWrite(push);

        public Task PushTelemetryAsync(TelemetryPush push)
        {
            var group = TelemetryHub.TankGroup(push.TankId.ToString());
            return _hub.Clients.Group(group).SendAsync("ReceiveTelemetry", push);
        }
    }
}

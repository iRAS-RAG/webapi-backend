using System.Threading.Channels;
using IRasRag.Application.Common.Interfaces.Advisory;
using IRasRag.Application.Common.Models.Advisory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace IRasRag.Infrastructure.Services.Advisory
{
    public class IoTIngestBatchWriter : BackgroundService, IIoTIngestBatchWriter
    {
        private static readonly TimeSpan FlushInterval = TimeSpan.FromSeconds(1);
        private static readonly TimeSpan MaxBackoff = TimeSpan.FromMinutes(2);

        private readonly Channel<IoTIngestPayload> _channel;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<IoTIngestBatchWriter> _logger;

        private TimeSpan _backoff = TimeSpan.Zero;
        private DateTime _nextFlush = DateTime.MinValue;

        public IoTIngestBatchWriter(
            IServiceScopeFactory scopeFactory,
            ILogger<IoTIngestBatchWriter> logger
        )
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
            _channel = Channel.CreateBounded<IoTIngestPayload>(
                new BoundedChannelOptions(1000)
                {
                    FullMode = BoundedChannelFullMode.DropNewest,
                    SingleReader = true,
                    SingleWriter = false,
                }
            );
        }

        public void Enqueue(IoTIngestPayload payload) => _channel.Writer.TryWrite(payload);

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    await Task.Delay(FlushInterval, stoppingToken);

                    if (DateTime.UtcNow < _nextFlush)
                        continue;

                    await FlushAsync(stoppingToken);
                }
            }
            catch (OperationCanceledException) { }
            finally
            {
                await FlushAsync(CancellationToken.None);
            }
        }

        private async Task FlushAsync(CancellationToken ct)
        {
            var events = new List<IoTIngestPayload>();
            while (_channel.Reader.TryRead(out var item))
                events.Add(item);

            if (events.Count == 0)
                return;

            try
            {
                using var scope = _scopeFactory.CreateScope();
                var client = scope.ServiceProvider.GetRequiredService<IIoTIngestClient>();
                await client.IngestBatchAsync(new IoTBatchIngestPayload { Events = events }, ct);

                // Success — reset backoff
                _backoff = TimeSpan.Zero;
            }
            catch (Exception ex)
            {
                if (_backoff == TimeSpan.Zero)
                    _backoff = TimeSpan.FromSeconds(2);
                else if (_backoff < MaxBackoff)
                    _backoff = _backoff * 2;

                if (_backoff > MaxBackoff)
                    _backoff = MaxBackoff;

                _nextFlush = DateTime.UtcNow + _backoff;

                _logger.LogWarning(
                    ex,
                    "Advisory IoT batch ingest failed for {Count} events — discarding. Backing off for {Backoff}",
                    events.Count,
                    _backoff
                );
            }
        }
    }
}

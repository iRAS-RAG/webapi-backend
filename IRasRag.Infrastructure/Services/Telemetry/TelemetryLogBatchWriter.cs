using System.Threading.Channels;
using IRasRag.Application.Common.Interfaces.Persistence;
using IRasRag.Application.Common.Interfaces.Telemetry;
using IRasRag.Application.Common.Models.Telemetry;
using IRasRag.Domain.Entities;
using IRasRag.Infrastructure.Settings;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace IRasRag.Infrastructure.Services.Telemetry
{
    public class TelemetryLogBatchWriter : BackgroundService, ITelemetryLogBatchWriter
    {
        private static readonly TimeSpan FlushInterval = TimeSpan.FromMinutes(1);
        private readonly LogBatchWriterSettings _settings;
        private static readonly TimeSpan RetryBaseDelay = TimeSpan.FromMilliseconds(250);

        private readonly Channel<TelemetryLogWriteModel> _channel;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<TelemetryLogBatchWriter> _logger;

        public TelemetryLogBatchWriter(
            IServiceScopeFactory scopeFactory,
            IOptions<LogBatchWriterSettings> settings,
            ILogger<TelemetryLogBatchWriter> logger)
        {
            _scopeFactory = scopeFactory;
            _settings = settings.Value;
            _logger = logger;

            _channel = Channel.CreateBounded<TelemetryLogWriteModel>(
                new BoundedChannelOptions(20_000)
                {
                    FullMode = BoundedChannelFullMode.Wait,
                    SingleReader = true,
                    SingleWriter = false
                });
        }

        public async Task EnqueueBatchAsync(
            IReadOnlyCollection<TelemetryLogWriteModel> logs,
            CancellationToken cancellationToken = default)
        {
            foreach (var log in logs)
            {
                await _channel.Writer.WriteAsync(log, cancellationToken);
            }
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var reader = _channel.Reader;
            var windows = new Dictionary<Guid, SensorWindowAccumulator>();
            var windowStart = SensorWindowAccumulator.GetWindowStart(DateTime.UtcNow);
            var nextFlush = windowStart.Add(FlushInterval);

            try
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    var now = DateTime.UtcNow;

                    // Time to flush — window expired
                    if (now >= nextFlush)
                    {
                        await FlushWindowsAsync(windows, stoppingToken);
                        windows.Clear();
                        windowStart = SensorWindowAccumulator.GetWindowStart(DateTime.UtcNow);
                        nextFlush = windowStart.Add(FlushInterval);
                    }

                    // Drain available readings into accumulators
                    var remaining = nextFlush - DateTime.UtcNow;
                    if (remaining <= TimeSpan.Zero)
                        continue;

                    var waitTask = reader.WaitToReadAsync(stoppingToken).AsTask();
                    var delayTask = Task.Delay(remaining, stoppingToken);
                    var completed = await Task.WhenAny(waitTask, delayTask);

                    if (completed == delayTask)
                        continue; // window expired — loop back to flush

                    if (!await waitTask)
                        break; // channel completed

                    while (reader.TryRead(out var item))
                    {
                        var windowKey = SensorWindowAccumulator.GetWindowStart(item.CreatedAt);

                        // Reading belongs to current window
                        if (windowKey == windowStart)
                        {
                            if (!windows.TryGetValue(item.SensorId, out var acc))
                            {
                                acc = new SensorWindowAccumulator(windowStart);
                                windows[item.SensorId] = acc;
                            }
                            acc.Add(item.Data, item.IsWarning);
                        }
                        else
                        {
                            // Late reading from previous window — log and discard
                            _logger.LogWarning(
                                "Late reading for sensor {SensorId} belongs to window {WindowKey}, current window {WindowStart} — discarding",
                                item.SensorId, windowKey, windowStart);
                        }
                    }
                }
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Telemetry log batch writer is stopping.");
            }
            finally
            {
                // Flush remaining partial window on shutdown
                if (windows.Count > 0)
                    await FlushWindowsAsync(windows, CancellationToken.None);
            }
        }

        private async Task FlushWindowsAsync(
            Dictionary<Guid, SensorWindowAccumulator> windows,
            CancellationToken cancellationToken)
        {
            if (windows.Count == 0) return;

            var entities = windows
                .Select(kvp => kvp.Value.Flush(kvp.Key))
                .Where(log => log != null)
                .Cast<SensorLog>()
                .ToList();

            if (entities.Count == 0) return;

            // Retry logic stays exactly the same as before
            await PersistWithRetryAsync(entities, cancellationToken);
        }

        private async Task PersistWithRetryAsync(
            List<SensorLog> logs,
            CancellationToken cancellationToken,
            int requeueCount = 0)
        {
            if (logs.Count == 0)
                return;

            Exception? lastException = null;

            for (var attempt = 1; attempt <= _settings.MaxRetryAttempts; attempt++)
            {
                cancellationToken.ThrowIfCancellationRequested();

                try
                {
                    using var scope = _scopeFactory.CreateScope();
                    var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                    var sensorLogRepository = unitOfWork.GetRepository<SensorLog>();

                    await sensorLogRepository.AddRangeAsync(logs);
                    await unitOfWork.SaveChangesAsync(cancellationToken);

                    _logger.LogDebug(
                        "Flushed telemetry log batch with {Count} items on attempt {Attempt}",
                        logs.Count,
                        attempt);
                    return;
                }
                catch (OperationCanceledException)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    lastException = ex;

                    if (attempt == _settings.MaxRetryAttempts)
                        break;

                    // Exponential backoff with jitter to avoid overloading the database during transient issues
                    var backoff = TimeSpan.FromMilliseconds(
                        RetryBaseDelay.TotalMilliseconds * Math.Pow(2, attempt - 1));
                    var jitter = TimeSpan.FromMilliseconds(Random.Shared.Next(25, 125));
                    var delay = backoff + jitter;

                    _logger.LogWarning(
                        ex,
                        "Batch insert failed on attempt {Attempt}/{MaxAttempts}. Retrying in {DelayMs} ms",
                        attempt,
                        _settings.MaxRetryAttempts,
                        delay.TotalMilliseconds);

                    await Task.Delay(delay, cancellationToken);
                }
            }

            _logger.LogError(
                lastException,
                "Batch insert failed after {MaxAttempts} attempts. Re-queueing {Count} logs",
                _settings.MaxRetryAttempts,
                logs.Count);

            // Small delay before re-queuing to avoid tight failure loops
            await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
            await RequeueSummariesAsync(logs, requeueCount + 1, cancellationToken);
        }

        private async Task RequeueSummariesAsync(
            List<SensorLog> logs,
            int requeueCount,
            CancellationToken cancellationToken)
        {
            if (requeueCount > _settings.MaxRequeueCount)
            {
                foreach (var log in logs)
                {
                    _logger.LogError(
                        "Dropping sensor log summary for sensor {SensorId} period {PeriodStart} after {RequeueCount} requeues",
                        log.SensorId, log.PeriodStart, requeueCount);
                }
                return;
            }

            // Small delay before retry to avoid tight failure loops
            await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
            await PersistWithRetryAsync(logs, cancellationToken, requeueCount);
        }
    }

    // Helper class to accumulate sensor readings for a specific time window and compute summary statistics
    public class SensorWindowAccumulator
    {
        private readonly List<double> _samples = new();
        private bool _hasWarning;
        public DateTime PeriodStart { get; }

        public SensorWindowAccumulator(DateTime periodStart)
        {
            PeriodStart = periodStart;
        }

        public void Add(double value, bool isWarning)
        {
            _samples.Add(value);
            if (isWarning) _hasWarning = true;
        }

        public SensorLog? Flush(Guid sensorId)
        {
            if (_samples.Count == 0) return null;

            return new SensorLog
            {
                SensorId = sensorId,
                PeriodStart = PeriodStart,
                Average = _samples.Average(),
                Min = _samples.Min(),
                Max = _samples.Max(),
                SampleCount = _samples.Count,
                HasWarning = _hasWarning
            };
        }
        public static DateTime GetWindowStart(DateTime timestamp)
            => new DateTime(
                timestamp.Year, timestamp.Month, timestamp.Day,
                timestamp.Hour, timestamp.Minute, 0,
                DateTimeKind.Utc);
    }
}
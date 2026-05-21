using IRasRag.Application.Common.Models.Telemetry;

namespace IRasRag.Application.Common.Interfaces.Telemetry
{
    public interface ITelemetryLogBatchWriter
    {
        Task EnqueueBatchAsync(
            IReadOnlyCollection<TelemetryLogWriteModel> logs,
            CancellationToken cancellationToken = default
        );
    }
}

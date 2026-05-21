using IRasRag.Application.Common.Models.Advisory;

namespace IRasRag.Application.Common.Interfaces.Advisory
{
    public interface IIoTIngestClient
    {
        Task IngestBatchAsync(IoTBatchIngestPayload payload, CancellationToken ct = default);
    }
}

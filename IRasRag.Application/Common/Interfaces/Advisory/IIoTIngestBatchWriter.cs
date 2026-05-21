using IRasRag.Application.Common.Models.Advisory;

namespace IRasRag.Application.Common.Interfaces.Advisory
{
    public interface IIoTIngestBatchWriter
    {
        void Enqueue(IoTIngestPayload payload);
    }
}

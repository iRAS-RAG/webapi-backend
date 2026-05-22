namespace IRasRag.Application.Common.Models.Advisory
{
    public sealed class IoTIngestPayload
    {
        public string FarmId { get; init; } = default!;
        public string TankId { get; init; } = default!;
        public string Ts { get; init; } = default!;
        public string? Species { get; init; }
        public string? Stage { get; init; }
        public IReadOnlyList<IoTMetric>? Metrics { get; init; }
    }

    public sealed class IoTMetric
    {
        public string Code { get; init; } = default!;
        public double Value { get; init; }
        public string? Unit { get; init; }
    }

    public sealed class IoTBatchIngestPayload
    {
        public IReadOnlyList<IoTIngestPayload> Events { get; init; } = [];
    }
}

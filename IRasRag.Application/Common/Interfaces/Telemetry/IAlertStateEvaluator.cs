using IRasRag.Domain.Entities;

namespace IRasRag.Application.Common.Interfaces.Telemetry
{
    public interface IAlertStateEvaluator
    {
        Task EvaluateAsync(
            Guid tankId,
            Guid sensorId,
            Guid sensorTypeId,
            Guid? batchId,
            SpeciesThreshold violatedThreshold,
            double value,
            string sensorName,
            string? batchName
        );
    }
}

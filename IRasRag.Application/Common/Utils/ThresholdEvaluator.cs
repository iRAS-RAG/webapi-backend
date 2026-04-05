using IRasRag.Domain.Entities;

namespace IRasRag.Application.Common.Utils
{
    public static class ThresholdEvaluator
    {
        public static bool IsViolation(double value, SpeciesThreshold threshold)
        {
            return value < threshold.MinValue || value > threshold.MaxValue;
        }
    }
}

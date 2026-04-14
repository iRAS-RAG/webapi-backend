using IRasRag.Domain.Entities;
using IRasRag.Domain.Enums;

namespace IRasRag.Application.Common.Utils
{
    public static class ThresholdEvaluator
    {
        public static bool IsViolation(double value, SpeciesThreshold threshold)
        {
            if (value < threshold.MinValue || value > threshold.MaxValue) 
                return true;

            return false;
        }
    }
}

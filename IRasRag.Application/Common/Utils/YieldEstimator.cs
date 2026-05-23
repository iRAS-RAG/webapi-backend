using IRasRag.Domain.Entities;

namespace IRasRag.Application.Common.Utils
{
    public static class YieldEstimator
    {
        public static (int EstimatedCount, double? EstimatedWeightKg) Estimate(
            IEnumerable<SpeciesStageConfig> orderedStages,
            int startQuantity
        )
        {
            if (startQuantity <= 0)
                return (0, 0.0);

            var stages = orderedStages.ToList();
            if (stages.Count == 0)
                return (startQuantity, null);

            double cumulativeSurvival = 1.0;
            foreach (var s in stages)
            {
                var sr = s.SurvivalRate ?? 1.0;
                if (sr < 0)
                    sr = 0;
                if (sr > 1)
                    sr = 1;
                cumulativeSurvival *= sr;
            }

            var estimatedCount = (int)Math.Floor(startQuantity * cumulativeSurvival);

            var stageWithWeight = stages.LastOrDefault(s => s.ExpectedWeightKgPerFish.HasValue);
            if (stageWithWeight != null && stageWithWeight.ExpectedWeightKgPerFish.HasValue)
            {
                var estWeight = estimatedCount * stageWithWeight.ExpectedWeightKgPerFish.Value;
                return (estimatedCount, estWeight);
            }

            return (estimatedCount, null);
        }
    }
}

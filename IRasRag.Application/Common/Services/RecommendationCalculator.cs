using IRasRag.Application.Common.Interfaces;
using IRasRag.Application.Common.Interfaces.Persistence;
using IRasRag.Application.Specifications.SpecificationHelpers;
using IRasRag.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace IRasRag.Application.Common.Services
{
    public class RecommendationCalculator : IRecommendationCalculator
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<RecommendationCalculator> _logger;

        public RecommendationCalculator(
            IUnitOfWork unitOfWork,
            ILogger<RecommendationCalculator> logger
        )
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<int?> GetRecommendedInitialAsync(Guid fishTankId, Guid speciesId)
        {
            try
            {
                var fishTank = await _unitOfWork.GetRepository<FishTank>().GetByIdAsync(fishTankId);
                if (fishTank == null)
                    return null;

                var stageConfigRepo = _unitOfWork.GetRepository<SpeciesStageConfig>();
                var stageConfigs = await stageConfigRepo.ListAsync(
                    new SpecBySpeciesOrderedSpec(speciesId)
                );
                if (stageConfigs == null || !stageConfigs.Any())
                    return null;

                var tankVolume = Math.PI * fishTank.Radius * fishTank.Radius * fishTank.Height;
                if (tankVolume <= 0)
                    return null;

                // Use the last (final) stage's max stocking density but account for
                // cumulative survival across all stages so the returned value is the
                // maximum initial stocking number that will not exceed final-stage capacity.
                var ordered = stageConfigs.OrderBy(s => s.Sequence).ToList();
                var last = ordered.Last();
                if (!last.MaxStockingDensity.HasValue || last.MaxStockingDensity.Value <= 0)
                    return null;

                // Compute cumulative survival from first to last stage, clamping rates to [0,1]
                double cumulativeSurvival = 1.0;
                foreach (var sc in ordered)
                {
                    var sr = sc.SurvivalRate ?? 1.0;
                    if (sr < 0)
                        sr = 0;
                    if (sr > 1)
                        sr = 1;
                    cumulativeSurvival *= sr;
                }

                if (cumulativeSurvival <= 0)
                    return null;

                var maxAllowedAtFinal = Math.Floor(last.MaxStockingDensity.Value * tankVolume);
                if (maxAllowedAtFinal <= 0)
                    return null;

                // maximum initial = floor(maxAllowedAtFinal / cumulativeSurvival)
                var recommended = (int)Math.Floor(maxAllowedAtFinal / cumulativeSurvival);
                return recommended > 0 ? recommended : null;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(
                    ex,
                    "Không thể tính mức đề nghị cho bể {FishTankId} và loài {SpeciesId}",
                    fishTankId,
                    speciesId
                );
                return null;
            }
        }
    }
}

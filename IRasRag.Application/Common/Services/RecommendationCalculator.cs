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

                var last = stageConfigs.OrderBy(s => s.Sequence).Last();
                if (!last.MaxStockingDensity.HasValue || last.MaxStockingDensity.Value <= 0)
                    return null;

                var recommended = (int)Math.Floor(last.MaxStockingDensity.Value * tankVolume);
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

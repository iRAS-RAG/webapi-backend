using AutoMapper;
using IRasRag.Application.Common.Interfaces;
using IRasRag.Application.Common.Models;
using IRasRag.Application.DTOs;
using IRasRag.Application.Services.Interfaces;
using IRasRag.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace IRasRag.Application.Services.Implementations
{
    public class GrowthStageService : IGrowthStageService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<GrowthStageService> _logger;
        private readonly IMapper _mapper;

        public GrowthStageService(
            IUnitOfWork unitOfWork,
            ILogger<GrowthStageService> logger,
            IMapper mapper
        )
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<Result<GrowthStageDto>> CreateGrowthStageAsync(
            CreateGrowthStageDto createDto
        )
        {
            try
            {
                if (string.IsNullOrWhiteSpace(createDto.Name))
                    return Result<GrowthStageDto>.Failure(
                        "Tên giai đoạn sinh trưởng không được để trống.",
                        ResultType.BadRequest
                    );

                if (string.IsNullOrWhiteSpace(createDto.Description))
                    return Result<GrowthStageDto>.Failure(
                        "Mô tả giai đoạn sinh trưởng không được để trống.",
                        ResultType.BadRequest
                    );

                var newGrowthStage = new GrowthStage
                {
                    Name = createDto.Name.Trim(),
                    Description = createDto.Description.Trim(),
                };

                await _unitOfWork.GetRepository<GrowthStage>().AddAsync(newGrowthStage);
                await _unitOfWork.SaveChangesAsync();

                return Result<GrowthStageDto>.Success(
                    _mapper.Map<GrowthStageDto>(newGrowthStage),
                    "Tạo giai đoạn sinh trưởng thành công."
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating growth stage");
                return Result<GrowthStageDto>.Failure(
                    "Lỗi khi tạo giai đoạn sinh trưởng.",
                    ResultType.Unexpected
                );
            }
        }

        public async Task<Result> DeleteGrowthStageAsync(Guid id)
        {
            try
            {
                var growthStage = await _unitOfWork.GetRepository<GrowthStage>().GetByIdAsync(id);

                if (growthStage == null)
                {
                    return Result.Failure(
                        "Giai đoạn sinh trưởng không tồn tại.",
                        ResultType.NotFound
                    );
                }

                _unitOfWork.GetRepository<GrowthStage>().Delete(growthStage);
                await _unitOfWork.SaveChangesAsync();

                return Result.Success("Xóa giai đoạn sinh trưởng thành công.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting growth stage");
                return Result.Failure("Lỗi khi xóa giai đoạn sinh trưởng.", ResultType.Unexpected);
            }
        }

        public async Task<Result<IEnumerable<GrowthStageDto>>> GetAllGrowthStagesAsync()
        {
            try
            {
                var list = await _unitOfWork.GetRepository<GrowthStage>().GetAllAsync();

                return Result<IEnumerable<GrowthStageDto>>.Success(
                    _mapper.Map<IEnumerable<GrowthStageDto>>(list),
                    "Lấy danh sách giai đoạn sinh trưởng thành công."
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all growth stages");
                return Result<IEnumerable<GrowthStageDto>>.Failure(
                    "Lỗi khi truy xuất danh sách giai đoạn sinh trưởng.",
                    ResultType.Unexpected
                );
            }
        }

        public async Task<Result<GrowthStageDto>> GetGrowthStageByIdAsync(Guid id)
        {
            try
            {
                var growthStage = await _unitOfWork.GetRepository<GrowthStage>().GetByIdAsync(id);

                if (growthStage == null)
                    return Result<GrowthStageDto>.Failure(
                        "Giai đoạn sinh trưởng không tồn tại.",
                        ResultType.NotFound
                    );

                var dto = new GrowthStageDto
                {
                    Id = growthStage.Id,
                    Name = growthStage.Name,
                    Description = growthStage.Description,
                };

                return Result<GrowthStageDto>.Success(dto, "Lấy giai đoạn sinh trưởng thành công.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving growth stage by ID");
                return Result<GrowthStageDto>.Failure(
                    "Lỗi khi truy xuất giai đoạn sinh trưởng.",
                    ResultType.Unexpected
                );
            }
        }

        public async Task<Result> UpdateGrowthStageAsync(Guid id, UpdateGrowthStageDto dto)
        {
            try
            {
                var nameToUpdate = string.IsNullOrWhiteSpace(dto.Name) ? null : dto.Name;

                var descriptionToUpdate = string.IsNullOrWhiteSpace(dto.Description)
                    ? null
                    : dto.Description;

                var growthStage = await _unitOfWork.GetRepository<GrowthStage>().GetByIdAsync(id);

                if (growthStage == null)
                    return Result.Failure(
                        "Giai đoạn sinh trưởng không tồn tại.",
                        ResultType.NotFound
                    );

                if (!string.IsNullOrWhiteSpace(nameToUpdate))
                {
                    growthStage.Name = nameToUpdate.Trim();
                }

                if (!string.IsNullOrWhiteSpace(descriptionToUpdate))
                {
                    growthStage.Description = descriptionToUpdate.Trim();
                }

                await _unitOfWork.SaveChangesAsync();

                return Result.Success("Cập nhật giai đoạn sinh trưởng thành công.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating growth stage");
                return Result.Failure(
                    "Lỗi khi cập nhật giai đoạn sinh trưởng.",
                    ResultType.Unexpected
                );
            }
        }
    }
}

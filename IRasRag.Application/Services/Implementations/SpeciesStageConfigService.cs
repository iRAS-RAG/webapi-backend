using AutoMapper;
using IRasRag.Application.Common.Interfaces.Persistence;
using IRasRag.Application.Common.Models;
using IRasRag.Application.Common.Models.Pagination;
using IRasRag.Application.Common.Utils;
using IRasRag.Application.DTOs;
using IRasRag.Application.Services.Interfaces;
using IRasRag.Application.Specifications.SpeciesStageConfigSpecifications;
using IRasRag.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace IRasRag.Application.Services.Implementations
{
    public class SpeciesStageConfigService : ISpeciesStageConfigService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<SpeciesStageConfigService> _logger;
        private readonly IMapper _mapper;

        public SpeciesStageConfigService(
            IUnitOfWork unitOfWork,
            ILogger<SpeciesStageConfigService> logger,
            IMapper mapper
        )
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<Result<SpeciesStageConfigDto>> CreateSpeciesStageConfig(
            CreateSpeciesStageConfigDto dto
        )
        {
            try
            {
                if (
                    await _unitOfWork.GetRepository<Species>().AnyAsync(s => s.Id == dto.SpeciesId)
                    == false
                )
                    return Result<SpeciesStageConfigDto>.Failure(
                        "Loài cá không tồn tại.",
                        ResultType.BadRequest
                    );

                if (
                    await _unitOfWork
                        .GetRepository<GrowthStage>()
                        .AnyAsync(gs => gs.Id == dto.GrowthStageId) == false
                )
                    return Result<SpeciesStageConfigDto>.Failure(
                        "Giai đoạn sinh trưởng không tồn tại.",
                        ResultType.BadRequest
                    );

                if (dto.FeedTypeIds == null || dto.FeedTypeIds.Count == 0)
                    return Result<SpeciesStageConfigDto>.Failure(
                        "Phải chọn ít nhất một kiểu cho ăn.",
                        ResultType.BadRequest
                    );

                var requestedFeedTypeIds = dto.FeedTypeIds.Distinct().ToList();
                var feedTypes = await _unitOfWork
                    .GetRepository<FeedType>()
                    .FindAllAsync(ft => requestedFeedTypeIds.Contains(ft.Id));

                if (feedTypes.Count != requestedFeedTypeIds.Count)
                    return Result<SpeciesStageConfigDto>.Failure(
                        "Kiểu cho ăn không tồn tại.",
                        ResultType.BadRequest
                    );

                var exists = await _unitOfWork
                    .GetRepository<SpeciesStageConfig>()
                    .AnyAsync(ssc =>
                        ssc.SpeciesId == dto.SpeciesId && ssc.GrowthStageId == dto.GrowthStageId
                    );

                if (exists)
                    return Result<SpeciesStageConfigDto>.Failure(
                        "Cấu hình giai đoạn sinh trưởng của cá ở giai đoạn này đã tồn tại.",
                        ResultType.Conflict
                    );

                var newConfig = _mapper.Map<SpeciesStageConfig>(dto);
                newConfig.FeedTypes = feedTypes.ToList();

                await _unitOfWork.GetRepository<SpeciesStageConfig>().AddAsync(newConfig);
                await _unitOfWork.SaveChangesAsync();

                return Result<SpeciesStageConfigDto>.Success(
                    _mapper.Map<SpeciesStageConfigDto>(newConfig),
                    "Tạo cấu hình giai đoạn sinh trưởng của cá thành công."
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating SpeciesStageConfig");
                return Result<SpeciesStageConfigDto>.Failure(
                    "Lỗi khi tạo cấu hình giai đoạn sinh trưởng của cá.",
                    ResultType.Unexpected
                );
            }
        }

        public async Task<Result> DeleteSpeciesStageConfig(Guid id)
        {
            try
            {
                var config = await _unitOfWork.GetRepository<SpeciesStageConfig>().GetByIdAsync(id);
                if (config == null)
                    return Result.Failure(
                        "Cấu hình giai đoạn sinh trưởng của cá không tồn tại.",
                        ResultType.NotFound
                    );

                _unitOfWork.GetRepository<SpeciesStageConfig>().Delete(config);
                await _unitOfWork.SaveChangesAsync();
                return Result.Success("Xóa cấu hình giai đoạn sinh trưởng của cá thành công.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting SpeciesStageConfig");
                return Result.Failure(
                    "Lỗi khi xóa cấu hình giai đoạn sinh trưởng của cá.",
                    ResultType.Unexpected
                );
            }
        }

        public async Task<Result<SpeciesStageConfigDto>> GetSpeciesStageConfigById(Guid id)
        {
            try
            {
                var config = await _unitOfWork
                    .GetRepository<SpeciesStageConfig>()
                    .FirstOrDefaultAsync(new SpeciesStageConfigByIdSpec(id));

                if (config == null)
                    return Result<SpeciesStageConfigDto>.Failure(
                        "Cấu hình giai đoạn sinh trưởng của cá không tồn tại.",
                        ResultType.NotFound
                    );

                return Result<SpeciesStageConfigDto>.Success(
                    _mapper.Map<SpeciesStageConfigDto>(config),
                    "Lấy cấu hình giai đoạn sinh trưởng của cá thành công."
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving SpeciesStageConfig by ID");
                return Result<SpeciesStageConfigDto>.Failure(
                    "Lỗi khi truy xuất cấu hình giai đoạn sinh trưởng của cá.",
                    ResultType.Unexpected
                );
            }
        }

        public async Task<PaginatedResult<SpeciesStageConfigDto>> GetAllSpeciesStageConfigsAsync(
            SpeciesStageConfigListRequest request
        )
        {
            try
            {
                _logger.LogInformation(
                    "Bắt đầu lấy danh sách cấu hình giai đoạn sinh trưởng (Page: {Page}, PageSize: {PageSize})",
                    request.Page,
                    request.PageSize
                );

                var repository = _unitOfWork.GetRepository<SpeciesStageConfig>();
                var pagedResult = await repository.GetPagedAsync(
                    new SpeciesStageConfigListSpec(request),
                    request.Page,
                    request.PageSize
                );

                var configDtos = pagedResult.Items;

                _logger.LogInformation(
                    "Lấy danh sách cấu hình giai đoạn sinh trưởng thành công: {Count} cấu hình",
                    configDtos.Count
                );

                return new PaginatedResult<SpeciesStageConfigDto>
                {
                    Message =
                        configDtos.Count == 0
                            ? "Không có cấu hình giai đoạn sinh trưởng nào"
                            : "Lấy danh sách cấu hình giai đoạn sinh trưởng thành công",
                    Data = configDtos,
                    Meta = PaginationBuilder.BuildPaginationMetadata(
                        request.Page,
                        request.PageSize,
                        pagedResult.TotalItems
                    ),
                    Links = PaginationBuilder.BuildPaginationLinks(
                        request.Page,
                        request.PageSize,
                        pagedResult.TotalItems
                    ),
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi truy xuất danh sách cấu hình giai đoạn sinh trưởng");

                return new PaginatedResult<SpeciesStageConfigDto>
                {
                    Message = "Lỗi khi truy xuất danh sách cấu hình giai đoạn sinh trưởng",
                    Data = Array.Empty<SpeciesStageConfigDto>(),
                    Meta = null,
                    Links = null,
                };
            }
        }

        public async Task<PaginatedResult<SpeciesStageConfigDto>> GetSpeciesStageConfigsBySpeciesId(
            Guid speciesId,
            SpeciesStageConfigListRequest request
        )
        {
            var species = await _unitOfWork
                .GetRepository<Species>()
                .AnyAsync(s => s.Id == speciesId);

            if (species == false)
                return new PaginatedResult<SpeciesStageConfigDto>
                {
                    Message = "Loài cá không tồn tại",
                    Data = Array.Empty<SpeciesStageConfigDto>(),
                    Meta = null,
                    Links = null,
                };

            var pagedResult = await _unitOfWork
                .GetRepository<SpeciesStageConfig>()
                .GetPagedAsync(
                    new SpeciesStageConfigBySpeciesIdSpec(speciesId, request),
                    request.Page,
                    request.PageSize
                );

            return new PaginatedResult<SpeciesStageConfigDto>
            {
                Message =
                    pagedResult.Items.Count == 0
                        ? "Không có cấu hình giai đoạn sinh trưởng nào cho loài cá này"
                        : "Lấy danh sách cấu hình giai đoạn sinh trưởng cho loài cá thành công",
                Data = pagedResult.Items,
                Meta = PaginationBuilder.BuildPaginationMetadata(
                    request.Page,
                    request.PageSize,
                    pagedResult.TotalItems
                ),
                Links = PaginationBuilder.BuildPaginationLinks(
                    request.Page,
                    request.PageSize,
                    pagedResult.TotalItems
                ),
            };
        }

        public async Task<Result> UpdateSpeciesStageConfig(Guid id, UpdateSpeciesStageConfigDto dto)
        {
            try
            {
                var config = await _unitOfWork.GetRepository<SpeciesStageConfig>().GetByIdAsync(id);

                if (config == null)
                    return Result.Failure(
                        "Cấu hình giai đoạn sinh trưởng của cá không tồn tại.",
                        ResultType.NotFound
                    );

                if (dto.FeedTypeIds != null)
                {
                    if (dto.FeedTypeIds.Count == 0)
                        return Result.Failure(
                            "Phải chọn ít nhất một kiểu cho ăn.",
                            ResultType.BadRequest
                        );

                    var requestedFeedTypeIds = dto.FeedTypeIds.Distinct().ToList();
                    var feedTypes = await _unitOfWork
                        .GetRepository<FeedType>()
                        .FindAllAsync(ft => requestedFeedTypeIds.Contains(ft.Id));

                    if (feedTypes.Count != requestedFeedTypeIds.Count)
                        return Result<SpeciesStageConfigDto>.Failure(
                            "Kiểu cho ăn không tồn tại.",
                            ResultType.BadRequest
                        );

                    config.FeedTypes = feedTypes.ToList();
                }

                _mapper.Map(dto, config);
                await _unitOfWork.SaveChangesAsync();

                return Result.Success("Cập nhật cấu hình giai đoạn sinh trưởng của cá thành công.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating SpeciesStageConfig");
                return Result.Failure(
                    "Lỗi khi cập nhật cấu hình giai đoạn sinh trưởng của cá.",
                    ResultType.Unexpected
                );
            }
        }
    }
}

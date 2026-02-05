using AutoMapper;
using IRasRag.Application.Common.Interfaces.Persistence;
using IRasRag.Application.Common.Models;
using IRasRag.Application.Common.Models.Pagination;
using IRasRag.Application.Common.Utils;
using IRasRag.Application.DTOs;
using IRasRag.Application.Services.Interfaces;
using IRasRag.Application.Specifications;
using IRasRag.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace IRasRag.Application.Services.Implementations
{
    public class SpeciesThresholdService : ISpeciesThresholdService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<SpeciesThresholdService> _logger;
        private readonly IMapper _mapper;

        public SpeciesThresholdService(
            IUnitOfWork unitOfWork,
            ILogger<SpeciesThresholdService> logger,
            IMapper mapper
        )
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<Result<SpeciesThresholdDto>> CreateSpeciesThreshold(
            CreateSpeciesThresholdDto dto
        )
        {
            try
            {
                if (dto.MinValue >= dto.MaxValue)
                    return Result<SpeciesThresholdDto>.Failure(
                        "Giá trị Min phải nhỏ hơn Max.",
                        ResultType.BadRequest
                    );

                if (
                    !await _unitOfWork.GetRepository<Species>().AnyAsync(s => s.Id == dto.SpeciesId)
                )
                    return Result<SpeciesThresholdDto>.Failure(
                        "Loài cá không tồn tại.",
                        ResultType.BadRequest
                    );

                if (
                    !await _unitOfWork
                        .GetRepository<GrowthStage>()
                        .AnyAsync(gs => gs.Id == dto.GrowthStageId)
                )
                    return Result<SpeciesThresholdDto>.Failure(
                        "Giai đoạn sinh trưởng không tồn tại.",
                        ResultType.BadRequest
                    );

                if (
                    !await _unitOfWork
                        .GetRepository<SensorType>()
                        .AnyAsync(st => st.Id == dto.SensorTypeId)
                )
                    return Result<SpeciesThresholdDto>.Failure(
                        "Loại cảm biến không tồn tại.",
                        ResultType.BadRequest
                    );

                var exists = await _unitOfWork
                    .GetRepository<SpeciesThreshold>()
                    .AnyAsync(st =>
                        st.SpeciesId == dto.SpeciesId
                        && st.GrowthStageId == dto.GrowthStageId
                        && st.SensorTypeId == dto.SensorTypeId
                    );

                if (exists)
                    return Result<SpeciesThresholdDto>.Failure(
                        "Ngưỡng sinh trưởng này đã tồn tại.",
                        ResultType.Conflict
                    );

                var newThreshold = _mapper.Map<SpeciesThreshold>(dto);

                await _unitOfWork.GetRepository<SpeciesThreshold>().AddAsync(newThreshold);

                await _unitOfWork.SaveChangesAsync();

                return Result<SpeciesThresholdDto>.Success(
                    _mapper.Map<SpeciesThresholdDto>(newThreshold),
                    "Tạo ngưỡng sinh trưởng thành công."
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating species threshold");
                return Result<SpeciesThresholdDto>.Failure(
                    "Lỗi khi tạo ngưỡng sinh trưởng.",
                    ResultType.Unexpected
                );
            }
        }

        public async Task<Result> DeleteSpeciesThreshold(Guid id)
        {
            try
            {
                var threshold = await _unitOfWork
                    .GetRepository<SpeciesThreshold>()
                    .GetByIdAsync(id);
                if (threshold == null)
                    return Result.Failure("Ngưỡng sinh trưởng không tồn tại.", ResultType.NotFound);
                _unitOfWork.GetRepository<SpeciesThreshold>().Delete(threshold);
                await _unitOfWork.SaveChangesAsync();
                return Result.Success("Xóa ngưỡng sinh trưởng thành công.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting species threshold");
                return Result.Failure("Lỗi khi xóa ngưỡng sinh trưởng.", ResultType.Unexpected);
            }
        }

        public async Task<Result<SpeciesThresholdDto>> GetSpeciesThresholdById(Guid id)
        {
            try
            {
                var threshold = await _unitOfWork
                    .GetRepository<SpeciesThreshold>()
                    .FirstOrDefaultAsync(new SpeciesThresholdByIdSpec(id));

                if (threshold == null)
                    return Result<SpeciesThresholdDto>.Failure(
                        "Ngưỡng sinh trưởng không tồn tại.",
                        ResultType.NotFound
                    );

                return Result<SpeciesThresholdDto>.Success(
                    threshold,
                    "Lấy ngưỡng sinh trưởng thành công."
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving species threshold by ID");
                return Result<SpeciesThresholdDto>.Failure(
                    "Lỗi khi truy xuất ngưỡng sinh trưởng.",
                    ResultType.Unexpected
                );
            }
        }

        public async Task<PaginatedResult<SpeciesThresholdDto>> GetAllSpeciesThresholdsAsync(
            int page,
            int pageSize
        )
        {
            try
            {
                _logger.LogInformation(
                    "Bắt đầu lấy danh sách ngưỡng sinh trưởng (Page: {Page}, PageSize: {PageSize})",
                    page,
                    pageSize
                );

                var repository = _unitOfWork.GetRepository<SpeciesThreshold>();
                var pagedResult = await repository.GetPagedAsync(
                    new SpeciesThresholdListSpec(),
                    page,
                    pageSize
                );

                var thresholdDtos = _mapper.Map<IReadOnlyList<SpeciesThresholdDto>>(
                    pagedResult.Items
                );

                _logger.LogInformation(
                    "Lấy danh sách ngưỡng sinh trưởng thành công: {Count} ngưỡng",
                    thresholdDtos.Count
                );

                return new PaginatedResult<SpeciesThresholdDto>
                {
                    Message =
                        thresholdDtos.Count == 0
                            ? "Không có ngưỡng sinh trưởng nào"
                            : "Lấy danh sách ngưỡng sinh trưởng thành công",
                    Data = thresholdDtos,
                    Meta = PaginationBuilder.BuildPaginationMetadata(
                        page,
                        pageSize,
                        pagedResult.TotalItems
                    ),
                    Links = PaginationBuilder.BuildPaginationLinks(
                        page,
                        pageSize,
                        pagedResult.TotalItems
                    ),
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi truy xuất danh sách ngưỡng sinh trưởng");

                return new PaginatedResult<SpeciesThresholdDto>
                {
                    Message = "Lỗi khi truy xuất danh sách ngưỡng sinh trưởng",
                    Data = Array.Empty<SpeciesThresholdDto>(),
                    Meta = null,
                    Links = null,
                };
            }
        }

        public async Task<Result> UpdateSpeciesThreshold(Guid id, UpdateSpeciesThresholdDto dto)
        {
            try
            {
                if (dto.MinValue >= dto.MaxValue)
                    return Result.Failure("Giá trị Min phải nhỏ hơn Max.", ResultType.BadRequest);

                var threshold = await _unitOfWork
                    .GetRepository<SpeciesThreshold>()
                    .GetByIdAsync(id);
                if (threshold == null)
                    return Result.Failure("Ngưỡng sinh trưởng không tồn tại.", ResultType.NotFound);

                _mapper.Map(dto, threshold);
                await _unitOfWork.SaveChangesAsync();
                return Result.Success("Cập nhật ngưỡng sinh trưởng thành công.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating species threshold");
                return Result.Failure(
                    "Lỗi khi cập nhật ngưỡng sinh trưởng.",
                    ResultType.Unexpected
                );
            }
        }
    }
}

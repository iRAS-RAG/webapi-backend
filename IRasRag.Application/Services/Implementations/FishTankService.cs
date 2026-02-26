using AutoMapper;
using IRasRag.Application.Common.Interfaces.Persistence;
using IRasRag.Application.Common.Models;
using IRasRag.Application.Common.Models.Pagination;
using IRasRag.Application.Common.Utils;
using IRasRag.Application.DTOs;
using IRasRag.Application.Services.Interfaces;
using IRasRag.Application.Specifications.FishTankSpecifications;
using IRasRag.Application.Specifications.SpeciesThresholdSpecifications;
using IRasRag.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace IRasRag.Application.Services.Implementations
{
    public class FishTankService : IFishTankService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<FishTankService> _logger;
        private readonly IMapper _mapper;

        public FishTankService(
            IUnitOfWork unitOfWork,
            ILogger<FishTankService> logger,
            IMapper mapper
        )
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<Result<FishTankDto>> CreateFishTankAsync(CreateFishTankDto createDto)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(createDto.Name))
                    return Result<FishTankDto>.Failure(
                        "Tên bể cá không được để trống.",
                        ResultType.BadRequest
                    );

                if (createDto.Height <= 0)
                    return Result<FishTankDto>.Failure(
                        "Chiều cao phải lớn hơn 0.",
                        ResultType.BadRequest
                    );

                if (createDto.Radius <= 0)
                    return Result<FishTankDto>.Failure(
                        "Bán kính phải lớn hơn 0.",
                        ResultType.BadRequest
                    );

                if (string.IsNullOrWhiteSpace(createDto.CameraUrl))
                    return Result<FishTankDto>.Failure(
                        "URL camera không được để trống.",
                        ResultType.BadRequest
                    );

                // Kiểm tra trang trại tồn tại
                var farm = await _unitOfWork.GetRepository<Farm>().GetByIdAsync(createDto.FarmId);
                if (farm == null || farm.IsDeleted)
                    return Result<FishTankDto>.Failure(
                        "Trang trại không tồn tại.",
                        ResultType.BadRequest
                    );

                var newFishTank = new FishTank
                {
                    Name = createDto.Name.Trim(),
                    Height = createDto.Height,
                    Radius = createDto.Radius,
                    FarmId = createDto.FarmId,
                    TopicCode = createDto.TopicCode?.Trim(),
                    CameraUrl = createDto.CameraUrl.Trim(),
                    IsDeleted = false,
                };

                await _unitOfWork.GetRepository<FishTank>().AddAsync(newFishTank);
                await _unitOfWork.SaveChangesAsync();

                var resultDto = new FishTankDto
                {
                    Id = newFishTank.Id,
                    Name = newFishTank.Name,
                    Volume = (float)(
                        Math.PI * newFishTank.Radius * newFishTank.Radius * newFishTank.Height
                    ),
                    FarmId = newFishTank.FarmId,
                    FarmName = farm.Name,
                    TopicCode = newFishTank.TopicCode,
                    CameraUrl = newFishTank.CameraUrl,
                };

                return Result<FishTankDto>.Success(resultDto, "Tạo bể cá thành công.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tạo bể cá");
                return Result<FishTankDto>.Failure("Lỗi khi tạo bể cá.", ResultType.Unexpected);
            }
        }

        public async Task<Result> DeleteFishTankAsync(Guid id)
        {
            try
            {
                var fishTank = await _unitOfWork.GetRepository<FishTank>().GetByIdAsync(id);

                if (fishTank == null || fishTank.IsDeleted)
                {
                    return Result.Failure("Bể cá không tồn tại.", ResultType.NotFound);
                }

                // Soft delete
                fishTank.IsDeleted = true;
                fishTank.DeletedAt = DateTime.UtcNow;

                _unitOfWork.GetRepository<FishTank>().Update(fishTank);
                await _unitOfWork.SaveChangesAsync();

                return Result.Success("Xóa bể cá thành công.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xóa bể cá");
                return Result.Failure("Lỗi khi xóa bể cá.", ResultType.Unexpected);
            }
        }

        public async Task<PaginatedResult<FishTankDto>> GetAllFishTanksAsync(
            FishTankListRequest request
        )
        {
            try
            {
                var fishTankRepo = _unitOfWork.GetRepository<FishTank>();
                var spec = new FishTankDtoListSpec(request);
                var pagedResult = await fishTankRepo.GetPagedAsync(
                    spec,
                    request.Page,
                    request.PageSize
                );

                return new PaginatedResult<FishTankDto>
                {
                    Message =
                        pagedResult.TotalItems == 0
                            ? "Không có bể cá nào"
                            : "Lấy danh sách bể cá thành công.",
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi truy xuất danh sách bể cá");

                return new PaginatedResult<FishTankDto>
                {
                    Message = "Lỗi khi truy xuất danh sách bể cá.",
                    Data = Array.Empty<FishTankDto>(),
                    Meta = null,
                    Links = null,
                };
            }
        }

        public async Task<
            Result<IReadOnlyList<FishTankMetricDto>>
        > GetLatestFishTankMetricsByFarmAsync(Guid farmId)
        {
            var farm = await _unitOfWork.GetRepository<Farm>().GetByIdAsync(farmId);
            if (farm == null)
            {
                return Result<IReadOnlyList<FishTankMetricDto>>.Failure(
                    "Trang trại không tồn tại.",
                    ResultType.NotFound
                );
            }
            var result = await _unitOfWork.FishTanks.GetLatestFishTankMetricsByFarmIdAsync(farmId);
            return Result<IReadOnlyList<FishTankMetricDto>>.Success(
                result,
                "Lấy thông tin chỉ số các bể cá thành công."
            );
        }

        public async Task<
            Result<FishTankMetricDto>    
        > GetLatestFishTankMetricsByTankAsync(Guid tankId)
        {
            var tank = await _unitOfWork.GetRepository<FishTank>().GetByIdAsync(tankId);
            if (tank == null)
            {
                return Result<FishTankMetricDto>.Failure(
                    "Bể cá không tồn tại.",
                    ResultType.NotFound
                );
            }
            var result = await _unitOfWork.FishTanks.GetLatestFishTankMetricsByTankIdAsync(tankId);
            return Result<FishTankMetricDto>.Success(
                result,
                "Lấy thông tin chỉ số của bể cá thành công."
            );
        }

        public async Task<Result<FishTankDto>> GetFishTankByIdAsync(Guid id)
        {
            var dto = await _unitOfWork
                .GetRepository<FishTank>()
                .FirstOrDefaultAsync(new FishTankDtoSpec(id));
            if (dto == null)
                return Result<FishTankDto>.Failure("Bể cá không tồn tại.", ResultType.NotFound);

            return Result<FishTankDto>.Success(dto, "Lấy thông tin bể cá thành công.");
        }

        public async Task<Result<FishTankDto>> UpdateFishTankAsync(Guid id, UpdateFishTankDto dto)
        {
            try
            {
                var fishTank = await _unitOfWork.GetRepository<FishTank>().GetByIdAsync(id);

                if (fishTank == null || fishTank.IsDeleted)
                    return Result<FishTankDto>.Failure("Bể cá không tồn tại.", ResultType.NotFound);

                if (!string.IsNullOrWhiteSpace(dto.Name))
                    fishTank.Name = dto.Name.Trim();

                if (dto.Height.HasValue)
                {
                    if (dto.Height.Value <= 0)
                        return Result<FishTankDto>.Failure(
                            "Chiều cao phải lớn hơn 0.",
                            ResultType.BadRequest
                        );
                    fishTank.Height = dto.Height.Value;
                }

                if (dto.Radius.HasValue)
                {
                    if (dto.Radius.Value <= 0)
                        return Result<FishTankDto>.Failure(
                            "Bán kính phải lớn hơn 0.",
                            ResultType.BadRequest
                        );
                    fishTank.Radius = dto.Radius.Value;
                }

                string? farmName = null;
                if (dto.FarmId.HasValue)
                {
                    var farm = await _unitOfWork
                        .GetRepository<Farm>()
                        .GetByIdAsync(dto.FarmId.Value);
                    if (farm == null || farm.IsDeleted)
                        return Result<FishTankDto>.Failure(
                            "Trang trại không tồn tại.",
                            ResultType.BadRequest
                        );
                    fishTank.FarmId = dto.FarmId.Value;
                    farmName = farm.Name;
                }

                if (!string.IsNullOrWhiteSpace(dto.TopicCode))
                    fishTank.TopicCode = dto.TopicCode.Trim();

                if (!string.IsNullOrWhiteSpace(dto.CameraUrl))
                    fishTank.CameraUrl = dto.CameraUrl.Trim();

                _unitOfWork.GetRepository<FishTank>().Update(fishTank);
                await _unitOfWork.SaveChangesAsync();

                if (farmName == null)
                {
                    var farm = await _unitOfWork
                        .GetRepository<Farm>()
                        .GetByIdAsync(fishTank.FarmId);
                    farmName = farm?.Name ?? "Unknown";
                }

                var resultDto = new FishTankDto
                {
                    Id = fishTank.Id,
                    Name = fishTank.Name,
                    Volume = (float)(Math.PI * fishTank.Radius * fishTank.Radius * fishTank.Height),
                    FarmId = fishTank.FarmId,
                    FarmName = farmName,
                    TopicCode = fishTank.TopicCode,
                    CameraUrl = fishTank.CameraUrl,
                };

                return Result<FishTankDto>.Success(resultDto, "Cập nhật bể cá thành công.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi cập nhật bể cá");
                return Result<FishTankDto>.Failure(
                    "Lỗi khi cập nhật bể cá.",
                    ResultType.Unexpected
                );
            }
        }
    }
}

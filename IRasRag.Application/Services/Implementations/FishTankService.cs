using AutoMapper;
using IRasRag.Application.Common.Interfaces.Persistence;
using IRasRag.Application.Common.Models;
using IRasRag.Application.Common.Models.Pagination;
using IRasRag.Application.Common.Utils;
using IRasRag.Application.DTOs;
using IRasRag.Application.Services.Interfaces;
using IRasRag.Application.Specifications.FishTankSpecifications;
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
                    Height = newFishTank.Height,
                    Radius = newFishTank.Radius,
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

                var fishTankDtos = pagedResult.Items.ToList();

                return new PaginatedResult<FishTankDto>
                {
                    Message =
                        fishTankDtos.Count == 0
                            ? "Không có bể cá nào"
                            : "Lấy danh sách bể cá thành công.",
                    Data = fishTankDtos,
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

        public async Task<Result<FishTankDto>> GetFishTankByIdAsync(Guid id)
        {
            try
            {
                var fishTank = await _unitOfWork.GetRepository<FishTank>().GetByIdAsync(id);

                if (fishTank == null || fishTank.IsDeleted)
                    return Result<FishTankDto>.Failure("Bể cá không tồn tại.", ResultType.NotFound);

                var farm = await _unitOfWork.GetRepository<Farm>().GetByIdAsync(fishTank.FarmId);

                var dto = new FishTankDto
                {
                    Id = fishTank.Id,
                    Name = fishTank.Name,
                    Height = fishTank.Height,
                    Radius = fishTank.Radius,
                    FarmId = fishTank.FarmId,
                    FarmName = farm?.Name ?? "Unknown",
                    TopicCode = fishTank.TopicCode,
                    CameraUrl = fishTank.CameraUrl,
                };

                return Result<FishTankDto>.Success(dto, "Lấy thông tin bể cá thành công.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi truy xuất thông tin bể cá");
                return Result<FishTankDto>.Failure(
                    "Lỗi khi truy xuất thông tin bể cá.",
                    ResultType.Unexpected
                );
            }
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
                    Height = fishTank.Height,
                    Radius = fishTank.Radius,
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

        #region Tank Status & Latest Data
        public async Task<Result<List<TankSensorLatestDataDto>>> GetTankLatestDataAsync(Guid tankId)
        {
            try
            {
                _logger.LogInformation("Bắt đầu lấy dữ liệu mới nhất của bể: {TankId}", tankId);

                var tank = await _unitOfWork.GetRepository<FishTank>().GetByIdAsync(tankId);

                if (tank == null)
                {
                    _logger.LogWarning("Không tìm thấy bể cá với Id: {TankId}", tankId);
                    return Result<List<TankSensorLatestDataDto>>.Failure(
                        $"Không tìm thấy bể cá với Id: {tankId}",
                        ResultType.NotFound
                    );
                }

                var result = (await _unitOfWork
                    .GetRepository<Sensor>()
                    .ListAsync(new TankSensorLatestDataSpec(tankId)))
                    .ToList();

                _logger.LogInformation(
                    "Lấy dữ liệu mới nhất thành công: {Count} cảm biến cho bể {TankId}",
                    result.Count,
                    tankId
                );

                return Result<List<TankSensorLatestDataDto>>.Success(
                    result,
                    result.Count == 0
                        ? "Bể cá chưa có cảm biến nào"
                        : $"Lấy dữ liệu mới nhất thành công: {result.Count} cảm biến"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy dữ liệu mới nhất của bể: {TankId}", tankId);
                return Result<List<TankSensorLatestDataDto>>.Failure(
                    "Đã xảy ra lỗi khi lấy dữ liệu mới nhất",
                    ResultType.Unexpected
                );
            }
        }
        public async Task<Result<TankStatusDto>> GetTankStatusAsync(Guid tankId)
        {
            try
            {
                _logger.LogInformation("Bắt đầu lấy trạng thái bể: {TankId}", tankId);

                var tank = await _unitOfWork.GetRepository<FishTank>().GetByIdAsync(tankId);

                if (tank == null)
                {
                    _logger.LogWarning("Không tìm thấy bể cá với Id: {TankId}", tankId);
                    return Result<TankStatusDto>.Failure(
                        $"Không tìm thấy bể cá với Id: {tankId}",
                        ResultType.NotFound
                    );
                }

                var sensors = (await _unitOfWork
                    .GetRepository<Sensor>()
                    .ListAsync(new TankSensorLatestDataSpec(tankId)))
                    .ToList();

                var totalSensors = sensors.Count;
                var warningSensors = sensors.Count(s => s.IsWarning == true);

                var statusDto = new TankStatusDto
                {
                    TankId = tank.Id,
                    TankName = tank.Name,
                    Status = warningSensors > 0 ? "Warning" : "Normal",
                    TotalSensors = totalSensors,
                    WarningSensors = warningSensors,
                };

                _logger.LogInformation(
                    "Lấy trạng thái bể thành công: {TankId} - {Status}",
                    tankId,
                    statusDto.Status
                );

                return Result<TankStatusDto>.Success(statusDto, $"Trạng thái bể: {statusDto.Status}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy trạng thái bể: {TankId}", tankId);
                return Result<TankStatusDto>.Failure(
                    "Đã xảy ra lỗi khi lấy trạng thái bể",
                    ResultType.Unexpected
                );
            }
        }
        #endregion
    }
}

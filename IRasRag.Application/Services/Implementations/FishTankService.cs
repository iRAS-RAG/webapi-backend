using AutoMapper;
using IRasRag.Application.Common.Constants;
using IRasRag.Application.Common.Interfaces;
using IRasRag.Application.Common.Interfaces.Auth;
using IRasRag.Application.Common.Interfaces.Persistence;
using IRasRag.Application.Common.Interfaces.Telemetry;
using IRasRag.Application.Common.Models;
using IRasRag.Application.Common.Models.Pagination;
using IRasRag.Application.Common.Services;
using IRasRag.Application.Common.Utils;
using IRasRag.Application.DTOs;
using IRasRag.Application.Services.Interfaces;
using IRasRag.Application.Specifications.FishTankSpecifications;
using IRasRag.Domain.Entities;
using IRasRag.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace IRasRag.Application.Services.Implementations
{
    public class FishTankService : IFishTankService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRecommendationCalculator _recommendationCalculator;
        private readonly ILogger<FishTankService> _logger;
        private readonly IMapper _mapper;
        private readonly IAuditLogService _auditLogService;
        private readonly ICurrentUserAccessor _currentUserAccessor;
        private readonly ILatestTelemetryCacheService _latestTelemetryCache;

        public FishTankService(
            IUnitOfWork unitOfWork,
            ILogger<FishTankService> logger,
            IMapper mapper,
            IAuditLogService auditLogService,
            ICurrentUserAccessor currentUserAccessor,
            ILatestTelemetryCacheService latestTelemetryCache,
            IRecommendationCalculator? recommendationCalculator = null
        )
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
            _auditLogService = auditLogService;
            _currentUserAccessor = currentUserAccessor;
            _latestTelemetryCache = latestTelemetryCache;
            _recommendationCalculator =
                recommendationCalculator
                ?? new RecommendationCalculator(
                    _unitOfWork,
                    Microsoft
                        .Extensions
                        .Logging
                        .Abstractions
                        .NullLogger<RecommendationCalculator>
                        .Instance
                );
        }

        public async Task<Result<List<RecommendedInitialDto>>> GetRecommendedInitialsAsync(
            Guid tankId
        )
        {
            try
            {
                var tank = await _unitOfWork.GetRepository<FishTank>().GetByIdAsync(tankId);
                if (tank == null)
                {
                    return Result<List<RecommendedInitialDto>>.Failure(
                        "Bể cá không tồn tại.",
                        ResultType.NotFound
                    );
                }

                var speciesList = (
                    await _unitOfWork.GetRepository<Species>().GetAllAsync()
                ).ToList();
                var results = new List<RecommendedInitialDto>();

                foreach (var sp in speciesList)
                {
                    int? recommended = null;
                    try
                    {
                        recommended = await _recommendationCalculator.GetRecommendedInitialAsync(
                            tankId,
                            sp.Id
                        );
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(
                            ex,
                            "Không thể tính mức đề nghị cho bể {TankId} và loài {SpeciesId}",
                            tankId,
                            sp.Id
                        );
                        recommended = null;
                    }

                    results.Add(
                        new RecommendedInitialDto
                        {
                            SpeciesId = sp.Id,
                            SpeciesName = sp.Name,
                            RecommendedInitial = recommended,
                        }
                    );
                }

                return Result<List<RecommendedInitialDto>>.Success(
                    results,
                    "Lấy mức đề nghị thành công"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy mức đề nghị cho bể {TankId}", tankId);
                return Result<List<RecommendedInitialDto>>.Failure(
                    "Lỗi khi lấy mức đề nghị",
                    ResultType.Unexpected
                );
            }
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
                if (farm == null)
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
                    TopicCode = createDto.TopicCode?.Trim() ?? string.Empty,
                    CameraUrl = createDto.CameraUrl.Trim(),
                };

                await _unitOfWork.GetRepository<FishTank>().AddAsync(newFishTank);
                await _unitOfWork.SaveChangesAsync();

                // Tạo snapshot mới cho audit log
                var newSnapshot = new
                {
                    newFishTank.Name,
                    newFishTank.Height,
                    newFishTank.Radius,
                    FarmName = farm?.Name,
                    newFishTank.CameraUrl,
                    newFishTank.TopicCode,
                };
                await WriteAuditLogAsync(
                    AuditLogActions.Create,
                    newFishTank.Id.ToString(),
                    null,
                    newSnapshot,
                    "create-fish-tank"
                );

                var farmName = farm?.Name ?? "Unknown";

                var resultDto = new FishTankDto
                {
                    Id = newFishTank.Id,
                    Name = newFishTank.Name,
                    Height = newFishTank.Height,
                    Radius = newFishTank.Radius,
                    FarmId = newFishTank.FarmId,
                    FarmName = farmName,
                    TopicCode = newFishTank.TopicCode ?? string.Empty,
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

                if (fishTank == null)
                {
                    return Result.Failure("Bể cá không tồn tại.", ResultType.NotFound);
                }

                // Check for active batches
                var hasActiveBatches = await _unitOfWork
                    .GetRepository<FarmingBatch>()
                    .AnyAsync(b => b.FishTankId == id && b.Status == FarmingBatchStatus.ACTIVE);
                if (hasActiveBatches)
                {
                    return Result.Failure(
                        "Bể cá đang có vụ nuôi đang hoạt động. Vui lòng kết thúc hoặc thu hoạch trước khi xóa.",
                        ResultType.BadRequest
                    );
                }

                // Check for historical batches
                var hasHistoricalBatches = await _unitOfWork
                    .GetRepository<FarmingBatch>()
                    .AnyAsync(b => b.FishTankId == id && b.Status != FarmingBatchStatus.ACTIVE);
                if (hasHistoricalBatches)
                {
                    return Result.Failure(
                        "Bể cá có lịch sử vụ nuôi. Vui lòng xóa các vụ nuôi trước khi xóa bể.",
                        ResultType.BadRequest
                    );
                }

                _unitOfWork.GetRepository<FishTank>().Delete(fishTank);
                await _unitOfWork.SaveChangesAsync();

                // Tạo snapshot cho audit log
                var farm = await _unitOfWork.GetRepository<Farm>().GetByIdAsync(fishTank.FarmId);
                var oldSnapshot = new
                {
                    fishTank.Name,
                    fishTank.Height,
                    fishTank.Radius,
                    FarmName = farm?.Name ?? "Unknown",
                    fishTank.TopicCode,
                    fishTank.CameraUrl,
                };
                await WriteAuditLogAsync(
                    AuditLogActions.Delete,
                    fishTank.Id.ToString(),
                    oldSnapshot,
                    null,
                    "delete-fish-tank"
                );

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
            Result<List<TankSensorLatestDataDto>>
        > GetLatestFishTankMetricsByFarmAsync(Guid farmId)
        {
            var farm = await _unitOfWork.GetRepository<Farm>().GetByIdAsync(farmId);
            if (farm == null)
            {
                return Result<List<TankSensorLatestDataDto>>.Failure(
                    "Trang trại không tồn tại.",
                    ResultType.NotFound
                );
            }

            var result = (
                await _unitOfWork
                    .GetRepository<Sensor>()
                    .ListAsync(new TankSensorLatestDataByFarmSpec(farmId))
            ).ToList();

            return Result<List<TankSensorLatestDataDto>>.Success(
                result,
                result.Count == 0
                    ? "Trang trại chưa có cảm biến nào"
                    : $"Lấy dữ liệu mới nhất thành công: {result.Count} cảm biến"
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
                if (fishTank == null)
                    return Result<FishTankDto>.Failure("Bể cá không tồn tại.", ResultType.NotFound);

                // tao snapshot cũ cho audit log
                var originalFarm = await _unitOfWork
                    .GetRepository<Farm>()
                    .GetByIdAsync(fishTank.FarmId);
                var oldSnapshot = new
                {
                    fishTank.Name,
                    fishTank.Height,
                    fishTank.Radius,
                    FarmName = originalFarm?.Name ?? "Unknown",
                    fishTank.TopicCode,
                    fishTank.CameraUrl,
                };
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
                    if (farm == null)
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
                // Tạo snapshot cho audit log
                var newSnapshot = new
                {
                    FarmName = farmName,
                    fishTank.Name,
                    fishTank.Height,
                    fishTank.Radius,
                    fishTank.CameraUrl,
                    fishTank.TopicCode,
                };

                await WriteAuditLogAsync(
                    AuditLogActions.Update,
                    fishTank.Id.ToString(),
                    oldSnapshot,
                    newSnapshot,
                    "update-fish-tank"
                );

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

                var result = (
                    await _unitOfWork
                        .GetRepository<Sensor>()
                        .ListAsync(new TankSensorLatestDataSpec(tankId))
                ).ToList();

                // Overlay in-memory cache values for instant updates.
                // The database (SensorLog) may lag behind due to batch writing,
                // so use the in-memory cache as the source of truth for the latest value.
                foreach (var dto in result)
                {
                    var cached = _latestTelemetryCache.Get(dto.SensorId);
                    if (cached != null && dto.LatestData != null)
                    {
                        // Only override the latest average from the in-memory cache.
                        // Leave LatestMin/LatestMax to the database since they represent
                        // windowed aggregates, not instantaneous readings.
                        if (cached.Timestamp > dto.LatestData.RecordedAt)
                        {
                            dto.LatestData.LatestAvg = cached.Value;
                        }
                    }
                    else if (cached != null && dto.LatestData == null)
                    {
                        dto.LatestData = new TankSensorLatestDataValueDto
                        {
                            LatestAvg = cached.Value,
                            LatestMax = cached.Value,
                            LatestMin = cached.Value,
                            RecordedAt = cached.Timestamp,
                        };
                    }
                }

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

                var sensors = (
                    await _unitOfWork
                        .GetRepository<Sensor>()
                        .ListAsync(new TankSensorLatestDataSpec(tankId))
                ).ToList();

                var totalSensors = sensors.Count;
                var warningSensors = sensors.Count(s => s.LatestData?.HasWarning == true);

                var statusDto = new TankStatusDto
                {
                    TankId = tank.Id,
                    TankName = tank.Name,
                    Status = warningSensors > 0 ? TankStatus.Warning : TankStatus.Normal,
                    TotalSensors = totalSensors,
                    WarningSensors = warningSensors,
                };

                _logger.LogInformation(
                    "Lấy trạng thái bể thành công: {TankId} - {Status}",
                    tankId,
                    statusDto.Status
                );

                return Result<TankStatusDto>.Success(
                    statusDto,
                    $"Trạng thái bể: {statusDto.Status}"
                );
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
        #region Audit Log Helpers
        private async Task WriteAuditLogAsync(
            string action,
            string entityId,
            object? oldValue,
            object? newValue,
            string operation
        )
        {
            try
            {
                await _auditLogService.WriteSemanticAsync(
                    action,
                    AuditLogEntityType.FishTank,
                    entityId,
                    oldValue,
                    newValue
                );

                await _unitOfWork.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Failed to write {Operation} audit entry for {EntityType} {EntityId}",
                    operation,
                    AuditLogEntityType.FishTank,
                    entityId
                );
            }
        }
        #endregion
    }
}

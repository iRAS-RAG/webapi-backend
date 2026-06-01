using AutoMapper;
using IRasRag.Application.Common.Interfaces.Persistence;
using IRasRag.Application.Common.Models;
using IRasRag.Application.Common.Models.Pagination;
using IRasRag.Application.Common.Utils;
using IRasRag.Application.DTOs;
using IRasRag.Application.Services.Interfaces;
using IRasRag.Application.Specifications.AlertSpecifications;
using IRasRag.Domain.Entities;
using IRasRag.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace IRasRag.Application.Services.Implementations
{
    public class AlertService : IAlertService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<AlertService> _logger;
        private readonly IMapper _mapper;

        public AlertService(IUnitOfWork unitOfWork, ILogger<AlertService> logger, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
        }

        #region Get Methods
        public async Task<AlertPaginatedResult> GetAllAlertsAsync(AlertListRequest request)
        {
            try
            {
                var alertRepo = _unitOfWork.GetRepository<Alert>();

                var pagedResult = await alertRepo.GetPagedAsync(
                    new AlertDtoListSpec(request),
                    request.Page,
                    request.PageSize
                );

                var statuses = await alertRepo.ListAsync(new AlertStatusGlobalSummarySpec());
                var statusCounts = new AlertStatusCounts
                {
                    Open = statuses.Count(s => s == AlertStatus.OPEN),
                    Acknowledged = statuses.Count(s => s == AlertStatus.ACKNOWLEDGED),
                    Resolved = statuses.Count(s => s == AlertStatus.RESOLVED),
                    Dismissed = statuses.Count(s => s == AlertStatus.DISMISSED),
                };

                return new AlertPaginatedResult
                {
                    Message = pagedResult.TotalItems > 0
                        ? "Lấy danh sách cảnh báo thành công"
                        : "Không có dữ liệu",
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
                    StatusCounts = statusCounts,
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy danh sách cảnh báo");
                return new AlertPaginatedResult
                {
                    Message = "Đã xảy ra lỗi khi lấy danh sách cảnh báo",
                    Data = new List<AlertDto>(),
                    Meta = new PaginationMeta(),
                    Links = new PaginationLinks(),
                    StatusCounts = new AlertStatusCounts(),
                };
            }
        }

        public async Task<Result<AlertDto>> GetAlertByIdAsync(Guid id)
        {
            try
            {
                var alertDto = await _unitOfWork
                    .GetRepository<Alert>()
                    .FirstOrDefaultAsync(new AlertDtoByIdSpec(id));

                if (alertDto == null)
                    return Result<AlertDto>.Failure("Không tìm thấy cảnh báo", ResultType.NotFound);

                return Result<AlertDto>.Success(alertDto, "Lấy thông tin cảnh báo thành công");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy thông tin cảnh báo với Id: {AlertId}", id);
                return Result<AlertDto>.Failure(
                    "Đã xảy ra lỗi khi lấy thông tin cảnh báo",
                    ResultType.Unexpected
                );
            }
        }
        #endregion

        #region Create Methods
        public async Task<Result<AlertDto>> CreateAlertAsync(CreateAlertDto createDto)
        {
            try
            {
                // Validate SensorLog exists
                var sensorRepo = _unitOfWork.GetRepository<Sensor>();
                var sensorLogExists = await sensorRepo.AnyAsync(s => s.Id == createDto.SensorId);
                if (!sensorLogExists)
                {
                    return Result<AlertDto>.Failure("Sensor không tồn tại", ResultType.NotFound);
                }

                // Validate SpeciesThreshold exists
                var speciesThresholdRepo = _unitOfWork.GetRepository<SpeciesThreshold>();
                var speciesThresholdExists = await speciesThresholdRepo.AnyAsync(st =>
                    st.Id == createDto.SpeciesThresholdId
                );
                if (!speciesThresholdExists)
                {
                    return Result<AlertDto>.Failure(
                        "SpeciesThreshold không tồn tại",
                        ResultType.NotFound
                    );
                }

                // Validate FishTank exists
                var fishTankRepo = _unitOfWork.GetRepository<FishTank>();
                var fishTank = await fishTankRepo.GetByIdAsync(createDto.FishTankId);
                if (fishTank == null)
                {
                    return Result<AlertDto>.Failure("Bể cá không tồn tại", ResultType.NotFound);
                }

                // Validate SensorType exists
                var sensorTypeRepo = _unitOfWork.GetRepository<SensorType>();
                var sensorType = await sensorTypeRepo.GetByIdAsync(createDto.SensorTypeId);
                if (sensorType == null)
                {
                    return Result<AlertDto>.Failure(
                        "Loại cảm biến không tồn tại",
                        ResultType.NotFound
                    );
                }

                // Validate FarmingBatch if provided
                if (createDto.FarmingBatchId.HasValue)
                {
                    var farmingBatchRepo = _unitOfWork.GetRepository<FarmingBatch>();
                    var farmingBatchExists = await farmingBatchRepo.AnyAsync(fb =>
                        fb.Id == createDto.FarmingBatchId.Value
                    );
                    if (!farmingBatchExists)
                    {
                        return Result<AlertDto>.Failure(
                            "Lô nuôi không tồn tại",
                            ResultType.NotFound
                        );
                    }
                }

                var alert = _mapper.Map<Alert>(createDto);

                await _unitOfWork.GetRepository<Alert>().AddAsync(alert);
                await _unitOfWork.SaveChangesAsync();

                var alertDto = await _unitOfWork
                    .GetRepository<Alert>()
                    .FirstOrDefaultAsync(new AlertDtoByIdSpec(alert.Id));
                return Result<AlertDto>.Success(alertDto!, "Tạo cảnh báo thành công");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tạo cảnh báo");
                return Result<AlertDto>.Failure(
                    "Đã xảy ra lỗi khi tạo cảnh báo",
                    ResultType.Unexpected
                );
            }
        }
        #endregion

        #region Update Methods
        public async Task<Result> UpdateAlertAsync(Guid id, UpdateAlertDto updateDto)
        {
            try
            {
                var alertRepo = _unitOfWork.GetRepository<Alert>();
                var alert = await alertRepo.GetByIdAsync(id);

                if (alert == null)
                {
                    return Result.Failure("Không tìm thấy cảnh báo", ResultType.NotFound);
                }

                // Validate SensorLog if provided
                if (updateDto.SensorId.HasValue)
                {
                    var sensorRepo = _unitOfWork.GetRepository<Sensor>();
                    var sensorExists = await sensorRepo.AnyAsync(s =>
                        s.Id == updateDto.SensorId.Value
                    );
                    if (!sensorExists)
                    {
                        return Result.Failure("Sensor không tồn tại", ResultType.NotFound);
                    }
                }

                // Validate SpeciesThreshold if provided
                if (updateDto.SpeciesThresholdId.HasValue)
                {
                    var speciesThresholdRepo = _unitOfWork.GetRepository<SpeciesThreshold>();
                    var speciesThresholdExists = await speciesThresholdRepo.AnyAsync(st =>
                        st.Id == updateDto.SpeciesThresholdId.Value
                    );
                    if (!speciesThresholdExists)
                    {
                        return Result.Failure(
                            "SpeciesThreshold không tồn tại",
                            ResultType.NotFound
                        );
                    }
                }

                // Validate FishTank if provided
                if (updateDto.FishTankId.HasValue)
                {
                    var fishTankRepo = _unitOfWork.GetRepository<FishTank>();
                    var fishTankExists = await fishTankRepo.AnyAsync(ft =>
                        ft.Id == updateDto.FishTankId.Value
                    );
                    if (!fishTankExists)
                    {
                        return Result.Failure("Bể cá không tồn tại", ResultType.NotFound);
                    }
                }

                // Validate SensorType if provided
                if (updateDto.SensorTypeId.HasValue)
                {
                    var sensorTypeRepo = _unitOfWork.GetRepository<SensorType>();
                    var sensorTypeExists = await sensorTypeRepo.AnyAsync(st =>
                        st.Id == updateDto.SensorTypeId.Value
                    );
                    if (!sensorTypeExists)
                    {
                        return Result.Failure("Loại cảm biến không tồn tại", ResultType.NotFound);
                    }
                }

                // Validate FarmingBatch if provided
                if (updateDto.FarmingBatchId.HasValue)
                {
                    var farmingBatchRepo = _unitOfWork.GetRepository<FarmingBatch>();
                    var farmingBatchExists = await farmingBatchRepo.AnyAsync(fb =>
                        fb.Id == updateDto.FarmingBatchId.Value
                    );
                    if (!farmingBatchExists)
                    {
                        return Result.Failure("Lô nuôi không tồn tại", ResultType.NotFound);
                    }
                }

                _mapper.Map(updateDto, alert);
                alertRepo.Update(alert);
                await _unitOfWork.SaveChangesAsync();

                return Result.Success("Cập nhật cảnh báo thành công");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi cập nhật cảnh báo với Id: {AlertId}", id);
                return Result.Failure("Đã xảy ra lỗi khi cập nhật cảnh báo", ResultType.Unexpected);
            }
        }
        #endregion

        public async Task<Result> UpdateAlertStatusAsync(Guid id, AlertStatus newStatus)
        {
            try
            {
                var alert = await _unitOfWork.GetRepository<Alert>().GetByIdAsync(id);
                if (alert == null)
                    return Result.Failure("Không tìm thấy cảnh báo", ResultType.NotFound);

                var allowed = (alert.Status, newStatus) switch
                {
                    (AlertStatus.OPEN, AlertStatus.ACKNOWLEDGED) => true,
                    (AlertStatus.OPEN, AlertStatus.DISMISSED) => true,
                    (AlertStatus.ACKNOWLEDGED, AlertStatus.DISMISSED) => true,
                    _ => false,
                };

                if (!allowed)
                    return Result.Failure(
                        $"Không thể chuyển trạng thái từ {alert.Status} sang {newStatus}",
                        ResultType.BadRequest
                    );

                alert.Status = newStatus;
                _unitOfWork.GetRepository<Alert>().Update(alert);
                await _unitOfWork.SaveChangesAsync();

                return Result.Success("Cập nhật trạng thái cảnh báo thành công");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi cập nhật trạng thái cảnh báo với Id: {AlertId}", id);
                return Result.Failure(
                    "Đã xảy ra lỗi khi cập nhật trạng thái cảnh báo",
                    ResultType.Unexpected
                );
            }
        }

        #region Delete Methods
        public async Task<Result> DeleteAlertAsync(Guid id)
        {
            try
            {
                var alertRepo = _unitOfWork.GetRepository<Alert>();
                var alert = await alertRepo.GetByIdAsync(id);

                if (alert == null)
                {
                    return Result.Failure("Không tìm thấy cảnh báo", ResultType.NotFound);
                }

                // Check if there are any CorrectiveActions
                var correctiveActionRepo = _unitOfWork.GetRepository<CorrectiveAction>();
                var hasCorrectiveActions = await correctiveActionRepo.AnyAsync(ca =>
                    ca.AlertId == id
                );
                if (hasCorrectiveActions)
                {
                    return Result.Failure(
                        "Không thể xóa cảnh báo vì có Hành động khắc phục liên quan",
                        ResultType.BadRequest
                    );
                }

                // Check if there are any Recommendations
                var recommendationRepo = _unitOfWork.GetRepository<Recommendation>();
                var hasRecommendations = await recommendationRepo.AnyAsync(r => r.AlertId == id);
                if (hasRecommendations)
                {
                    return Result.Failure(
                        "Không thể xóa cảnh báo vì có Khuyến nghị liên quan",
                        ResultType.BadRequest
                    );
                }

                alertRepo.Delete(alert);
                await _unitOfWork.SaveChangesAsync();

                return Result.Success("Xóa cảnh báo thành công");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xóa cảnh báo với Id: {AlertId}", id);
                return Result.Failure("Đã xảy ra lỗi khi xóa cảnh báo", ResultType.Unexpected);
            }
        }
        #endregion
    }
}

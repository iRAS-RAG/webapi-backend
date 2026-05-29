using AutoMapper;
using IRasRag.Application.Common.Interfaces.Persistence;
using IRasRag.Application.Common.Models;
using IRasRag.Application.Common.Models.Pagination;
using IRasRag.Application.Common.Utils;
using IRasRag.Application.DTOs;
using IRasRag.Application.Services.Interfaces;
using IRasRag.Application.Specifications.FarmingBatchSpecifications;
using IRasRag.Application.Specifications.FeedingLogSpecifications;
using IRasRag.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace IRasRag.Application.Services.Implementations
{
    public class FeedingLogService : IFeedingLogService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<FeedingLogService> _logger;
        private readonly IMapper _mapper;
        private readonly IRasRag.Application.Services.Interfaces.IFarmingBatchService _farmingBatchService;
        private readonly IRasRag.Application.Common.Interfaces.Realtime.ISupervisorNotifier _supervisorNotifier;

        public FeedingLogService(
            IUnitOfWork unitOfWork,
            ILogger<FeedingLogService> logger,
            IMapper mapper,
            IRasRag.Application.Services.Interfaces.IFarmingBatchService farmingBatchService,
            IRasRag.Application.Common.Interfaces.Realtime.ISupervisorNotifier supervisorNotifier
        )
        {
            _unitOfWork = unitOfWork ?? throw new System.ArgumentNullException(nameof(unitOfWork));
            _logger = logger ?? throw new System.ArgumentNullException(nameof(logger));
            _mapper = mapper ?? throw new System.ArgumentNullException(nameof(mapper));
            _farmingBatchService =
                farmingBatchService
                ?? throw new System.ArgumentNullException(nameof(farmingBatchService));
            _supervisorNotifier = supervisorNotifier;
        }

        #region Get Methods
        public async Task<PaginatedResult<FeedingLogDto>> GetAllFeedingLogsAsync(
            FeedingLogListRequest request
        )
        {
            try
            {
                _logger.LogInformation(
                    "Bắt đầu lấy danh sách nhật ký cho ăn (Page: {Page}, PageSize: {PageSize})",
                    request.Page,
                    request.PageSize
                );

                var repository = _unitOfWork.GetRepository<FeedingLog>();
                var spec = new FeedingLogDtoListSpec(request);
                var pagedResult = await repository.GetPagedAsync(
                    spec,
                    request.Page,
                    request.PageSize
                );

                var feedingLogDtos = pagedResult.Items;

                _logger.LogInformation(
                    "Lấy danh sách nhật ký cho ăn thành công: {Count} bản ghi",
                    pagedResult.Items.Count
                );

                return new PaginatedResult<FeedingLogDto>
                {
                    Message =
                        feedingLogDtos.Count == 0
                            ? "Không có nhật ký cho ăn nào"
                            : "Lấy danh sách nhật ký cho ăn thành công",
                    Data = feedingLogDtos,
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
                _logger.LogError(ex, "Lỗi khi lấy danh sách nhật ký cho ăn");

                return new PaginatedResult<FeedingLogDto>
                {
                    Message = "Đã xảy ra lỗi khi lấy danh sách nhật ký cho ăn",
                    Data = Array.Empty<FeedingLogDto>(),
                    Meta = null,
                    Links = null,
                };
            }
        }

        public async Task<Result<FeedingLogDto>> GetFeedingLogByIdAsync(Guid id)
        {
            try
            {
                _logger.LogInformation("Bắt đầu lấy nhật ký cho ăn với Id: {Id}", id);

                if (id == Guid.Empty)
                {
                    _logger.LogWarning("Id nhật ký cho ăn không hợp lệ");
                    return Result<FeedingLogDto>.Failure(
                        "Id nhật ký cho ăn không hợp lệ",
                        ResultType.BadRequest
                    );
                }

                var feedingLogRepository = _unitOfWork.GetRepository<FeedingLog>();
                var feedingLogDto = await feedingLogRepository.FirstOrDefaultAsync(
                    new FeedingLogDtoByIdSpec(id)
                );

                if (feedingLogDto == null)
                {
                    _logger.LogWarning("Không tìm thấy nhật ký cho ăn với Id: {Id}", id);
                    return Result<FeedingLogDto>.Failure(
                        "Không tìm thấy nhật ký cho ăn",
                        ResultType.NotFound
                    );
                }

                _logger.LogInformation("Lấy nhật ký cho ăn thành công: {Id}", id);

                return Result<FeedingLogDto>.Success(
                    feedingLogDto,
                    "Lấy nhật ký cho ăn thành công"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy nhật ký cho ăn với Id: {Id}", id);
                return Result<FeedingLogDto>.Failure(
                    "Đã xảy ra lỗi khi lấy nhật ký cho ăn",
                    ResultType.Unexpected
                );
            }
        }
        #endregion

        #region Create Method
        public async Task<Result<FeedingLogDto>> CreateFeedingLogAsync(
            CreateFeedingLogDto createDto
        )
        {
            try
            {
                _logger.LogInformation(
                    "Bắt đầu tạo nhật ký cho ăn mới cho FarmingBatchId: {FarmingBatchId}",
                    createDto.FarmingBatchId
                );

                // Validate input
                if (createDto.FarmingBatchId == Guid.Empty)
                {
                    _logger.LogWarning("Mã lô nuôi không hợp lệ");
                    return Result<FeedingLogDto>.Failure(
                        "Mã lô nuôi không hợp lệ",
                        ResultType.BadRequest
                    );
                }

                if (createDto.FeedTypeId == Guid.Empty)
                {
                    _logger.LogWarning("Loại thức ăn không hợp lệ");
                    return Result<FeedingLogDto>.Failure(
                        "Loại thức ăn không hợp lệ",
                        ResultType.BadRequest
                    );
                }

                if (createDto.Amount <= 0)
                {
                    _logger.LogWarning("Lượng thức ăn phải lớn hơn 0");
                    return Result<FeedingLogDto>.Failure(
                        "Lượng thức ăn phải lớn hơn 0",
                        ResultType.BadRequest
                    );
                }

                // Verify valid FarmingBatch that user can access
                var farmingBatchRepository = _unitOfWork.GetRepository<FarmingBatch>();
                var farmingBatch = await farmingBatchRepository.FirstOrDefaultAsync(
                    new FarmingBatchByUserAccessSpec(createDto.FarmingBatchId, createDto.UserId)
                );

                if (farmingBatch == null)
                {
                    _logger.LogWarning(
                        "Không tìm thấy lô nuôi với Id: {FarmingBatchId} hoặc người dùng {UserId} không có quyền truy cập",
                        createDto.FarmingBatchId,
                        createDto.UserId
                    );
                    return Result<FeedingLogDto>.Failure(
                        "Không tìm thấy lô nuôi hoặc bạn không có quyền truy cập",
                        ResultType.NotFound
                    );
                }

                // Verify allowed FeedType for current stage of the FarmingBatch
                var speciesStageConfigRepository = _unitOfWork.GetRepository<SpeciesStageConfig>();
                var isFeedTypeAllowedForCurrentStage = await speciesStageConfigRepository.AnyAsync(
                    ssc =>
                        ssc.Id == farmingBatch.CurrentStageConfigId
                        && ssc.FeedTypes.Any(ft => ft.Id == createDto.FeedTypeId)
                );

                if (!isFeedTypeAllowedForCurrentStage)
                {
                    _logger.LogWarning(
                        "FeedTypeId {FeedTypeId} không thuộc danh sách cho phép của CurrentStageConfigId {CurrentStageConfigId}",
                        createDto.FeedTypeId,
                        farmingBatch.CurrentStageConfigId
                    );
                    return Result<FeedingLogDto>.Failure(
                        "Loại thức ăn không tồn tại/không hợp lệ với giai đoạn sinh trưởng hiện tại của lô nuôi",
                        ResultType.BadRequest
                    );
                }

                var feedingLogRepository = _unitOfWork.GetRepository<FeedingLog>();
                var feedingLog = _mapper.Map<FeedingLog>(createDto);

                await feedingLogRepository.AddAsync(feedingLog);
                await _unitOfWork.SaveChangesAsync();

                try
                {
                    await _farmingBatchService.ComputeAndPersistFcrAsync(feedingLog.FarmingBatchId);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(
                        ex,
                        "Không thể tính FCR sau khi tạo feed log cho FarmingBatchId {BatchId}",
                        feedingLog.FarmingBatchId
                    );
                }

                var feedingLogDto = await feedingLogRepository.FirstOrDefaultAsync(
                    new FeedingLogDtoByIdSpec(feedingLog.Id)
                );

                if (feedingLogDto == null)
                {
                    _logger.LogWarning(
                        "Không thể tải lại nhật ký cho ăn vừa tạo với Id: {Id}",
                        feedingLog.Id
                    );
                    return Result<FeedingLogDto>.Failure(
                        "Đã xảy ra lỗi khi tạo nhật ký cho ăn",
                        ResultType.Unexpected
                    );
                }

                _logger.LogInformation(
                    "Tạo nhật ký cho ăn thành công: {Id} - FarmingBatchId: {FarmingBatchId}, UserId: {UserId}, Amount: {Amount}",
                    feedingLog.Id,
                    feedingLog.FarmingBatchId,
                    feedingLog.UserId,
                    feedingLog.Amount
                );

                // Broadcast to supervisors for this farm
                try
                {
                    var farmId = farmingBatch.FishTank?.FarmId;
                    var payload = new
                    {
                        feedingLogDto.Id,
                        feedingLogDto.FarmingBatchId,
                        feedingLogDto.Amount,
                        feedingLogDto.CreatedDate,
                    };
                    if (farmId != null)
                        await _supervisorNotifier.NotifyFeedingLogAsync(farmId.Value, payload);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to broadcast feeding log");
                }

                return Result<FeedingLogDto>.Success(
                    feedingLogDto,
                    "Tạo nhật ký cho ăn thành công"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Lỗi khi tạo nhật ký cho ăn cho FarmingBatchId: {FarmingBatchId}",
                    createDto.FarmingBatchId
                );
                return Result<FeedingLogDto>.Failure(
                    "Đã xảy ra lỗi khi tạo nhật ký cho ăn",
                    ResultType.Unexpected
                );
            }
        }
        #endregion

        #region Update Method
        public async Task<Result> UpdateFeedingLogAsync(Guid id, UpdateFeedingLogDto updateDto)
        {
            try
            {
                _logger.LogInformation("Bắt đầu cập nhật nhật ký cho ăn: {Id}", id);

                if (id == Guid.Empty)
                {
                    _logger.LogWarning("Id nhật ký cho ăn không hợp lệ");
                    return Result.Failure("Id nhật ký cho ăn không hợp lệ", ResultType.BadRequest);
                }

                var feedingLogRepository = _unitOfWork.GetRepository<FeedingLog>();
                var feedingLog = await feedingLogRepository.GetByIdAsync(id);

                if (feedingLog == null)
                {
                    _logger.LogWarning("Không tìm thấy nhật ký cho ăn với Id: {Id}", id);
                    return Result.Failure("Không tìm thấy nhật ký cho ăn", ResultType.NotFound);
                }

                // Validate amount if being updated
                if (updateDto.Amount.HasValue && updateDto.Amount.Value <= 0)
                {
                    _logger.LogWarning("Lượng thức ăn phải lớn hơn 0");
                    return Result.Failure("Lượng thức ăn phải lớn hơn 0", ResultType.BadRequest);
                }

                // Verify new FarmingBatchId exists if being updated
                if (
                    updateDto.FarmingBatchId.HasValue
                    && updateDto.FarmingBatchId.Value != Guid.Empty
                )
                {
                    var farmingBatchRepository = _unitOfWork.GetRepository<FarmingBatch>();
                    var farmingBatchExists = await farmingBatchRepository.AnyAsync(fb =>
                        fb.Id == updateDto.FarmingBatchId.Value
                    );

                    if (!farmingBatchExists)
                    {
                        _logger.LogWarning(
                            "Không tìm thấy lô nuôi với Id: {FarmingBatchId}",
                            updateDto.FarmingBatchId.Value
                        );
                        return Result.Failure("Không tìm thấy lô nuôi", ResultType.NotFound);
                    }
                }

                if (updateDto.FeedTypeId.HasValue && updateDto.FeedTypeId.Value != Guid.Empty)
                {
                    var feedTypeRepository = _unitOfWork.GetRepository<FeedType>();
                    var feedTypeExists = await feedTypeRepository.AnyAsync(ft =>
                        ft.Id == updateDto.FeedTypeId.Value
                    );

                    if (!feedTypeExists)
                    {
                        _logger.LogWarning(
                            "Không tìm thấy loại thức ăn với Id: {FeedTypeId}",
                            updateDto.FeedTypeId.Value
                        );
                        return Result.Failure("Không tìm thấy loại thức ăn", ResultType.NotFound);
                    }
                }

                _mapper.Map(updateDto, feedingLog);

                feedingLogRepository.Update(feedingLog);
                await _unitOfWork.SaveChangesAsync();

                try
                {
                    await _farmingBatchService.ComputeAndPersistFcrAsync(feedingLog.FarmingBatchId);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Không thể tính FCR sau khi cập nhật feed log {Id}", id);
                }

                _logger.LogInformation("Cập nhật nhật ký cho ăn thành công: {Id}", id);
                return Result.Success("Cập nhật nhật ký cho ăn thành công");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi cập nhật nhật ký cho ăn: {Id}", id);
                return Result.Failure(
                    "Đã xảy ra lỗi khi cập nhật nhật ký cho ăn",
                    ResultType.Unexpected
                );
            }
        }
        #endregion

        #region Delete Method
        public async Task<Result> DeleteFeedingLogAsync(Guid id)
        {
            try
            {
                _logger.LogInformation("Bắt đầu xóa nhật ký cho ăn: {Id}", id);

                if (id == Guid.Empty)
                {
                    _logger.LogWarning("Id nhật ký cho ăn không hợp lệ");
                    return Result.Failure("Id nhật ký cho ăn không hợp lệ", ResultType.BadRequest);
                }

                var feedingLogRepository = _unitOfWork.GetRepository<FeedingLog>();
                var feedingLog = await feedingLogRepository.GetByIdAsync(id);

                if (feedingLog == null)
                {
                    _logger.LogWarning("Không tìm thấy nhật ký cho ăn với Id: {Id}", id);
                    return Result.Failure("Không tìm thấy nhật ký cho ăn", ResultType.NotFound);
                }

                feedingLogRepository.Delete(feedingLog);
                await _unitOfWork.SaveChangesAsync();

                try
                {
                    await _farmingBatchService.ComputeAndPersistFcrAsync(feedingLog.FarmingBatchId);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Không thể tính FCR sau khi xóa feed log {Id}", id);
                }

                _logger.LogInformation("Xóa nhật ký cho ăn thành công: {Id}", id);
                return Result.Success("Xóa nhật ký cho ăn thành công");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xóa nhật ký cho ăn: {Id}", id);
                return Result.Failure(
                    "Đã xảy ra lỗi khi xóa nhật ký cho ăn",
                    ResultType.Unexpected
                );
            }
        }
        #endregion
    }
}

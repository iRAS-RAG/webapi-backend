using AutoMapper;
using IRasRag.Application.Common.Interfaces.Persistence;
using IRasRag.Application.Common.Models;
using IRasRag.Application.DTOs;
using IRasRag.Application.Services.Interfaces;
using IRasRag.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace IRasRag.Application.Services.Implementations
{
    public class FeedingLogService : IFeedingLogService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<FeedingLogService> _logger;
        private readonly IMapper _mapper;

        public FeedingLogService(
            IUnitOfWork unitOfWork,
            ILogger<FeedingLogService> logger,
            IMapper mapper
        )
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
        }

        #region Get Methods
        public async Task<Result<IEnumerable<FeedingLogDto>>> GetAllFeedingLogsAsync()
        {
            try
            {
                _logger.LogInformation("Bắt đầu lấy danh sách nhật ký cho ăn");

                var feedingLogRepository = _unitOfWork.GetRepository<FeedingLog>();
                var feedingLogs = await feedingLogRepository.GetAllAsync();

                if (!feedingLogs.Any())
                {
                    _logger.LogInformation("Không tìm thấy nhật ký cho ăn nào");
                    return Result<IEnumerable<FeedingLogDto>>.Success(
                        new List<FeedingLogDto>(),
                        "Không có nhật ký cho ăn nào"
                    );
                }

                var feedingLogDtos = _mapper.Map<IEnumerable<FeedingLogDto>>(feedingLogs);
                _logger.LogInformation(
                    "Lấy danh sách nhật ký cho ăn thành công: {Count} bản ghi",
                    feedingLogs.Count()
                );

                return Result<IEnumerable<FeedingLogDto>>.Success(
                    feedingLogDtos,
                    "Lấy danh sách nhật ký cho ăn thành công"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy danh sách nhật ký cho ăn");
                return Result<IEnumerable<FeedingLogDto>>.Failure(
                    "Đã xảy ra lỗi khi lấy danh sách nhật ký cho ăn",
                    ResultType.Unexpected
                );
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
                var feedingLog = await feedingLogRepository.GetByIdAsync(id);

                if (feedingLog == null)
                {
                    _logger.LogWarning("Không tìm thấy nhật ký cho ăn với Id: {Id}", id);
                    return Result<FeedingLogDto>.Failure(
                        "Không tìm thấy nhật ký cho ăn",
                        ResultType.NotFound
                    );
                }

                var feedingLogDto = _mapper.Map<FeedingLogDto>(feedingLog);
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

                if (createDto.Amount <= 0)
                {
                    _logger.LogWarning("Lượng thức ăn phải lớn hơn 0");
                    return Result<FeedingLogDto>.Failure(
                        "Lượng thức ăn phải lớn hơn 0",
                        ResultType.BadRequest
                    );
                }

                // Verify FarmingBatchId exists
                var farmingBatchRepository = _unitOfWork.GetRepository<FarmingBatch>();
                var farmingBatchExists = await farmingBatchRepository.AnyAsync(fb =>
                    fb.Id == createDto.FarmingBatchId
                );

                if (!farmingBatchExists)
                {
                    _logger.LogWarning(
                        "Không tìm thấy lô nuôi với Id: {FarmingBatchId}",
                        createDto.FarmingBatchId
                    );
                    return Result<FeedingLogDto>.Failure(
                        "Không tìm thấy lô nuôi",
                        ResultType.NotFound
                    );
                }

                var feedingLogRepository = _unitOfWork.GetRepository<FeedingLog>();
                var feedingLog = _mapper.Map<FeedingLog>(createDto);

                await feedingLogRepository.AddAsync(feedingLog);
                await _unitOfWork.SaveChangesAsync();

                var feedingLogDto = _mapper.Map<FeedingLogDto>(feedingLog);
                _logger.LogInformation(
                    "Tạo nhật ký cho ăn thành công: {Id} - FarmingBatchId: {FarmingBatchId}, Amount: {Amount}",
                    feedingLog.Id,
                    feedingLog.FarmingBatchId,
                    feedingLog.Amount
                );

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

                _mapper.Map(updateDto, feedingLog);

                feedingLogRepository.Update(feedingLog);
                await _unitOfWork.SaveChangesAsync();

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

using AutoMapper;
using IRasRag.Application.Common.Interfaces.Persistence;
using IRasRag.Application.Common.Models;
using IRasRag.Application.DTOs;
using IRasRag.Application.Services.Interfaces;
using IRasRag.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace IRasRag.Application.Services.Implementations
{
    public class MortalityLogService : IMortalityLogService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<MortalityLogService> _logger;
        private readonly IMapper _mapper;

        public MortalityLogService(
            IUnitOfWork unitOfWork,
            ILogger<MortalityLogService> logger,
            IMapper mapper
        )
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
        }

        #region Get Methods
        public async Task<Result<IEnumerable<MortalityLogDto>>> GetAllMortalityLogsAsync()
        {
            try
            {
                _logger.LogInformation("Bắt đầu lấy danh sách nhật ký tử vong");

                var mortalityLogRepository = _unitOfWork.GetRepository<MortalityLog>();
                var mortalityLogs = await mortalityLogRepository.GetAllAsync();

                if (!mortalityLogs.Any())
                {
                    _logger.LogInformation("Không tìm thấy nhật ký tử vong nào");
                    return Result<IEnumerable<MortalityLogDto>>.Success(
                        new List<MortalityLogDto>(),
                        "Không có nhật ký tử vong nào"
                    );
                }

                var mortalityLogDtos = _mapper.Map<IEnumerable<MortalityLogDto>>(mortalityLogs);
                _logger.LogInformation(
                    "Lấy danh sách nhật ký tử vong thành công: {Count} bản ghi",
                    mortalityLogs.Count()
                );

                return Result<IEnumerable<MortalityLogDto>>.Success(
                    mortalityLogDtos,
                    "Lấy danh sách nhật ký tử vong thành công"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy danh sách nhật ký tử vong");
                return Result<IEnumerable<MortalityLogDto>>.Failure(
                    "Đã xảy ra lỗi khi lấy danh sách nhật ký tử vong",
                    ResultType.Unexpected
                );
            }
        }

        public async Task<Result<MortalityLogDto>> GetMortalityLogByIdAsync(Guid id)
        {
            try
            {
                _logger.LogInformation("Bắt đầu lấy nhật ký tử vong với Id: {Id}", id);

                if (id == Guid.Empty)
                {
                    _logger.LogWarning("Id nhật ký tử vong không hợp lệ");
                    return Result<MortalityLogDto>.Failure(
                        "Id nhật ký tử vong không hợp lệ",
                        ResultType.BadRequest
                    );
                }

                var mortalityLogRepository = _unitOfWork.GetRepository<MortalityLog>();
                var mortalityLog = await mortalityLogRepository.GetByIdAsync(id);

                if (mortalityLog == null)
                {
                    _logger.LogWarning("Không tìm thấy nhật ký tử vong với Id: {Id}", id);
                    return Result<MortalityLogDto>.Failure(
                        "Không tìm thấy nhật ký tử vong",
                        ResultType.NotFound
                    );
                }

                var mortalityLogDto = _mapper.Map<MortalityLogDto>(mortalityLog);
                _logger.LogInformation("Lấy nhật ký tử vong thành công: {Id}", id);

                return Result<MortalityLogDto>.Success(
                    mortalityLogDto,
                    "Lấy nhật ký tử vong thành công"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy nhật ký tử vong với Id: {Id}", id);
                return Result<MortalityLogDto>.Failure(
                    "Đã xảy ra lỗi khi lấy nhật ký tử vong",
                    ResultType.Unexpected
                );
            }
        }
        #endregion

        #region Create Method
        public async Task<Result<MortalityLogDto>> CreateMortalityLogAsync(
            CreateMortalityLogDto createDto
        )
        {
            try
            {
                _logger.LogInformation(
                    "Bắt đầu tạo nhật ký tử vong mới cho BatchId: {BatchId}",
                    createDto.BatchId
                );

                // Validate input
                if (createDto.BatchId == Guid.Empty)
                {
                    _logger.LogWarning("Mã lô nuôi không hợp lệ");
                    return Result<MortalityLogDto>.Failure(
                        "Mã lô nuôi không hợp lệ",
                        ResultType.BadRequest
                    );
                }

                if (createDto.Quantity <= 0)
                {
                    _logger.LogWarning("Số lượng phải lớn hơn 0");
                    return Result<MortalityLogDto>.Failure(
                        "Số lượng phải lớn hơn 0",
                        ResultType.BadRequest
                    );
                }

                // Verify BatchId exists
                var batchRepository = _unitOfWork.GetRepository<FarmingBatch>();
                var batchExists = await batchRepository.AnyAsync(b => b.Id == createDto.BatchId);

                if (!batchExists)
                {
                    _logger.LogWarning(
                        "Không tìm thấy lô nuôi với Id: {BatchId}",
                        createDto.BatchId
                    );
                    return Result<MortalityLogDto>.Failure(
                        "Không tìm thấy lô nuôi",
                        ResultType.NotFound
                    );
                }

                var mortalityLogRepository = _unitOfWork.GetRepository<MortalityLog>();
                var mortalityLog = _mapper.Map<MortalityLog>(createDto);

                await mortalityLogRepository.AddAsync(mortalityLog);
                await _unitOfWork.SaveChangesAsync();

                var mortalityLogDto = _mapper.Map<MortalityLogDto>(mortalityLog);
                _logger.LogInformation(
                    "Tạo nhật ký tử vong thành công: {Id} - BatchId: {BatchId}, Quantity: {Quantity}",
                    mortalityLog.Id,
                    mortalityLog.BatchId,
                    mortalityLog.Quantity
                );

                return Result<MortalityLogDto>.Success(
                    mortalityLogDto,
                    "Tạo nhật ký tử vong thành công"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Lỗi khi tạo nhật ký tử vong cho BatchId: {BatchId}",
                    createDto.BatchId
                );
                return Result<MortalityLogDto>.Failure(
                    "Đã xảy ra lỗi khi tạo nhật ký tử vong",
                    ResultType.Unexpected
                );
            }
        }
        #endregion

        #region Update Method
        public async Task<Result> UpdateMortalityLogAsync(Guid id, UpdateMortalityLogDto updateDto)
        {
            try
            {
                _logger.LogInformation("Bắt đầu cập nhật nhật ký tử vong: {Id}", id);

                if (id == Guid.Empty)
                {
                    _logger.LogWarning("Id nhật ký tử vong không hợp lệ");
                    return Result.Failure("Id nhật ký tử vong không hợp lệ", ResultType.BadRequest);
                }

                var mortalityLogRepository = _unitOfWork.GetRepository<MortalityLog>();
                var mortalityLog = await mortalityLogRepository.GetByIdAsync(id);

                if (mortalityLog == null)
                {
                    _logger.LogWarning("Không tìm thấy nhật ký tử vong với Id: {Id}", id);
                    return Result.Failure("Không tìm thấy nhật ký tử vong", ResultType.NotFound);
                }

                // Validate quantity if being updated
                if (updateDto.Quantity.HasValue && updateDto.Quantity.Value <= 0)
                {
                    _logger.LogWarning("Số lượng phải lớn hơn 0");
                    return Result.Failure("Số lượng phải lớn hơn 0", ResultType.BadRequest);
                }

                // Verify new BatchId exists if being updated
                if (updateDto.BatchId.HasValue && updateDto.BatchId.Value != Guid.Empty)
                {
                    var batchRepository = _unitOfWork.GetRepository<FarmingBatch>();
                    var batchExists = await batchRepository.AnyAsync(b =>
                        b.Id == updateDto.BatchId.Value
                    );

                    if (!batchExists)
                    {
                        _logger.LogWarning(
                            "Không tìm thấy lô nuôi với Id: {BatchId}",
                            updateDto.BatchId.Value
                        );
                        return Result.Failure("Không tìm thấy lô nuôi", ResultType.NotFound);
                    }
                }

                _mapper.Map(updateDto, mortalityLog);

                mortalityLogRepository.Update(mortalityLog);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Cập nhật nhật ký tử vong thành công: {Id}", id);
                return Result.Success("Cập nhật nhật ký tử vong thành công");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi cập nhật nhật ký tử vong: {Id}", id);
                return Result.Failure(
                    "Đã xảy ra lỗi khi cập nhật nhật ký tử vong",
                    ResultType.Unexpected
                );
            }
        }
        #endregion

        #region Delete Method
        public async Task<Result> DeleteMortalityLogAsync(Guid id)
        {
            try
            {
                _logger.LogInformation("Bắt đầu xóa nhật ký tử vong: {Id}", id);

                if (id == Guid.Empty)
                {
                    _logger.LogWarning("Id nhật ký tử vong không hợp lệ");
                    return Result.Failure("Id nhật ký tử vong không hợp lệ", ResultType.BadRequest);
                }

                var mortalityLogRepository = _unitOfWork.GetRepository<MortalityLog>();
                var mortalityLog = await mortalityLogRepository.GetByIdAsync(id);

                if (mortalityLog == null)
                {
                    _logger.LogWarning("Không tìm thấy nhật ký tử vong với Id: {Id}", id);
                    return Result.Failure("Không tìm thấy nhật ký tử vong", ResultType.NotFound);
                }

                mortalityLogRepository.Delete(mortalityLog);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Xóa nhật ký tử vong thành công: {Id}", id);
                return Result.Success("Xóa nhật ký tử vong thành công");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xóa nhật ký tử vong: {Id}", id);
                return Result.Failure(
                    "Đã xảy ra lỗi khi xóa nhật ký tử vong",
                    ResultType.Unexpected
                );
            }
        }
        #endregion
    }
}

using AutoMapper;
using IRasRag.Application.Common.Interfaces.Persistence;
using IRasRag.Application.Common.Models;
using IRasRag.Application.Common.Models.Pagination;
using IRasRag.Application.Common.Utils;
using IRasRag.Application.DTOs;
using IRasRag.Application.Services.Interfaces;
using IRasRag.Application.Specifications.MortalityLogSpecifications;
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
        public async Task<PaginatedResult<MortalityLogDto>> GetAllMortalityLogsAsync(
            MortalityLogListRequest request
        )
        {
            try
            {
                _logger.LogInformation(
                    "Bắt đầu lấy danh sách nhật ký tử vong (Page: {Page}, PageSize: {PageSize})",
                    request.Page,
                    request.PageSize
                );

                var repository = _unitOfWork.GetRepository<MortalityLog>();
                var spec = new MortalityLogDtoListSpec(request);
                var pagedResult = await repository.GetPagedAsync(
                    spec,
                    request.Page,
                    request.PageSize
                );

                var mortalityLogDtos = pagedResult.Items;

                _logger.LogInformation(
                    "Lấy danh sách nhật ký tử vong thành công: {Count} bản ghi",
                    pagedResult.Items.Count
                );

                return new PaginatedResult<MortalityLogDto>
                {
                    Message =
                        mortalityLogDtos.Count == 0
                            ? "Không có nhật ký tử vong nào"
                            : "Lấy danh sách nhật ký tử vong thành công",
                    Data = mortalityLogDtos,
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
                _logger.LogError(ex, "Lỗi khi lấy danh sách nhật ký tử vong");

                return new PaginatedResult<MortalityLogDto>
                {
                    Message = "Đã xảy ra lỗi khi lấy danh sách nhật ký tử vong",
                    Data = Array.Empty<MortalityLogDto>(),
                    Meta = null,
                    Links = null,
                };
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
                var mortalityLogDto = await mortalityLogRepository.FirstOrDefaultAsync(
                    new MortalityLogDtoByIdSpec(id)
                );

                if (mortalityLogDto == null)
                {
                    _logger.LogWarning("Không tìm thấy nhật ký tử vong với Id: {Id}", id);
                    return Result<MortalityLogDto>.Failure(
                        "Không tìm thấy nhật ký tử vong",
                        ResultType.NotFound
                    );
                }

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
            var batchRepository = _unitOfWork.GetRepository<FarmingBatch>();
            var userRepository = _unitOfWork.GetRepository<User>();
            var mortalityLogRepository = _unitOfWork.GetRepository<MortalityLog>();

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

                if (createDto.UserId == Guid.Empty)
                {
                    _logger.LogWarning("Người dùng không hợp lệ");
                    return Result<MortalityLogDto>.Failure(
                        "Người dùng không hợp lệ",
                        ResultType.BadRequest
                    );
                }

                await _unitOfWork.BeginTransactionAsync();

                var batch = await batchRepository.GetByIdAsync(createDto.BatchId);
                if (batch == null)
                {
                    _logger.LogWarning(
                        "Không tìm thấy lô nuôi với Id: {BatchId}",
                        createDto.BatchId
                    );
                    await _unitOfWork.RollbackTransactionAsync();
                    return Result<MortalityLogDto>.Failure(
                        "Không tìm thấy lô nuôi",
                        ResultType.NotFound
                    );
                }

                var userExists = await userRepository.AnyAsync(u => u.Id == createDto.UserId);

                if (!userExists)
                {
                    _logger.LogWarning(
                        "Không tìm thấy người dùng với Id: {UserId}",
                        createDto.UserId
                    );
                    await _unitOfWork.RollbackTransactionAsync();
                    return Result<MortalityLogDto>.Failure(
                        "Không tìm thấy người dùng",
                        ResultType.NotFound
                    );
                }

                if (createDto.Quantity > batch.CurrentQuantity)
                {
                    _logger.LogWarning(
                        "Số lượng hao hụt ({Quantity}) vượt quá số lượng hiện tại của lô ({CurrentQuantity})",
                        createDto.Quantity,
                        batch.CurrentQuantity
                    );

                    await _unitOfWork.RollbackTransactionAsync();

                    return Result<MortalityLogDto>.Failure(
                        "Số lượng hao hụt không được lớn hơn số lượng hiện tại của lô",
                        ResultType.BadRequest
                    );
                }

                var mortalityLog = _mapper.Map<MortalityLog>(createDto);

                await mortalityLogRepository.AddAsync(mortalityLog);

                batch.CurrentQuantity -= createDto.Quantity;
                batchRepository.Update(batch);

                await _unitOfWork.SaveChangesAsync();

                var mortalityLogDto = await mortalityLogRepository.FirstOrDefaultAsync(
                    new MortalityLogDtoByIdSpec(mortalityLog.Id)
                );

                if (mortalityLogDto == null)
                {
                    _logger.LogWarning(
                        "Không thể tải lại nhật ký tử vong vừa tạo với Id: {Id}",
                        mortalityLog.Id
                    );
                    await _unitOfWork.RollbackTransactionAsync();
                    return Result<MortalityLogDto>.Failure(
                        "Đã xảy ra lỗi khi tạo nhật ký tử vong",
                        ResultType.Unexpected
                    );
                }

                await _unitOfWork.CommitTransactionAsync();

                _logger.LogInformation(
                    "Tạo nhật ký tử vong thành công: {Id} - BatchId: {BatchId}, UserId: {UserId}, Quantity: {Quantity}",
                    mortalityLog.Id,
                    mortalityLog.BatchId,
                    mortalityLog.UserId,
                    mortalityLog.Quantity
                );

                return Result<MortalityLogDto>.Success(
                    mortalityLogDto,
                    "Tạo nhật ký tử vong thành công"
                );
            }
            catch (Exception ex)
            {
                try
                {
                    await _unitOfWork.RollbackTransactionAsync();
                }
                catch (Exception rollbackEx)
                {
                    _logger.LogWarning(
                        rollbackEx,
                        "Không thể rollback transaction khi tạo nhật ký tử vong cho BatchId: {BatchId}",
                        createDto.BatchId
                    );
                }

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

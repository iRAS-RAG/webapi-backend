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
    public class CorrectiveActionService : ICorrectiveActionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CorrectiveActionService> _logger;
        private readonly IMapper _mapper;

        public CorrectiveActionService(
            IUnitOfWork unitOfWork,
            ILogger<CorrectiveActionService> logger,
            IMapper mapper
        )
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
        }

        #region Get Methods
        public async Task<PaginatedResult<CorrectiveActionDto>> GetAllCorrectiveActionsAsync(
            int page,
            int pageSize
        )
        {
            try
            {
                _logger.LogInformation(
                    "Bắt đầu lấy danh sách hành động khắc phục (Page: {Page}, PageSize: {PageSize})",
                    page,
                    pageSize
                );

                var correctiveActionRepository = _unitOfWork.GetRepository<CorrectiveAction>();
                var spec = new CorrectiveActionDtoListSpec();
                var pagedResult = await correctiveActionRepository.GetPagedAsync(
                    spec,
                    page,
                    pageSize
                );

                _logger.LogInformation(
                    "Lấy danh sách hành động khắc phục thành công: {Count} hành động",
                    pagedResult.Items.Count
                );

                return new PaginatedResult<CorrectiveActionDto>
                {
                    Message =
                        pagedResult.Items.Count == 0
                            ? "Không có hành động khắc phục nào"
                            : "Lấy danh sách hành động khắc phục thành công",
                    Data = pagedResult.Items,
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
                _logger.LogError(ex, "Lỗi khi lấy danh sách hành động khắc phục");

                return new PaginatedResult<CorrectiveActionDto>
                {
                    Message = "Đã xảy ra lỗi khi lấy danh sách hành động khắc phục",
                    Data = Array.Empty<CorrectiveActionDto>(),
                    Meta = null,
                    Links = null,
                };
            }
        }

        public async Task<Result<CorrectiveActionDto>> GetCorrectiveActionByIdAsync(Guid id)
        {
            try
            {
                _logger.LogInformation("Bắt đầu lấy hành động khắc phục với Id: {Id}", id);

                var correctiveActionRepository = _unitOfWork.GetRepository<CorrectiveAction>();
                var correctiveAction = await correctiveActionRepository.GetByIdAsync(id);

                if (correctiveAction == null)
                {
                    _logger.LogWarning("Không tìm thấy hành động khắc phục với Id: {Id}", id);
                    return Result<CorrectiveActionDto>.Failure(
                        "Không tìm thấy hành động khắc phục",
                        ResultType.NotFound
                    );
                }

                var correctiveActionDto = _mapper.Map<CorrectiveActionDto>(correctiveAction);
                _logger.LogInformation("Lấy hành động khắc phục thành công với Id: {Id}", id);

                return Result<CorrectiveActionDto>.Success(
                    correctiveActionDto,
                    "Lấy thông tin hành động khắc phục thành công"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy hành động khắc phục với Id: {Id}", id);
                return Result<CorrectiveActionDto>.Failure(
                    "Đã xảy ra lỗi khi lấy thông tin hành động khắc phục",
                    ResultType.Unexpected
                );
            }
        }
        #endregion

        #region Create Method
        public async Task<Result<CorrectiveActionDto>> CreateCorrectiveActionAsync(
            CreateCorrectiveActionDto createDto
        )
        {
            try
            {
                _logger.LogInformation("Bắt đầu tạo hành động khắc phục mới");

                // Validate ActionTaken
                if (string.IsNullOrWhiteSpace(createDto.ActionTaken))
                {
                    _logger.LogWarning("Hành động khắc phục không được để trống");
                    return Result<CorrectiveActionDto>.Failure(
                        "Hành động khắc phục không được để trống",
                        ResultType.BadRequest
                    );
                }

                // Validate Alert exists
                var alertRepository = _unitOfWork.GetRepository<Alert>();
                var alert = await alertRepository.GetByIdAsync(createDto.AlertId);

                if (alert == null)
                {
                    _logger.LogWarning(
                        "Không tìm thấy cảnh báo với Id: {AlertId}",
                        createDto.AlertId
                    );
                    return Result<CorrectiveActionDto>.Failure(
                        "Không tìm thấy cảnh báo",
                        ResultType.NotFound
                    );
                }

                // Validate User exists
                var userRepository = _unitOfWork.GetRepository<User>();
                var user = await userRepository.GetByIdAsync(createDto.UserId);

                if (user == null)
                {
                    _logger.LogWarning(
                        "Không tìm thấy người dùng với Id: {UserId}",
                        createDto.UserId
                    );
                    return Result<CorrectiveActionDto>.Failure(
                        "Không tìm thấy người dùng",
                        ResultType.NotFound
                    );
                }

                // Create CorrectiveAction
                var correctiveAction = _mapper.Map<CorrectiveAction>(createDto);
                correctiveAction.ActionTaken = correctiveAction.ActionTaken.Trim();
                if (!string.IsNullOrWhiteSpace(correctiveAction.Notes))
                {
                    correctiveAction.Notes = correctiveAction.Notes.Trim();
                }

                var correctiveActionRepository = _unitOfWork.GetRepository<CorrectiveAction>();
                await correctiveActionRepository.AddAsync(correctiveAction);
                await _unitOfWork.SaveChangesAsync();

                var correctiveActionDto = _mapper.Map<CorrectiveActionDto>(correctiveAction);
                _logger.LogInformation(
                    "Tạo hành động khắc phục thành công với Id: {Id}",
                    correctiveAction.Id
                );

                return Result<CorrectiveActionDto>.Success(
                    correctiveActionDto,
                    "Tạo hành động khắc phục thành công"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tạo hành động khắc phục");
                return Result<CorrectiveActionDto>.Failure(
                    "Đã xảy ra lỗi khi tạo hành động khắc phục",
                    ResultType.Unexpected
                );
            }
        }
        #endregion

        #region Update Method
        public async Task<Result> UpdateCorrectiveActionAsync(
            Guid id,
            UpdateCorrectiveActionDto updateDto
        )
        {
            try
            {
                _logger.LogInformation("Bắt đầu cập nhật hành động khắc phục với Id: {Id}", id);

                var correctiveActionRepository = _unitOfWork.GetRepository<CorrectiveAction>();
                var correctiveAction = await correctiveActionRepository.GetByIdAsync(id);

                if (correctiveAction == null)
                {
                    _logger.LogWarning("Không tìm thấy hành động khắc phục với Id: {Id}", id);
                    return Result.Failure(
                        "Không tìm thấy hành động khắc phục",
                        ResultType.NotFound
                    );
                }

                // Update AlertId if provided
                if (updateDto.AlertId.HasValue)
                {
                    var alertRepository = _unitOfWork.GetRepository<Alert>();
                    var alert = await alertRepository.GetByIdAsync(updateDto.AlertId.Value);

                    if (alert == null)
                    {
                        _logger.LogWarning(
                            "Không tìm thấy cảnh báo với Id: {AlertId}",
                            updateDto.AlertId.Value
                        );
                        return Result.Failure("Không tìm thấy cảnh báo", ResultType.NotFound);
                    }

                    correctiveAction.AlertId = updateDto.AlertId.Value;
                }

                // Update UserId if provided
                if (updateDto.UserId.HasValue)
                {
                    var userRepository = _unitOfWork.GetRepository<User>();
                    var user = await userRepository.GetByIdAsync(updateDto.UserId.Value);

                    if (user == null)
                    {
                        _logger.LogWarning(
                            "Không tìm thấy người dùng với Id: {UserId}",
                            updateDto.UserId.Value
                        );
                        return Result.Failure("Không tìm thấy người dùng", ResultType.NotFound);
                    }

                    correctiveAction.UserId = updateDto.UserId.Value;
                }

                // Update ActionTaken if provided
                if (!string.IsNullOrWhiteSpace(updateDto.ActionTaken))
                {
                    correctiveAction.ActionTaken = updateDto.ActionTaken.Trim();
                }

                // Update Notes if provided
                if (updateDto.Notes != null)
                {
                    correctiveAction.Notes = string.IsNullOrWhiteSpace(updateDto.Notes)
                        ? string.Empty
                        : updateDto.Notes.Trim();
                }

                correctiveActionRepository.Update(correctiveAction);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Cập nhật hành động khắc phục thành công với Id: {Id}", id);
                return Result.Success("Cập nhật hành động khắc phục thành công");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi cập nhật hành động khắc phục với Id: {Id}", id);
                return Result.Failure(
                    "Đã xảy ra lỗi khi cập nhật hành động khắc phục",
                    ResultType.Unexpected
                );
            }
        }
        #endregion

        #region Delete Method
        public async Task<Result> DeleteCorrectiveActionAsync(Guid id)
        {
            try
            {
                _logger.LogInformation("Bắt đầu xóa hành động khắc phục với Id: {Id}", id);

                var correctiveActionRepository = _unitOfWork.GetRepository<CorrectiveAction>();
                var correctiveAction = await correctiveActionRepository.GetByIdAsync(id);

                if (correctiveAction == null)
                {
                    _logger.LogWarning("Không tìm thấy hành động khắc phục với Id: {Id}", id);
                    return Result.Failure(
                        "Không tìm thấy hành động khắc phục",
                        ResultType.NotFound
                    );
                }

                correctiveActionRepository.Delete(correctiveAction);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Xóa hành động khắc phục thành công với Id: {Id}", id);
                return Result.Success("Xóa hành động khắc phục thành công");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xóa hành động khắc phục với Id: {Id}", id);
                return Result.Failure(
                    "Đã xảy ra lỗi khi xóa hành động khắc phục",
                    ResultType.Unexpected
                );
            }
        }
        #endregion
    }
}

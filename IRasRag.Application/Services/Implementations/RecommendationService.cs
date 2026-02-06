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
    public class RecommendationService : IRecommendationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<RecommendationService> _logger;
        private readonly IMapper _mapper;

        public RecommendationService(
            IUnitOfWork unitOfWork,
            ILogger<RecommendationService> logger,
            IMapper mapper
        )
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
        }

        #region Get Methods
        public async Task<PaginatedResult<RecommendationDto>> GetAllRecommendationsAsync(
            int page,
            int pageSize
        )
        {
            try
            {
                _logger.LogInformation(
                    "Bắt đầu lấy danh sách khuyến nghị (Page: {Page}, PageSize: {PageSize})",
                    page,
                    pageSize
                );

                var recommendationRepository = _unitOfWork.GetRepository<Recommendation>();
                var spec = new RecommendationDtoListSpec();
                var pagedResult = await recommendationRepository.GetPagedAsync(
                    spec,
                    page,
                    pageSize
                );

                _logger.LogInformation(
                    "Lấy danh sách khuyến nghị thành công: {Count} khuyến nghị",
                    pagedResult.Items.Count
                );

                return new PaginatedResult<RecommendationDto>
                {
                    Message =
                        pagedResult.Items.Count == 0
                            ? "Không có khuyến nghị nào"
                            : "Lấy danh sách khuyến nghị thành công",
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
                _logger.LogError(ex, "Lỗi khi lấy danh sách khuyến nghị");

                return new PaginatedResult<RecommendationDto>
                {
                    Message = "Đã xảy ra lỗi khi lấy danh sách khuyến nghị",
                    Data = Array.Empty<RecommendationDto>(),
                    Meta = null,
                    Links = null,
                };
            }
        }

        public async Task<Result<RecommendationDto>> GetRecommendationByIdAsync(Guid id)
        {
            try
            {
                _logger.LogInformation("Bắt đầu lấy khuyến nghị với Id: {Id}", id);

                var recommendationRepository = _unitOfWork.GetRepository<Recommendation>();
                var recommendation = await recommendationRepository.GetByIdAsync(id);

                if (recommendation == null)
                {
                    _logger.LogWarning("Không tìm thấy khuyến nghị với Id: {Id}", id);
                    return Result<RecommendationDto>.Failure(
                        "Không tìm thấy khuyến nghị",
                        ResultType.NotFound
                    );
                }

                var recommendationDto = _mapper.Map<RecommendationDto>(recommendation);
                _logger.LogInformation("Lấy khuyến nghị thành công với Id: {Id}", id);

                return Result<RecommendationDto>.Success(
                    recommendationDto,
                    "Lấy thông tin khuyến nghị thành công"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy khuyến nghị với Id: {Id}", id);
                return Result<RecommendationDto>.Failure(
                    "Đã xảy ra lỗi khi lấy thông tin khuyến nghị",
                    ResultType.Unexpected
                );
            }
        }
        #endregion

        #region Create Method
        public async Task<Result<RecommendationDto>> CreateRecommendationAsync(
            CreateRecommendationDto createDto
        )
        {
            try
            {
                _logger.LogInformation("Bắt đầu tạo khuyến nghị mới");

                // Validate SuggestionText
                if (string.IsNullOrWhiteSpace(createDto.SuggestionText))
                {
                    _logger.LogWarning("Nội dung khuyến nghị không được để trống");
                    return Result<RecommendationDto>.Failure(
                        "Nội dung khuyến nghị không được để trống",
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
                    return Result<RecommendationDto>.Failure(
                        "Không tìm thấy cảnh báo",
                        ResultType.NotFound
                    );
                }

                // Validate Document exists
                var documentRepository = _unitOfWork.GetRepository<Document>();
                var document = await documentRepository.GetByIdAsync(createDto.DocumentId);

                if (document == null)
                {
                    _logger.LogWarning(
                        "Không tìm thấy tài liệu với Id: {DocumentId}",
                        createDto.DocumentId
                    );
                    return Result<RecommendationDto>.Failure(
                        "Không tìm thấy tài liệu",
                        ResultType.NotFound
                    );
                }

                // Create Recommendation
                var recommendation = _mapper.Map<Recommendation>(createDto);
                recommendation.SuggestionText = recommendation.SuggestionText.Trim();

                var recommendationRepository = _unitOfWork.GetRepository<Recommendation>();
                await recommendationRepository.AddAsync(recommendation);
                await _unitOfWork.SaveChangesAsync();

                var recommendationDto = _mapper.Map<RecommendationDto>(recommendation);
                _logger.LogInformation(
                    "Tạo khuyến nghị thành công với Id: {Id}",
                    recommendation.Id
                );

                return Result<RecommendationDto>.Success(
                    recommendationDto,
                    "Tạo khuyến nghị thành công"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tạo khuyến nghị");
                return Result<RecommendationDto>.Failure(
                    "Đã xảy ra lỗi khi tạo khuyến nghị",
                    ResultType.Unexpected
                );
            }
        }
        #endregion

        #region Update Method
        public async Task<Result> UpdateRecommendationAsync(
            Guid id,
            UpdateRecommendationDto updateDto
        )
        {
            try
            {
                _logger.LogInformation("Bắt đầu cập nhật khuyến nghị với Id: {Id}", id);

                var recommendationRepository = _unitOfWork.GetRepository<Recommendation>();
                var recommendation = await recommendationRepository.GetByIdAsync(id);

                if (recommendation == null)
                {
                    _logger.LogWarning("Không tìm thấy khuyến nghị với Id: {Id}", id);
                    return Result.Failure("Không tìm thấy khuyến nghị", ResultType.NotFound);
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

                    recommendation.AlertId = updateDto.AlertId.Value;
                }

                // Update DocumentId if provided
                if (updateDto.DocumentId.HasValue)
                {
                    var documentRepository = _unitOfWork.GetRepository<Document>();
                    var document = await documentRepository.GetByIdAsync(
                        updateDto.DocumentId.Value
                    );

                    if (document == null)
                    {
                        _logger.LogWarning(
                            "Không tìm thấy tài liệu với Id: {DocumentId}",
                            updateDto.DocumentId.Value
                        );
                        return Result.Failure("Không tìm thấy tài liệu", ResultType.NotFound);
                    }

                    recommendation.DocumentId = updateDto.DocumentId.Value;
                }

                // Update SuggestionText if provided
                if (!string.IsNullOrWhiteSpace(updateDto.SuggestionText))
                {
                    recommendation.SuggestionText = updateDto.SuggestionText.Trim();
                }

                recommendationRepository.Update(recommendation);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Cập nhật khuyến nghị thành công với Id: {Id}", id);
                return Result.Success("Cập nhật khuyến nghị thành công");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi cập nhật khuyến nghị với Id: {Id}", id);
                return Result.Failure(
                    "Đã xảy ra lỗi khi cập nhật khuyến nghị",
                    ResultType.Unexpected
                );
            }
        }
        #endregion

        #region Delete Method
        public async Task<Result> DeleteRecommendationAsync(Guid id)
        {
            try
            {
                _logger.LogInformation("Bắt đầu xóa khuyến nghị với Id: {Id}", id);

                var recommendationRepository = _unitOfWork.GetRepository<Recommendation>();
                var recommendation = await recommendationRepository.GetByIdAsync(id);

                if (recommendation == null)
                {
                    _logger.LogWarning("Không tìm thấy khuyến nghị với Id: {Id}", id);
                    return Result.Failure("Không tìm thấy khuyến nghị", ResultType.NotFound);
                }

                recommendationRepository.Delete(recommendation);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Xóa khuyến nghị thành công với Id: {Id}", id);
                return Result.Success("Xóa khuyến nghị thành công");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xóa khuyến nghị với Id: {Id}", id);
                return Result.Failure("Đã xảy ra lỗi khi xóa khuyến nghị", ResultType.Unexpected);
            }
        }
        #endregion
    }
}

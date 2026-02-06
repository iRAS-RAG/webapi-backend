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
    public class DocumentService : IDocumentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<DocumentService> _logger;
        private readonly IMapper _mapper;

        public DocumentService(
            IUnitOfWork unitOfWork,
            ILogger<DocumentService> logger,
            IMapper mapper
        )
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
        }

        #region Get Methods
        public async Task<PaginatedResult<DocumentDto>> GetAllDocumentsAsync(int page, int pageSize)
        {
            try
            {
                _logger.LogInformation(
                    "Bắt đầu lấy danh sách tài liệu (Page: {Page}, PageSize: {PageSize})",
                    page,
                    pageSize
                );

                var documentRepository = _unitOfWork.GetRepository<Document>();
                var spec = new DocumentDtoListSpec();
                var pagedResult = await documentRepository.GetPagedAsync(spec, page, pageSize);

                _logger.LogInformation(
                    "Lấy danh sách tài liệu thành công: {Count} tài liệu",
                    pagedResult.Items.Count
                );

                return new PaginatedResult<DocumentDto>
                {
                    Message =
                        pagedResult.Items.Count == 0
                            ? "Không có tài liệu nào"
                            : "Lấy danh sách tài liệu thành công",
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
                _logger.LogError(ex, "Lỗi khi lấy danh sách tài liệu");

                return new PaginatedResult<DocumentDto>
                {
                    Message = "Đã xảy ra lỗi khi lấy danh sách tài liệu",
                    Data = Array.Empty<DocumentDto>(),
                    Meta = null,
                    Links = null,
                };
            }
        }

        public async Task<Result<DocumentDto>> GetDocumentByIdAsync(Guid id)
        {
            try
            {
                _logger.LogInformation("Bắt đầu lấy tài liệu với Id: {Id}", id);

                var documentRepository = _unitOfWork.GetRepository<Document>();
                var document = await documentRepository.GetByIdAsync(id);

                if (document == null)
                {
                    _logger.LogWarning("Không tìm thấy tài liệu với Id: {Id}", id);
                    return Result<DocumentDto>.Failure(
                        "Không tìm thấy tài liệu",
                        ResultType.NotFound
                    );
                }

                var documentDto = _mapper.Map<DocumentDto>(document);
                _logger.LogInformation("Lấy tài liệu thành công với Id: {Id}", id);

                return Result<DocumentDto>.Success(
                    documentDto,
                    "Lấy thông tin tài liệu thành công"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy tài liệu với Id: {Id}", id);
                return Result<DocumentDto>.Failure(
                    "Đã xảy ra lỗi khi lấy thông tin tài liệu",
                    ResultType.Unexpected
                );
            }
        }
        #endregion

        #region Create Method
        public async Task<Result<DocumentDto>> CreateDocumentAsync(CreateDocumentDto createDto)
        {
            try
            {
                _logger.LogInformation("Bắt đầu tạo tài liệu mới: {Title}", createDto.Title);

                // Validate Title
                if (string.IsNullOrWhiteSpace(createDto.Title))
                {
                    _logger.LogWarning("Tiêu đề không được để trống");
                    return Result<DocumentDto>.Failure(
                        "Tiêu đề không được để trống",
                        ResultType.BadRequest
                    );
                }

                // Validate Content
                if (string.IsNullOrWhiteSpace(createDto.Content))
                {
                    _logger.LogWarning("Nội dung không được để trống");
                    return Result<DocumentDto>.Failure(
                        "Nội dung không được để trống",
                        ResultType.BadRequest
                    );
                }

                // Validate User exists
                var userRepository = _unitOfWork.GetRepository<User>();
                var user = await userRepository.GetByIdAsync(createDto.UploadedByUserId);

                if (user == null)
                {
                    _logger.LogWarning(
                        "Không tìm thấy người dùng với Id: {UserId}",
                        createDto.UploadedByUserId
                    );
                    return Result<DocumentDto>.Failure(
                        "Không tìm thấy người dùng",
                        ResultType.NotFound
                    );
                }

                // Create Document
                var document = _mapper.Map<Document>(createDto);
                document.Title = document.Title.Trim();
                document.Content = document.Content.Trim();

                var documentRepository = _unitOfWork.GetRepository<Document>();
                await documentRepository.AddAsync(document);
                await _unitOfWork.SaveChangesAsync();

                var documentDto = _mapper.Map<DocumentDto>(document);
                _logger.LogInformation("Tạo tài liệu thành công với Id: {Id}", document.Id);

                return Result<DocumentDto>.Success(documentDto, "Tạo tài liệu thành công");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tạo tài liệu");
                return Result<DocumentDto>.Failure(
                    "Đã xảy ra lỗi khi tạo tài liệu",
                    ResultType.Unexpected
                );
            }
        }
        #endregion

        #region Update Method
        public async Task<Result> UpdateDocumentAsync(Guid id, UpdateDocumentDto updateDto)
        {
            try
            {
                _logger.LogInformation("Bắt đầu cập nhật tài liệu với Id: {Id}", id);

                var documentRepository = _unitOfWork.GetRepository<Document>();
                var document = await documentRepository.GetByIdAsync(id);

                if (document == null)
                {
                    _logger.LogWarning("Không tìm thấy tài liệu với Id: {Id}", id);
                    return Result.Failure("Không tìm thấy tài liệu", ResultType.NotFound);
                }

                // Update Title if provided
                if (!string.IsNullOrWhiteSpace(updateDto.Title))
                {
                    document.Title = updateDto.Title.Trim();
                }

                // Update Content if provided
                if (!string.IsNullOrWhiteSpace(updateDto.Content))
                {
                    document.Content = updateDto.Content.Trim();
                }

                documentRepository.Update(document);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Cập nhật tài liệu thành công với Id: {Id}", id);
                return Result.Success("Cập nhật tài liệu thành công");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi cập nhật tài liệu với Id: {Id}", id);
                return Result.Failure("Đã xảy ra lỗi khi cập nhật tài liệu", ResultType.Unexpected);
            }
        }
        #endregion

        #region Delete Method
        public async Task<Result> DeleteDocumentAsync(Guid id)
        {
            try
            {
                _logger.LogInformation("Bắt đầu xóa tài liệu với Id: {Id}", id);

                var documentRepository = _unitOfWork.GetRepository<Document>();
                var document = await documentRepository.GetByIdAsync(id);

                if (document == null)
                {
                    _logger.LogWarning("Không tìm thấy tài liệu với Id: {Id}", id);
                    return Result.Failure("Không tìm thấy tài liệu", ResultType.NotFound);
                }

                // Check if document has recommendations
                var recommendationRepository = _unitOfWork.GetRepository<Recommendation>();
                var hasRecommendations = await recommendationRepository.AnyAsync(r =>
                    r.DocumentId == id
                );

                if (hasRecommendations)
                {
                    _logger.LogWarning(
                        "Không thể xóa tài liệu với Id: {Id} vì có khuyến nghị liên quan",
                        id
                    );
                    return Result.Failure(
                        "Không thể xóa tài liệu vì có khuyến nghị liên quan",
                        ResultType.Conflict
                    );
                }

                documentRepository.Delete(document);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Xóa tài liệu thành công với Id: {Id}", id);
                return Result.Success("Xóa tài liệu thành công");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xóa tài liệu với Id: {Id}", id);
                return Result.Failure("Đã xảy ra lỗi khi xóa tài liệu", ResultType.Unexpected);
            }
        }
        #endregion
    }
}

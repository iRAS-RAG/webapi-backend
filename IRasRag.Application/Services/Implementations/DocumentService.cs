using AutoMapper;
using IRasRag.Application.Common.Interfaces.BackgroundJobs;
using IRasRag.Application.Common.Interfaces.CloudFileStorage;
using IRasRag.Application.Common.Interfaces.FileExtraction;
using IRasRag.Application.Common.Interfaces.FileExtractor;
using IRasRag.Application.Common.Interfaces.FileValidator;
using IRasRag.Application.Common.Interfaces.Persistence;
using IRasRag.Application.Common.Models;
using IRasRag.Application.Common.Models.Pagination;
using IRasRag.Application.Common.Utils;
using IRasRag.Application.DTOs;
using IRasRag.Application.Services.Interfaces;
using IRasRag.Application.Specifications.DocumentSpecifications;
using IRasRag.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace IRasRag.Application.Services.Implementations
{
    public class DocumentService : IDocumentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<DocumentService> _logger;
        private readonly IMapper _mapper;
        private readonly IFileContentValidator _fileContentValidator;
        private readonly ICloudFileStorageService _cloudFileStorageService;

        //private readonly IFileTextExtractor _pdfTextExtractor;
        private readonly IFileTextExtractorResolver _fileTextExtractorResolver;
        private readonly IBackgroundJobService _backgroundJobService;

        public DocumentService(
            IUnitOfWork unitOfWork,
            ILogger<DocumentService> logger,
            IMapper mapper,
            IFileContentValidator fileContentValidator,
            ICloudFileStorageService cloudFileStorageService,
            IFileTextExtractorResolver fileTextExtractorResolver,
            IBackgroundJobService backgroundJobService
        )
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
            _fileContentValidator = fileContentValidator;
            _cloudFileStorageService = cloudFileStorageService;
            _fileTextExtractorResolver = fileTextExtractorResolver;
            _backgroundJobService = backgroundJobService;
        }

        #region Get Methods
        public async Task<PaginatedResult<DocumentDto>> GetAllDocumentsAsync(
            DocumentListRequest request
        )
        {
            try
            {
                var documentRepository = _unitOfWork.GetRepository<Document>();
                var spec = new DocumentDtoListSpec(request);
                var pagedResult = await documentRepository.GetPagedAsync(
                    spec,
                    request.Page,
                    request.PageSize
                );

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

        public async Task<Result<DocumentDetailDto>> GetDocumentByIdAsync(Guid id)
        {
            try
            {
                _logger.LogInformation("Bắt đầu lấy tài liệu với Id: {Id}", id);

                var document = await _unitOfWork
                    .GetRepository<Document>()
                    .FirstOrDefaultAsync(new DocumentDtoByIdSpec(id));

                if (document == null)
                {
                    _logger.LogWarning("Không tìm thấy tài liệu với Id: {Id}", id);
                    return Result<DocumentDetailDto>.Failure(
                        "Không tìm thấy tài liệu",
                        ResultType.NotFound
                    );
                }

                return Result<DocumentDetailDto>.Success(
                    document,
                    "Lấy thông tin tài liệu thành công"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy tài liệu với Id: {Id}", id);
                return Result<DocumentDetailDto>.Failure(
                    "Đã xảy ra lỗi khi lấy thông tin tài liệu",
                    ResultType.Unexpected
                );
            }
        }
        #endregion

        #region Create Method
        public async Task<Result> CreateDocumentAsync(CreateDocumentDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.FileTitle))
            {
                _logger.LogWarning("Tiêu đề không được để trống");
                return Result.Failure("Tiêu đề không được để trống", ResultType.BadRequest);
            }

            var exists = await _unitOfWork
                .GetRepository<Document>()
                .AnyAsync(d => d.Title == dto.FileTitle);
            if (exists)
                return Result.Failure("Tài liệu đã tồn tại", ResultType.Conflict);

            if (_fileContentValidator.HasValidSize(dto.FileSize) == false)
            {
                _logger.LogWarning(
                    "Kích thước tệp vượt quá giới hạn: {FileSize} bytes",
                    dto.FileSize
                );
                return Result.Failure("Kích thước tệp vượt quá giới hạn", ResultType.BadRequest);
            }

            var isUserExists = await _unitOfWork
                .GetRepository<User>()
                .AnyAsync(u => u.Id == dto.UploadedByUserId);
            if (!isUserExists)
            {
                _logger.LogWarning(
                    "Không tìm thấy người dùng với Id: {UserId}",
                    dto.UploadedByUserId
                );
                return Result.Failure("Không tìm thấy người dùng", ResultType.NotFound);
            }

            // Create a copy of the file stream to avoid issues with stream position during upload and text extraction
            using var buffer = new MemoryStream();
            await dto.FileStream.CopyToAsync(buffer);
            buffer.Position = 0;

            var fileExtension = _fileContentValidator.DetectExtension(buffer);
            if (fileExtension == null)
            {
                _logger.LogWarning("Định dạng tệp không hợp lệ: {FileName}", dto.FileName);
                return Result.Failure(
                    "Định dạng tệp không hợp lệ, hiện tại chỉ hỗ trợ PDF và Docx",
                    ResultType.BadRequest
                );
            }

            // Upload file to cloud storage and get the URL
            var fileUrl = string.Empty;
            try
            {
                buffer.Position = 0;
                fileUrl = await _cloudFileStorageService.UploadAsync(
                    buffer,
                    dto.FileName,
                    dto.FileSize
                );
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(
                    ex,
                    "Lỗi khi tải tệp lên dịch vụ lưu trữ đám mây: {Message}",
                    ex.Message
                );
                return Result.Failure(
                    "Đã xảy ra lỗi khi tải tệp lên dịch vụ lưu trữ đám mây",
                    ResultType.Unexpected
                );
            }

            var document = new Document
            {
                Title = dto.FileTitle,
                UploadedByUserId = dto.UploadedByUserId,
                UploadedAt = DateTime.UtcNow,
                FileUrl = fileUrl,
            };

            try
            {
                await _unitOfWork.GetRepository<Document>().AddAsync(document);
                await _unitOfWork.SaveChangesAsync();

                _backgroundJobService.Enqueue<IDocumentIngestJob>(s => s.RunAsync(document.Id));

                return Result.Success("Tạo tài liệu thành công");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi bắt đầu giao dịch tạo tài liệu");
                // delete file from cloud storage if database operation fails
                _backgroundJobService.Enqueue<ICloudFileStorageService>(service =>
                    service.DeleteAsync(fileUrl)
                );
                return Result.Failure("Đã xảy ra lỗi khi tạo tài liệu", ResultType.Unexpected);
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
                if (string.IsNullOrWhiteSpace(updateDto.Title))
                {
                    return Result.Failure("Tiêu đề không được để trống", ResultType.BadRequest);
                }

                var titleExists = await documentRepository.AnyAsync(d =>
                    d.Title == updateDto.Title.Trim() && d.Id != id
                );
                if (titleExists)
                {
                    return Result.Failure(
                        "Tài liệu với tiêu đề này đã tồn tại",
                        ResultType.Conflict
                    );
                }

                document.Title = updateDto.Title.Trim();

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
                _backgroundJobService.Enqueue<ICloudFileStorageService>(service =>
                    service.DeleteAsync(document.FileUrl)
                );

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

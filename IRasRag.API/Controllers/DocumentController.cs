using IRasRag.API.Utils;
using IRasRag.Application.Common.Models;
using IRasRag.Application.DTOs;
using IRasRag.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IRasRag.API.Controllers
{
    [ApiController]
    [Route("api/documents")]
    [Authorize]
    public class DocumentController : ControllerBase
    {
        private readonly IDocumentService _documentService;
        private readonly ILogger<DocumentController> _logger;
        private readonly HttpContextUtils _httpContextUtils;

        public DocumentController(
            IDocumentService documentService,
            ILogger<DocumentController> logger,
            HttpContextUtils httpContextUtils
        )
        {
            _documentService = documentService;
            _logger = logger;
            _httpContextUtils = httpContextUtils;
        }

        /// <summary>
        /// Lấy danh sách tất cả tài liệu
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAllDocuments([FromQuery] DocumentListRequest request)
        {
            try
            {
                if (request.Page <= 0 || request.PageSize <= 0)
                {
                    return BadRequest(
                        new { Message = "Số trang và kích thước trang phải lớn hơn 0." }
                    );
                }

                if (request.PageSize > 100)
                {
                    return BadRequest(new { Message = "Kích thước trang tối đa là 100." });
                }

                var result = await _documentService.GetAllDocumentsAsync(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy danh sách tài liệu");
                return StatusCode(500, new { Message = "Đã xảy ra lỗi khi xử lý yêu cầu" });
            }
        }

        /// <summary>
        /// Lấy thông tin tài liệu theo Id
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetDocumentById(Guid id)
        {
            try
            {
                var result = await _documentService.GetDocumentByIdAsync(id);

                if (!result.IsSuccess)
                {
                    return result.Type switch
                    {
                        ResultType.NotFound => NotFound(new { result.Message }),
                        _ => BadRequest(new { result.Message }),
                    };
                }

                return Ok(new { result.Message, result.Data });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy tài liệu với Id: {Id}", id);
                return StatusCode(500, new { Message = "Đã xảy ra lỗi khi xử lý yêu cầu" });
            }
        }

        /// <summary>
        /// Tạo tài liệu mới
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> CreateDocument(
            string? fileTitle,
            IFormFile file,
            CancellationToken ct
        )
        {
            if (file == null)
            {
                return BadRequest(new { Message = "File không được để trống" });
            }

            var userId = _httpContextUtils.GetUserId();
            if (userId == null)
            {
                return Unauthorized(new { Message = "Không xác thực được người dùng" });
            }

            var dto = new CreateDocumentDto
            {
                FileStream = file.OpenReadStream(),
                FileTitle = string.IsNullOrWhiteSpace(fileTitle)
                    ? Path.GetFileNameWithoutExtension(file.FileName)
                    : fileTitle,
                FileName = file.FileName,
                FileSize = file.Length,
                UploadedByUserId = userId.Value,
            };

            var result = await _documentService.CreateDocumentAsync(dto, ct);

            return result.Type switch
            {
                ResultType.NotFound => NotFound(new { result.Message }),
                ResultType.Conflict => Conflict(new { result.Message }),
                ResultType.BadRequest => BadRequest(new { result.Message }),
                ResultType.Unexpected => StatusCode(500, new { result.Message }),
                _ => Ok(new { result.Message }),
            };
        }

        /// <summary>
        /// Cập nhật tài liệu
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDocument(
            Guid id,
            [FromBody] UpdateDocumentDto updateDto
        )
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(
                        new { Message = "Dữ liệu không hợp lệ", Errors = ModelState }
                    );
                }

                var result = await _documentService.UpdateDocumentAsync(id, updateDto);

                if (!result.IsSuccess)
                {
                    return result.Type switch
                    {
                        ResultType.NotFound => NotFound(new { result.Message }),
                        ResultType.Conflict => Conflict(new { result.Message }),
                        ResultType.BadRequest => BadRequest(new { result.Message }),
                        _ => BadRequest(new { result.Message }),
                    };
                }

                return Ok(new { result.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi cập nhật tài liệu với Id: {Id}", id);
                return StatusCode(500, new { Message = "Đã xảy ra lỗi khi xử lý yêu cầu" });
            }
        }

        /// <summary>
        /// Đồng bộ lại tài liệu thất bại theo Id
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpPost("{id}/resync")]
        public async Task<IActionResult> ResyncDocument(Guid id)
        {
            var result = await _documentService.ResyncDocumentAsync(id);

            return result.Type switch
            {
                ResultType.NotFound => NotFound(new { result.Message }),
                ResultType.BadRequest => BadRequest(new { result.Message }),
                ResultType.Unexpected => StatusCode(500, new { result.Message }),
                _ => Ok(new { result.Message }),
            };
        }

        /// <summary>
        /// Xóa tài liệu
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDocument(Guid id)
        {
            var result = await _documentService.DeleteDocumentAsync(id);

            if (!result.IsSuccess)
            {
                return result.Type switch
                {
                    ResultType.NotFound => NotFound(new { result.Message }),
                    ResultType.Conflict => Conflict(new { result.Message }),
                    ResultType.Unexpected => StatusCode(500, new { result.Message }),
                    _ => BadRequest(new { result.Message }),
                };
            }

            return Ok(new { result.Message });
        }
    }
}

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
        [Authorize(Roles = "Supervisor")]
        public async Task<IActionResult> CreateDocument(
            [FromForm] IFormFile file,
            [FromForm] string title
        )
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { Message = "Dữ liệu không hợp lệ", Errors = ModelState });
            }

            if (string.IsNullOrWhiteSpace(title))
            {
                return BadRequest(new { Message = "Tiêu đề không được để trống" });
            }

            var userId = _httpContextUtils.GetUserId();
            if (userId == null)
            {
                return Unauthorized(new { Message = "Không xác thực được người dùng" });
            }

            var fileStream = file.OpenReadStream();
            var fileName = file.FileName;
            var fileSize = file.Length;
            var fileTitle = title.Trim();

            var dto = new CreateDocumentDto
            {
                FileStream = fileStream,
                FileName = fileName,
                FileSize = fileSize,
                FileTitle = fileTitle,
                UploadedByUserId = userId.Value,
            };
            var result = await _documentService.CreateDocumentAsync(dto);

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
        [HttpPut("{id}")]
        [Authorize(Roles = "Supervisor")]
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
        /// Xóa tài liệu
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Supervisor")]
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

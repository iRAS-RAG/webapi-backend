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

        public DocumentController(
            IDocumentService documentService,
            ILogger<DocumentController> logger
        )
        {
            _documentService = documentService;
            _logger = logger;
        }

        /// <summary>
        /// Lấy danh sách tất cả tài liệu
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAllDocuments(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10
        )
        {
            try
            {
                if (page <= 0 || pageSize <= 0)
                {
                    return BadRequest(
                        new { Message = "Số trang và kích thước trang phải lớn hơn 0." }
                    );
                }

                if (pageSize > 100)
                {
                    return BadRequest(new { Message = "Kích thước trang tối đa là 100." });
                }

                var result = await _documentService.GetAllDocumentsAsync(page, pageSize);
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
        public async Task<IActionResult> CreateDocument([FromBody] CreateDocumentDto createDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(
                        new { Message = "Dữ liệu không hợp lệ", Errors = ModelState }
                    );
                }

                var result = await _documentService.CreateDocumentAsync(createDto);

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

                return CreatedAtAction(
                    nameof(GetDocumentById),
                    new { id = result.Data!.Id },
                    new { result.Message, result.Data }
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tạo tài liệu");
                return StatusCode(500, new { Message = "Đã xảy ra lỗi khi xử lý yêu cầu" });
            }
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
            try
            {
                var result = await _documentService.DeleteDocumentAsync(id);

                if (!result.IsSuccess)
                {
                    return result.Type switch
                    {
                        ResultType.NotFound => NotFound(new { result.Message }),
                        ResultType.Conflict => Conflict(new { result.Message }),
                        _ => BadRequest(new { result.Message }),
                    };
                }

                return Ok(new { result.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xóa tài liệu với Id: {Id}", id);
                return StatusCode(500, new { Message = "Đã xảy ra lỗi khi xử lý yêu cầu" });
            }
        }
    }
}

using IRasRag.Application.Common.Models;
using IRasRag.Application.DTOs;
using IRasRag.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IRasRag.API.Controllers
{
    [ApiController]
    [Route("api/corrective-actions")]
    [Authorize]
    public class CorrectiveActionController : ControllerBase
    {
        private readonly ICorrectiveActionService _correctiveActionService;
        private readonly ILogger<CorrectiveActionController> _logger;

        public CorrectiveActionController(
            ICorrectiveActionService correctiveActionService,
            ILogger<CorrectiveActionController> logger
        )
        {
            _correctiveActionService = correctiveActionService;
            _logger = logger;
        }

        /// <summary>
        /// Lấy danh sách tất cả hành động khắc phục
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAllCorrectiveActions(
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

                var result = await _correctiveActionService.GetAllCorrectiveActionsAsync(
                    page,
                    pageSize
                );
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy danh sách hành động khắc phục");
                return StatusCode(500, new { Message = "Đã xảy ra lỗi khi xử lý yêu cầu" });
            }
        }

        /// <summary>
        /// Lấy thông tin hành động khắc phục theo Id
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCorrectiveActionById(Guid id)
        {
            try
            {
                var result = await _correctiveActionService.GetCorrectiveActionByIdAsync(id);

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
                _logger.LogError(ex, "Lỗi khi lấy hành động khắc phục với Id: {Id}", id);
                return StatusCode(500, new { Message = "Đã xảy ra lỗi khi xử lý yêu cầu" });
            }
        }

        /// <summary>
        /// Tạo hành động khắc phục mới
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Supervisor,Worker")]
        public async Task<IActionResult> CreateCorrectiveAction(
            [FromBody] CreateCorrectiveActionDto createDto
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

                var result = await _correctiveActionService.CreateCorrectiveActionAsync(createDto);

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
                    nameof(GetCorrectiveActionById),
                    new { id = result.Data!.Id },
                    new { result.Message, result.Data }
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tạo hành động khắc phục");
                return StatusCode(500, new { Message = "Đã xảy ra lỗi khi xử lý yêu cầu" });
            }
        }

        /// <summary>
        /// Cập nhật hành động khắc phục
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "Supervisor")]
        public async Task<IActionResult> UpdateCorrectiveAction(
            Guid id,
            [FromBody] UpdateCorrectiveActionDto updateDto
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

                var result = await _correctiveActionService.UpdateCorrectiveActionAsync(
                    id,
                    updateDto
                );

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
                _logger.LogError(ex, "Lỗi khi cập nhật hành động khắc phục với Id: {Id}", id);
                return StatusCode(500, new { Message = "Đã xảy ra lỗi khi xử lý yêu cầu" });
            }
        }

        /// <summary>
        /// Xóa hành động khắc phục
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Supervisor")]
        public async Task<IActionResult> DeleteCorrectiveAction(Guid id)
        {
            try
            {
                var result = await _correctiveActionService.DeleteCorrectiveActionAsync(id);

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
                _logger.LogError(ex, "Lỗi khi xóa hành động khắc phục với Id: {Id}", id);
                return StatusCode(500, new { Message = "Đã xảy ra lỗi khi xử lý yêu cầu" });
            }
        }
    }
}

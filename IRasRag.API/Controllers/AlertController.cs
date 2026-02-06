using IRasRag.Application.Common.Interfaces;
using IRasRag.Application.Common.Models;
using IRasRag.Application.DTOs;
using IRasRag.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IRasRag.API.Controllers
{
    [ApiController]
    [Route("api/alerts")]
    [Authorize]
    public class AlertController : ControllerBase
    {
        private readonly IAlertService _alertService;
        private readonly ILogger<AlertController> _logger;

        public AlertController(IAlertService alertService, ILogger<AlertController> logger)
        {
            _alertService = alertService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAlerts(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10
        )
        {
            try
            {
                if (page <= 0 || pageSize <= 0)
                {
                    return BadRequest(new { Message = "Page và PageSize phải lớn hơn 0" });
                }

                if (pageSize > 100)
                {
                    return BadRequest(new { Message = "PageSize không được vượt quá 100" });
                }

                var result = await _alertService.GetAllAlertsAsync(page, pageSize);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy danh sách cảnh báo");
                return StatusCode(
                    500,
                    new { Message = "Đã xảy ra lỗi khi lấy danh sách cảnh báo" }
                );
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAlertById(Guid id)
        {
            try
            {
                var result = await _alertService.GetAlertByIdAsync(id);
                return result.Type switch
                {
                    ResultType.Ok => Ok(new { result.Message, result.Data }),
                    ResultType.NotFound => NotFound(new { result.Message }),
                    _ => StatusCode(500, new { result.Message }),
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy thông tin cảnh báo với Id: {AlertId}", id);
                return StatusCode(
                    500,
                    new { Message = "Đã xảy ra lỗi khi lấy thông tin cảnh báo" }
                );
            }
        }

        [HttpPost]
        [Authorize(Roles = "Supervisor,Worker")]
        public async Task<IActionResult> CreateAlert([FromBody] CreateAlertDto createDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var result = await _alertService.CreateAlertAsync(createDto);
                return result.Type switch
                {
                    ResultType.Ok => CreatedAtAction(
                        nameof(GetAlertById),
                        new { id = result.Data!.Id },
                        new { result.Message, result.Data }
                    ),
                    ResultType.NotFound => NotFound(new { result.Message }),
                    ResultType.BadRequest => BadRequest(new { result.Message }),
                    _ => StatusCode(500, new { result.Message }),
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tạo cảnh báo");
                return StatusCode(500, new { Message = "Đã xảy ra lỗi khi tạo cảnh báo" });
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Supervisor")]
        public async Task<IActionResult> UpdateAlert(Guid id, [FromBody] UpdateAlertDto updateDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var result = await _alertService.UpdateAlertAsync(id, updateDto);
                return result.Type switch
                {
                    ResultType.Ok => Ok(new { result.Message }),
                    ResultType.NotFound => NotFound(new { result.Message }),
                    ResultType.BadRequest => BadRequest(new { result.Message }),
                    _ => StatusCode(500, new { result.Message }),
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi cập nhật cảnh báo với Id: {AlertId}", id);
                return StatusCode(500, new { Message = "Đã xảy ra lỗi khi cập nhật cảnh báo" });
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Supervisor")]
        public async Task<IActionResult> DeleteAlert(Guid id)
        {
            try
            {
                var result = await _alertService.DeleteAlertAsync(id);
                return result.Type switch
                {
                    ResultType.Ok => Ok(new { result.Message }),
                    ResultType.NotFound => NotFound(new { result.Message }),
                    ResultType.BadRequest => BadRequest(new { result.Message }),
                    _ => StatusCode(500, new { result.Message }),
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xóa cảnh báo với Id: {AlertId}", id);
                return StatusCode(500, new { Message = "Đã xảy ra lỗi khi xóa cảnh báo" });
            }
        }
    }
}

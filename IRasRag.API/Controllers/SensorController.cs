using IRasRag.Application.Common.Models;
using IRasRag.Application.DTOs;
using IRasRag.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IRasRag.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/sensors")]
    public class SensorController : ControllerBase
    {
        private readonly ISensorService _sensorService;
        private readonly ILogger<SensorController> _logger;

        public SensorController(ISensorService sensorService, ILogger<SensorController> logger)
        {
            _sensorService = sensorService;
            _logger = logger;
        }

        /// <summary>
        /// Nhập dữ liệu cảm biến thủ công
        /// </summary>
        [Authorize(Roles = "Supervisor")]
        [HttpPost("{id}/logs")]
        public async Task<IActionResult> CreateSensorLog(Guid id, [FromBody] CreateSensorLogDto dto)
        {
            try
            {
                var result = await _sensorService.CreateSensorLogAsync(id, dto);
                return result.Type switch
                {
                    ResultType.Ok => Ok(new { result.Message, result.Data }),
                    ResultType.NotFound => NotFound(new { result.Message }),
                    ResultType.BadRequest => BadRequest(new { result.Message }),
                    _ => StatusCode(500, new { result.Message }),
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Đã xảy ra lỗi khi tạo dữ liệu cảm biến cho sensor: {SensorId}",
                    id
                );
                return StatusCode(500, new { Message = "Có lỗi xảy ra, vui lòng thử lại sau." });
            }
        }

        /// <summary>
        /// Lấy lịch sử dữ liệu cảm biến cho biểu đồ (có phân trang)
        /// </summary>
        [HttpGet("{id}/logs")]
        public async Task<IActionResult> GetSensorLogs(
            Guid id,
            [FromQuery] SensorLogListRequest request
        )
        {
            try
            {
                var result = await _sensorService.GetSensorLogsAsync(id, request);
                return result.Type switch
                {
                    ResultType.Ok => Ok(result.Data),
                    ResultType.NotFound => NotFound(new { result.Message }),
                    ResultType.BadRequest => BadRequest(new { result.Message }),
                    _ => StatusCode(500, new { result.Message }),
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Đã xảy ra lỗi khi lấy dữ liệu cảm biến cho sensor: {SensorId}",
                    id
                );
                return StatusCode(500, new { Message = "Có lỗi xảy ra, vui lòng thử lại sau." });
            }
        }

        /// <summary>
        /// [LEGACY] Lấy lịch sử dữ liệu cảm biến — aggregation thực hiện trong bộ nhớ.
        /// Dùng để so sánh kết quả với GET /logs. Xóa sau khi xác nhận.
        /// </summary>
        [HttpGet("{id}/logs/legacy")]
        public async Task<IActionResult> GetSensorLogsLegacy(
            Guid id,
            [FromQuery] SensorLogListRequest request
        )
        {
            try
            {
                var result = await _sensorService.GetSensorLogsLegacyAsync(id, request);
                return result.Type switch
                {
                    ResultType.Ok => Ok(result.Data),
                    ResultType.NotFound => NotFound(new { result.Message }),
                    ResultType.BadRequest => BadRequest(new { result.Message }),
                    _ => StatusCode(500, new { result.Message }),
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "[Legacy] Đã xảy ra lỗi khi lấy dữ liệu cảm biến cho sensor: {SensorId}",
                    id
                );
                return StatusCode(500, new { Message = "Có lỗi xảy ra, vui lòng thử lại sau." });
            }
        }
    }
}

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
                _logger.LogError(ex, "An error occurred while creating sensor log for sensor: {SensorId}", id);
                return StatusCode(500, new { Message = "Có lỗi xảy ra, vui lòng thử lại sau." });
            }
        }

        /// <summary>
        /// Lấy lịch sử dữ liệu cảm biến cho biểu đồ
        /// </summary>
        [HttpGet("{id}/logs")]
        public async Task<IActionResult> GetSensorLogs(Guid id, [FromQuery] SensorLogListRequest request)
        {
            try
            {
                var result = await _sensorService.GetSensorLogsAsync(id, request);
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
                _logger.LogError(ex, "An error occurred while retrieving logs for sensor: {SensorId}", id);
                return StatusCode(500, new { Message = "Có lỗi xảy ra, vui lòng thử lại sau." });
            }
        }
    }
}

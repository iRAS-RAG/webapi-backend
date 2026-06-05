using IRasRag.API.Utils;
using IRasRag.Application.Common.Models;
using IRasRag.Application.DTOs;
using IRasRag.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IRasRag.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/mortality-logs")]
    public class MortalityLogController : ControllerBase
    {
        private readonly IMortalityLogService _mortalityLogService;
        private readonly ILogger<MortalityLogController> _logger;
        private readonly HttpContextUtils _httpContextUtils;

        public MortalityLogController(
            IMortalityLogService mortalityLogService,
            ILogger<MortalityLogController> logger,
            HttpContextUtils httpContextUtils
        )
        {
            _mortalityLogService = mortalityLogService;
            _logger = logger;
            _httpContextUtils = httpContextUtils;
        }

        [Authorize(Roles = "Supervisor,Operator")]
        [HttpPost]
        public async Task<IActionResult> CreateMortalityLog([FromBody] CreateMortalityLogDto dto)
        {
            try
            {
                var userId = _httpContextUtils.GetUserId();
                if (userId == null)
                {
                    return Unauthorized(new { Message = "Không xác thực được người dùng" });
                }

                dto.UserId = userId.Value;

                var result = await _mortalityLogService.CreateMortalityLogAsync(dto);

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
                    "An error occurred while creating mortality log for BatchId: {BatchId}",
                    dto.BatchId
                );
                return StatusCode(500, new { Message = "Có lỗi xảy ra, vui lòng thử lại sau." });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllMortalityLogs(
            [FromQuery] MortalityLogListRequest request
        )
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

                var result = await _mortalityLogService.GetAllMortalityLogsAsync(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving mortality logs.");
                return StatusCode(500, new { Message = "Có lỗi xảy ra, vui lòng thử lại sau." });
            }
        }

        [Authorize(Roles = "Supervisor,Operator")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetMortalityLogById(Guid id)
        {
            try
            {
                var result = await _mortalityLogService.GetMortalityLogByIdAsync(id);
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
                    "An error occurred while retrieving mortality log: {MortalityLogId}",
                    id
                );
                return StatusCode(500, new { Message = "Có lỗi xảy ra, vui lòng thử lại sau." });
            }
        }

        [Authorize(Roles = "Supervisor, Operator")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateMortalityLog(
            Guid id,
            [FromBody] UpdateMortalityLogDto dto
        )
        {
            try
            {
                var result = await _mortalityLogService.UpdateMortalityLogAsync(id, dto);

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
                _logger.LogError(
                    ex,
                    "An error occurred while updating mortality log: {MortalityLogId}",
                    id
                );
                return StatusCode(500, new { Message = "Có lỗi xảy ra, vui lòng thử lại sau." });
            }
        }

        [Authorize(Roles = "Supervisor, Operator")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMortalityLog(Guid id)
        {
            try
            {
                var result = await _mortalityLogService.DeleteMortalityLogAsync(id);

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
                _logger.LogError(
                    ex,
                    "An error occurred while deleting mortality log: {MortalityLogId}",
                    id
                );
                return StatusCode(500, new { Message = "Có lỗi xảy ra, vui lòng thử lại sau." });
            }
        }
    }
}

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
    [Route("api/feeding-logs")]
    public class FeedingLogController : ControllerBase
    {
        private readonly IFeedingLogService _feedingLogService;
        private readonly ILogger<FeedingLogController> _logger;
        private readonly HttpContextUtils _httpContextUtils;

        public FeedingLogController(
            IFeedingLogService feedingLogService,
            ILogger<FeedingLogController> logger,
            HttpContextUtils httpContextUtils
        )
        {
            _feedingLogService = feedingLogService;
            _logger = logger;
            _httpContextUtils = httpContextUtils;
        }

        [Authorize(Roles = "Supervisor,Operator")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetFeedingLogById(Guid id)
        {
            try
            {
                var result = await _feedingLogService.GetFeedingLogByIdAsync(id);
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
                    "An error occurred while retrieving feeding log: {FeedingLogId}",
                    id
                );
                return StatusCode(500, new { Message = "Có lỗi xảy ra, vui lòng thử lại sau." });
            }
        }

        [Authorize(Roles = "Supervisor, Operator")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateFeedingLog(
            Guid id,
            [FromBody] UpdateFeedingLogDto dto
        )
        {
            try
            {
                var result = await _feedingLogService.UpdateFeedingLogAsync(id, dto);

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
                    "An error occurred while updating feeding log: {FeedingLogId}",
                    id
                );
                return StatusCode(500, new { Message = "Có lỗi xảy ra, vui lòng thử lại sau." });
            }
        }

        [Authorize(Roles = "Supervisor, Operator")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFeedingLog(Guid id)
        {
            try
            {
                var result = await _feedingLogService.DeleteFeedingLogAsync(id);

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
                    "An error occurred while deleting feeding log: {FeedingLogId}",
                    id
                );
                return StatusCode(500, new { Message = "Có lỗi xảy ra, vui lòng thử lại sau." });
            }
        }
    }
}

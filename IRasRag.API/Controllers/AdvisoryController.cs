using IRasRag.API.Utils;
using IRasRag.Application.Common.Interfaces.Advisory;
using IRasRag.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IRasRag.API.Controllers
{
    [ApiController]
    [Route("api/advisory")]
    [Authorize]
    public class AdvisoryController : ControllerBase
    {
        private readonly IAdvisoryService _advisoryService;
        private readonly IUserTankAccessService _tankAccessService;
        private readonly HttpContextUtils _httpContextUtils;
        private readonly ILogger<AdvisoryController> _logger;

        public AdvisoryController(
            IAdvisoryService advisoryService,
            IUserTankAccessService tankAccessService,
            HttpContextUtils httpContextUtils,
            ILogger<AdvisoryController> logger
        )
        {
            _advisoryService = advisoryService;
            _tankAccessService = tankAccessService;
            _httpContextUtils = httpContextUtils;
            _logger = logger;
        }

        /// <summary>
        /// Gửi câu hỏi tư vấn cho hệ thống AI (F07)
        /// </summary>
        [HttpPost("chat")]
        public async Task<IActionResult> ChatAsync(
            [FromBody] AdvisoryChatRequest request,
            CancellationToken ct
        )
        {
            var userId = _httpContextUtils.GetUserId();
            if (userId == null)
                return Unauthorized();

            if (!await _tankAccessService.CanAccessTankAsync(userId.Value, request.TankId))
                return StatusCode(403, new { Message = "Bạn không có quyền truy cập bể nuôi này" });

            try
            {
                var result = await _advisoryService.ChatAsync(
                    request.TankId,
                    userId.Value,
                    request.Message,
                    ct
                );
                return Ok(
                    new
                    {
                        result.Answer,
                        result.IsOffTopic,
                        result.Citations,
                    }
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error processing advisory chat for user {UserId}, tank {TankId}",
                    userId,
                    request.TankId
                );
                return StatusCode(500, new { Message = "Đã xảy ra lỗi khi xử lý yêu cầu" });
            }
        }
    }

    public record AdvisoryChatRequest(Guid TankId, string Message);
}

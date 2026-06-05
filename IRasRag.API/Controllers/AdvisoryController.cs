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
                        result.Intent,
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

        /// <summary>
        /// Đánh giá câu trả lời AI là hữu ích hay không
        /// </summary>
        [HttpPost("chat/feedback")]
        public async Task<IActionResult> SubmitFeedbackAsync(
            [FromBody] AdvisoryFeedbackRequest request,
            CancellationToken ct
        )
        {
            var userId = _httpContextUtils.GetUserId();
            if (userId == null)
                return Unauthorized();

            if (!await _tankAccessService.CanAccessTankAsync(userId.Value, request.TankId))
                return StatusCode(403, new { Message = "Bạn không có quyền truy cập bể nuôi này" });

            var result = await _advisoryService.SubmitFeedbackAsync(
                userId.Value,
                request.Response,
                request.Helpful,
                request.Intent,
                request.Question,
                ct
            );

            if (result == null)
                return StatusCode(
                    502,
                    new { Message = "Không thể ghi nhận phản hồi. Vui lòng thử lại." }
                );

            var message = (result.Status, result.Saved, request.Helpful) switch
            {
                ("ok", true, _) => "Đã đánh dấu câu trả lời là hữu ích.",
                ("ok", false, _) => "Đã bỏ đánh dấu hữu ích.",
                (_, _, true) => "Câu trả lời này đã được đánh dấu là hữu ích trước đó.",
                (_, _, false) => "Câu trả lời này chưa được đánh dấu là hữu ích.",
            };

            return Ok(new { Message = message });
        }

        /// <summary>
        /// Chẩn đoán nguyên nhân cá chết — thu thập dữ liệu mortality, feeding, alerts
        /// và gửi đến AI RAG để phân tích nguyên nhân gốc rễ (F08)
        /// </summary>
        [HttpPost("diagnose-mortality")]
        public async Task<IActionResult> DiagnoseMortalityAsync(
            [FromBody] DiagnoseMortalityRequest request,
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
                var result = await _advisoryService.DiagnoseMortalityAsync(
                    request.TankId,
                    userId.Value,
                    request.BatchId,
                    request.TimeRange,
                    request.Message,
                    ct
                );

                return Ok(
                    new
                    {
                        result.Answer,
                        result.Intent,
                        result.Confidence,
                        result.Citations,
                        result.AnswerBasis,
                    }
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error processing mortality diagnosis for user {UserId}, tank {TankId}",
                    userId,
                    request.TankId
                );
                return StatusCode(
                    500,
                    new { Message = "Đã xảy ra lỗi khi chẩn đoán nguyên nhân cá chết" }
                );
            }
        }
    }

    public record AdvisoryChatRequest(Guid TankId, string Message);

    public record AdvisoryFeedbackRequest(
        Guid TankId,
        string Response,
        bool Helpful,
        string? Intent,
        string? Question
    );

    public record DiagnoseMortalityRequest(
        Guid TankId,
        Guid? BatchId = null,
        string? TimeRange = null,
        string? Message = null
    );
}

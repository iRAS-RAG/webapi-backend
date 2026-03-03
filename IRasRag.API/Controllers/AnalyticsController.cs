using IRasRag.API.Utils;
using IRasRag.Application.Common.Models;
using IRasRag.Application.DTOs;
using IRasRag.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IRasRag.API.Controllers
{
    [ApiController]
    [Route("api/analytics")]
    [Authorize]
    public class AnalyticsController : ControllerBase
    {
        private readonly IAnalyticsService _analyticsService;
        private readonly ILogger<AnalyticsController> _logger;
        private readonly HttpContextUtils _httpContextUtils;

        public AnalyticsController(
            IAnalyticsService analyticsService,
            ILogger<AnalyticsController> logger,
            HttpContextUtils httpContextUtils
        )
        {
            _analyticsService = analyticsService;
            _logger = logger;
            _httpContextUtils = httpContextUtils;
        }

        /// <summary>
        /// Thống kê tần suất cảnh báo theo loại cảm biến (F12 – Supervisor).
        /// </summary>
        /// <param name="request">
        /// from / to: khoảng thời gian (UTC). Mặc định: 30 ngày gần nhất. <br/>
        /// fishTankId / farmId: lọc theo bể hoặc trang trại (tuỳ chọn). <br/>
        /// topN: số loại cảnh báo trả về (mặc định: 10, tối đa: 50).
        /// </param>
        [HttpGet("alert-frequency")]
        [Authorize(Roles = "Supervisor")]
        public async Task<IActionResult> GetAlertFrequency(
            [FromQuery] AlertFrequencyRequest request
        )
        {
            var userId = _httpContextUtils.GetUserId();
            if (userId == null)
                return Unauthorized(new { Message = "Người dùng chưa được xác thực." });

            request.UserId = userId.Value;

            try
            {
                _logger.LogInformation(
                    "Alert frequency endpoint called. From={From}, To={To}, FishTankId={TankId}, FarmId={FarmId}, TopN={TopN}",
                    request.From,
                    request.To,
                    request.FishTankId,
                    request.FarmId,
                    request.TopN
                );

                var result = await _analyticsService.GetAlertFrequencyAsync(request);

                return result.Type switch
                {
                    ResultType.Ok => Ok(new { result.Message, result.Data }),
                    ResultType.BadRequest => BadRequest(new { result.Message }),
                    ResultType.NotFound => NotFound(new { result.Message }),
                    ResultType.Unauthorized => Unauthorized(new { result.Message }),
                    _ => StatusCode(500, new { result.Message }),
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "An unexpected error occurred in the alert-frequency endpoint."
                );
                return StatusCode(500, new { Message = "Có lỗi xảy ra, vui lòng thử lại sau." });
            }
        }

        /// <summary>
        /// So sánh các lô nuôi theo các chỉ số thống kê cơ bản (F11 – Supervisor).
        /// </summary>
        /// <param name="request">
        /// batchIds: danh sách ID lô nuôi cần so sánh (tối đa 10). <br/>
        /// metrics: danh sách chỉ số cần trả về (survival_rate | mortality | feeding | alerts | &lt;tên loại cảm biến&gt;).
        /// Bỏ trống để lấy tất cả các chỉ số.
        /// </param>
        [HttpGet("compare")]
        [Authorize(Roles = "Supervisor")]
        public async Task<IActionResult> CompareBatches([FromQuery] BatchCompareRequest request)
        {
            var userId = _httpContextUtils.GetUserId();
            if (userId == null)
                return Unauthorized(new { Message = "Người dùng chưa được xác thực." });

            request.UserId = userId.Value;

            try
            {
                _logger.LogInformation(
                    "Batch comparison endpoint called. BatchIds: [{Ids}], Metrics: [{Metrics}]",
                    string.Join(", ", request.BatchIds ?? new List<Guid>()),
                    string.Join(", ", request.Metrics ?? new List<string>())
                );

                var result = await _analyticsService.CompareBatchesAsync(request);

                return result.Type switch
                {
                    ResultType.Ok => Ok(new { result.Message, result.Data }),
                    ResultType.BadRequest => BadRequest(new { result.Message }),
                    ResultType.NotFound => NotFound(new { result.Message }),
                    ResultType.Unauthorized => Unauthorized(new { result.Message }),
                    _ => StatusCode(500, new { result.Message }),
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "An unexpected error occurred in the batch comparison endpoint."
                );
                return StatusCode(500, new { Message = "Có lỗi xảy ra, vui lòng thử lại sau." });
            }
        }
    }
}

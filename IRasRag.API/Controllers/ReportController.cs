using IRasRag.API.Utils;
using IRasRag.Application.Common.Models;
using IRasRag.Application.DTOs;
using IRasRag.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IRasRag.API.Controllers
{
    [ApiController]
    [Route("api/reports")]
    [Authorize]
    public class ReportController : ControllerBase
    {
        private readonly IReportService _reportService;
        private readonly ILogger<ReportController> _logger;
        private readonly HttpContextUtils _httpContextUtils;

        public ReportController(
            IReportService reportService,
            ILogger<ReportController> logger,
            HttpContextUtils httpContextUtils)
        {
            _reportService = reportService;
            _logger = logger;
            _httpContextUtils = httpContextUtils;
        }

        /// <summary>
        /// Lấy bản tóm tắt KPI cho dashboard (tỷ lệ sống sót, số lượng cảnh báo, v.v.)
        /// </summary>
        /// <param name="request">
        /// period: today | week | month | year (mặc định: month)
        /// </param>
        [HttpGet("dashboard")]
        public async Task<IActionResult> GetDashboardSummary(
            [FromQuery] DashboardQueryRequest request
        )
        {
            var userId = _httpContextUtils.GetUserId();
            if (userId == null)
                return Unauthorized(new { Message = "Người dùng chưa được xác thực." });

            request.UserId = userId.Value;

            try
            {
                _logger.LogInformation(
                    "Dashboard summary requested with period: {Period}",
                    request.Period
                );

                var result = await _reportService.GetDashboardSummaryAsync(request);

                return result.Type switch
                {
                    ResultType.Ok => Ok(new { result.Message, result.Data }),
                    ResultType.BadRequest => BadRequest(new { result.Message }),
                    _ => StatusCode(500, new { result.Message }),
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "An error occurred while fetching dashboard summary for period: {Period}",
                    request.Period
                );
                return StatusCode(500, new { Message = "Có lỗi xảy ra, vui lòng thử lại sau." });
            }
        }
        /// <summary>
        /// Lấy báo cáo tuần tự động dành cho Supervisor (F10).
        /// Tổng hợp cảnh báo, vấn đề chính, hành động khắc phục, khuyến nghị đã dùng và sức khỏe lô nuôi.
        /// </summary>
        /// <param name="request">
        /// period: current | last | số tuần lùi (mặc định: current)
        /// </param>
        [HttpGet("weekly")]
        public async Task<IActionResult> GetWeeklyReport(
            [FromQuery] WeeklyReportQueryRequest request
        )
        {
            var userId = _httpContextUtils.GetUserId();
            if (userId == null)
                return Unauthorized(new { Message = "Người dùng chưa được xác thực." });

            request.UserId = userId.Value;

            try
            {
                _logger.LogInformation(
                    "Weekly report requested with period: {Period}",
                    request.Period
                );

                var result = await _reportService.GetWeeklyReportAsync(request);

                return result.Type switch
                {
                    ResultType.Ok => Ok(new { result.Message, result.Data }),
                    ResultType.BadRequest => BadRequest(new { result.Message }),
                    _ => StatusCode(500, new { result.Message }),
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "An error occurred while generating weekly report for period: {Period}",
                    request.Period
                );
                return StatusCode(500, new { Message = "Có lỗi xảy ra, vui lòng thử lại sau." });
            }
        }
    }
}

using System.Globalization;
using IRasRag.Application.DTOs;
using IRasRag.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IRasRag.API.Controllers
{
    [Authorize(Roles = "Admin")]
    [ApiController]
    [Route("api/audit-logs")]
    public class AuditLogController : ControllerBase
    {
        private const int MaxPageSize = 100;

        private readonly IAuditLogService _auditLogService;
        private readonly ILogger<AuditLogController> _logger;

        public AuditLogController(
            IAuditLogService auditLogService,
            ILogger<AuditLogController> logger
        )
        {
            _auditLogService = auditLogService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAuditLogs([FromQuery] AuditLogQueryRequest request)
        {
            try
            {
                if (request.Page <= 0 || request.PageSize <= 0)
                {
                    return BadRequest(
                        new { Message = "Số trang và kích thước trang phải lớn hơn 0." }
                    );
                }

                if (request.PageSize > MaxPageSize)
                {
                    return BadRequest(new { Message = "Kích thước trang tối đa là 100." });
                }

                var result = await _auditLogService.GetPagedAsync(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving audit logs.");
                return StatusCode(500, new { Message = "Có lỗi xảy ra, vui lòng thử lại sau." });
            }
        }

        [HttpGet("export")]
        public async Task<IActionResult> ExportAuditLogs([FromQuery] AuditLogQueryRequest request)
        {
            try
            {
                var csvBytes = await _auditLogService.ExportCsvAsync(request);
                var fileName =
                    $"audit-logs-{DateTime.UtcNow.ToString("yyyyMMdd_HHmmss", CultureInfo.InvariantCulture)}.csv";
                return File(csvBytes, "text/csv; charset=utf-8", fileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while exporting audit logs.");
                return StatusCode(500, new { Message = "Có lỗi xảy ra, vui lòng thử lại sau." });
            }
        }
    }
}

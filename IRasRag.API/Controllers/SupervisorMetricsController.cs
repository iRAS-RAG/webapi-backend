using IRasRag.Application.Common.Models;
using IRasRag.Application.DTOs.Metrics;
using IRasRag.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IRasRag.API.Controllers
{
    [Authorize(Roles = "Supervisor")]
    [ApiController]
    [Route("api/supervisor/metrics")]
    public class SupervisorMetricsController : ControllerBase
    {
        private readonly ISupervisorMetricsService _metricsService;

        public SupervisorMetricsController(ISupervisorMetricsService metricsService)
        {
            _metricsService = metricsService;
        }

        [HttpGet("farm/{farmId}/summary")]
        public async Task<IActionResult> GetFarmSummary(
            Guid farmId,
            [FromQuery] DateTime? start,
            [FromQuery] DateTime? end,
            [FromQuery] string groupBy = "none"
        )
        {
            var result = await _metricsService.GetFarmSummaryAsync(farmId, start, end, groupBy);
            return result.Type == ResultType.Ok
                ? Ok(result.Data)
                : StatusCode(500, new { result.Message });
        }

        [HttpGet("farm/{farmId}/timeseries")]
        public async Task<IActionResult> GetFarmTimeSeries(
            Guid farmId,
            [FromQuery] DateTime? start,
            [FromQuery] DateTime? end,
            [FromQuery] string metric = "feed",
            [FromQuery] string interval = "day",
            [FromQuery] string groupBy = "none",
            [FromQuery] string aggregations = "sum"
        )
        {
            var aggs = aggregations.Split(
                ',',
                StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries
            );
            var result = await _metricsService.GetFarmTimeSeriesAsync(
                farmId,
                start,
                end,
                metric,
                interval,
                groupBy,
                aggs
            );
            return result.Type == ResultType.Ok
                ? Ok(result.Data)
                : StatusCode(500, new { result.Message });
        }

        [HttpGet("batch/{batchId}/history")]
        public async Task<IActionResult> GetBatchHistory(
            Guid batchId,
            [FromQuery] DateTime? start,
            [FromQuery] DateTime? end,
            [FromQuery] string metrics = "feed,mortality",
            [FromQuery] string interval = "day"
        )
        {
            var m = metrics.Split(
                ',',
                StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries
            );
            var result = await _metricsService.GetBatchHistoryAsync(
                batchId,
                start,
                end,
                m,
                interval
            );
            return result.Type == ResultType.Ok
                ? Ok(result.Data)
                : StatusCode(500, new { result.Message });
        }

        [HttpGet("farm/{farmId}/top-batches")]
        public async Task<IActionResult> GetTopBatches(
            Guid farmId,
            [FromQuery] DateTime? start,
            [FromQuery] DateTime? end,
            [FromQuery] string metric = "feed",
            [FromQuery] int limit = 10
        )
        {
            var result = await _metricsService.GetTopBatchesAsync(
                farmId,
                start,
                end,
                metric,
                limit
            );
            return result.Type == ResultType.Ok
                ? Ok(result.Data)
                : StatusCode(500, new { result.Message });
        }
    }
}

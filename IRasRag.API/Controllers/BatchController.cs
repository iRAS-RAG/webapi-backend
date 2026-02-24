using System.ComponentModel.DataAnnotations;
using IRasRag.Application.Common.Models;
using IRasRag.Application.DTOs;
using IRasRag.Application.Services.Interfaces;
using IRasRag.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IRasRag.API.Controllers
{
    [ApiController]
    [Route("api/batches")]
    [Authorize]
    public class BatchController : ControllerBase
    {
        private readonly IFarmingBatchService _farmingBatchService;
        private readonly IMortalityLogService _mortalityLogService;
        private readonly IFeedingLogService _feedingLogService;
        private readonly ILogger<BatchController> _logger;

        public BatchController(
            IFarmingBatchService farmingBatchService,
            IMortalityLogService mortalityLogService,
            IFeedingLogService feedingLogService,
            ILogger<BatchController> logger
        )
        {
            _farmingBatchService = farmingBatchService;
            _mortalityLogService = mortalityLogService;
            _feedingLogService = feedingLogService;
            _logger = logger;
        }

        // GET /api/batches?status=Active&fishTankId=...&page=1&pageSize=20
        [HttpGet]
        public async Task<IActionResult> GetAllBatches([FromQuery] FarmingBatchListRequest request)
        {
            try
            {
                if (request.Page <= 0 || request.PageSize <= 0)
                    return BadRequest(new { Message = "Page và PageSize phải lớn hơn 0." });

                if (request.PageSize > 100)
                    return BadRequest(new { Message = "PageSize không được vượt quá 100." });

                var result = await _farmingBatchService.GetAllFarmingBatchesAsync(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy danh sách lô nuôi");
                return StatusCode(
                    500,
                    new { Message = "Đã xảy ra lỗi khi lấy danh sách lô nuôi." }
                );
            }
        }

        // GET /api/batches/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetBatchById(Guid id)
        {
            try
            {
                var result = await _farmingBatchService.GetFarmingBatchByIdAsync(id);
                return result.Type switch
                {
                    ResultType.Ok => Ok(new { result.Message, result.Data }),
                    ResultType.NotFound => NotFound(new { result.Message }),
                    _ => StatusCode(500, new { result.Message }),
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy thông tin lô nuôi với ID {Id}", id);
                return StatusCode(
                    500,
                    new { Message = "Đã xảy ra lỗi khi lấy thông tin lô nuôi." }
                );
            }
        }

        // POST /api/batches
        [HttpPost]
        [Authorize(Roles = "Supervisor")]
        public async Task<IActionResult> CreateBatch([FromBody] CreateFarmingBatchDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var result = await _farmingBatchService.CreateFarmingBatchAsync(dto);
                return result.Type switch
                {
                    ResultType.Ok => CreatedAtAction(
                        nameof(GetBatchById),
                        new { id = result.Data?.Id },
                        new { result.Message, result.Data }
                    ),
                    ResultType.BadRequest => BadRequest(new { result.Message }),
                    ResultType.Conflict => Conflict(new { result.Message }),
                    _ => StatusCode(500, new { result.Message }),
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tạo lô nuôi");
                return StatusCode(500, new { Message = "Đã xảy ra lỗi khi tạo lô nuôi." });
            }
        }

        // PUT /api/batches/{id}
        [HttpPut("{id}")]
        [Authorize(Roles = "Supervisor")]
        public async Task<IActionResult> UpdateBatch(Guid id, [FromBody] UpdateFarmingBatchDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var result = await _farmingBatchService.UpdateFarmingBatchAsync(id, dto);
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
                _logger.LogError(ex, "Lỗi khi cập nhật lô nuôi với ID {Id}", id);
                return StatusCode(500, new { Message = "Đã xảy ra lỗi khi cập nhật lô nuôi." });
            }
        }

        // POST /api/batches/{id}/mortality  body: { quantity, date }
        [HttpPost("{id}/mortality")]
        [Authorize(Roles = "Supervisor")]
        public async Task<IActionResult> LogMortality(Guid id, [FromBody] LogMortalityRequest body)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var dto = new CreateMortalityLogDto
                {
                    BatchId = id,
                    Quantity = body.Quantity,
                    Date = body.Date,
                };

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
                _logger.LogError(ex, "Lỗi khi ghi nhận tỷ lệ chết cho lô nuôi {Id}", id);
                return StatusCode(500, new { Message = "Đã xảy ra lỗi khi ghi nhận tỷ lệ chết." });
            }
        }

        // GET /api/batches/{id}/feeding-logs?page=1&pageSize=20
        [HttpGet("{id}/feeding-logs")]
        public async Task<IActionResult> GetFeedingLogs(
            Guid id,
            [FromQuery] BaseFeedingLogQuery query
        )
        {
            try
            {
                if (query.Page <= 0 || query.PageSize <= 0)
                    return BadRequest(new { Message = "Page và PageSize phải lớn hơn 0." });

                if (query.PageSize > 100)
                    return BadRequest(new { Message = "PageSize không được vượt quá 100." });

                var request = new FeedingLogListRequest
                {
                    FarmingBatchId = id,
                    Page = query.Page,
                    PageSize = query.PageSize,
                    SortBy = query.SortBy,
                    SortDir = query.SortDir,
                };

                var result = await _feedingLogService.GetAllFeedingLogsAsync(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy lịch sử cho ăn cho lô nuôi {Id}", id);
                return StatusCode(500, new { Message = "Đã xảy ra lỗi khi lấy lịch sử cho ăn." });
            }
        }

        // POST /api/batches/{id}/feeding  body: { amount }
        [HttpPost("{id}/feeding")]
        [Authorize(Roles = "Supervisor")]
        public async Task<IActionResult> RecordFeeding(
            Guid id,
            [FromBody] RecordFeedingRequest body
        )
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var dto = new CreateFeedingLogDto
                {
                    FarmingBatchId = id,
                    Amount = body.Amount,
                    CreatedDate = DateTime.UtcNow,
                };

                var result = await _feedingLogService.CreateFeedingLogAsync(dto);
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
                _logger.LogError(ex, "Lỗi khi ghi nhận sự kiện cho ăn cho lô nuôi {Id}", id);
                return StatusCode(
                    500,
                    new { Message = "Đã xảy ra lỗi khi ghi nhận sự kiện cho ăn." }
                );
            }
        }

        // ── Inline request DTOs ─────────────────────────────────────────────

        public class LogMortalityRequest
        {
            [Required(ErrorMessage = "Số lượng là bắt buộc")]
            [Range(0.1, float.MaxValue, ErrorMessage = "Số lượng phải lớn hơn 0")]
            public float Quantity { get; set; }

            [Required(ErrorMessage = "Ngày ghi nhận là bắt buộc")]
            public DateTime Date { get; set; }
        }

        public class RecordFeedingRequest
        {
            [Required(ErrorMessage = "Lượng thức ăn là bắt buộc")]
            [Range(0.1, float.MaxValue, ErrorMessage = "Lượng thức ăn phải lớn hơn 0")]
            public float Amount { get; set; }
        }

        public class BaseFeedingLogQuery
        {
            public int Page { get; set; } = 1;
            public int PageSize { get; set; } = 20;
            public string? SortBy { get; set; }
            public string SortDir { get; set; } = "asc";
        }
    }
}

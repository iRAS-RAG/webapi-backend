using IRasRag.API.Utils;
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
    public class FarmingBatchController : ControllerBase
    {
        private readonly IFarmingBatchService _farmingBatchService;
        private readonly IMortalityLogService _mortalityLogService;
        private readonly IFeedingLogService _feedingLogService;
        private readonly ILogger<FarmingBatchController> _logger;
        private readonly HttpContextUtils _httpContextUtils;

        public FarmingBatchController(
            IFarmingBatchService farmingBatchService,
            IMortalityLogService mortalityLogService,
            IFeedingLogService feedingLogService,
            ILogger<FarmingBatchController> logger,
            HttpContextUtils httpContextUtils
        )
        {
            _farmingBatchService = farmingBatchService;
            _mortalityLogService = mortalityLogService;
            _feedingLogService = feedingLogService;
            _logger = logger;
            _httpContextUtils = httpContextUtils;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllFarmingBatches(
            [FromQuery] FarmingBatchListRequest request
        )
        {
            try
            {
                if (request.Page <= 0 || request.PageSize <= 0)
                {
                    return BadRequest(new { Message = "Page và PageSize phải lớn hơn 0" });
                }

                if (request.PageSize > 100)
                {
                    return BadRequest(new { Message = "PageSize không được vượt quá 100" });
                }
                var result = await _farmingBatchService.GetAllFarmingBatchesAsync(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy danh sách lô nuôi");
                return StatusCode(500, new { Message = "Đã xảy ra lỗi khi lấy danh sách lô nuôi" });
            }
        }

        [HttpGet("active")]
        public async Task<IActionResult> GetActiveFarmingBatches([FromQuery] Guid fishTankId)
        {
            try
            {
                var result = await _farmingBatchService.GetActiveFarmingBatchByFishTankIdAsync(
                    fishTankId
                );
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy danh sách lô nuôi đang hoạt động");
                return StatusCode(
                    500,
                    new { Message = "Đã xảy ra lỗi khi lấy danh sách lô nuôi đang hoạt động" }
                );
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetFarmingBatchById(Guid id)
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
                return StatusCode(500, new { Message = "Đã xảy ra lỗi khi lấy thông tin lô nuôi" });
            }
        }

        [HttpPost]
        [Authorize(Roles = "Supervisor")]
        public async Task<IActionResult> CreateFarmingBatch(
            [FromBody] CreateFarmingBatchDto createDto
        )
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var result = await _farmingBatchService.CreateFarmingBatchAsync(createDto);

                return result.Type switch
                {
                    ResultType.Ok => CreatedAtAction(
                        nameof(GetFarmingBatchById),
                        new { id = result.Data?.Id },
                        new { result.Message, result.Data }
                    ),
                    ResultType.BadRequest => BadRequest(new { result.Message }),
                    _ => StatusCode(500, new { result.Message }),
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tạo lô nuôi");
                return StatusCode(500, new { Message = "Đã xảy ra lỗi khi tạo lô nuôi" });
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Supervisor")]
        public async Task<IActionResult> UpdateFarmingBatch(
            Guid id,
            [FromBody] UpdateFarmingBatchDto updateDto
        )
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var result = await _farmingBatchService.UpdateFarmingBatchAsync(id, updateDto);

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
                return StatusCode(500, new { Message = "Đã xảy ra lỗi khi cập nhật lô nuôi" });
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Supervisor")]
        public async Task<IActionResult> DeleteFarmingBatch(Guid id)
        {
            try
            {
                var result = await _farmingBatchService.DeleteFarmingBatchAsync(id);

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
                _logger.LogError(ex, "Lỗi khi xóa lô nuôi với ID {Id}", id);
                return StatusCode(500, new { Message = "Đã xảy ra lỗi khi xóa lô nuôi" });
            }
        }

        [HttpPost("{id}/harvest")]
        [Authorize(Roles = "Supervisor")]
        public async Task<IActionResult> HarvestBatch(Guid id, DateTime harvestDate)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var result = await _farmingBatchService.HarvestBatchAsync(id, harvestDate);

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
                _logger.LogError(ex, "Lỗi khi thu hoạch lô nuôi với ID {Id}", id);
                return StatusCode(500, new { Message = "Đã xảy ra lỗi khi thu hoạch lô nuôi" });
            }
        }

        [HttpPost("{id}/mortality")]
        [Authorize(Roles = "Supervisor")]
        public async Task<IActionResult> LogMortality(Guid id, [FromBody] LogMortalityRequest body)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var userId = _httpContextUtils.GetUserId();
                if (userId == null)
                {
                    return Unauthorized(new { Message = "Không xác thực được người dùng" });
                }

                var dto = new CreateMortalityLogDto
                {
                    BatchId = id,
                    UserId = userId.Value,
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

        [HttpGet("{id}/feeding-logs")]
        public async Task<IActionResult> GetFeedingLogs(
            Guid id,
            [FromQuery] FeedingLogListRequest request
        )
        {
            try
            {
                if (request.Page <= 0 || request.PageSize <= 0)
                    return BadRequest(new { Message = "Page và PageSize phải lớn hơn 0." });

                if (request.PageSize > 100)
                    return BadRequest(new { Message = "PageSize không được vượt quá 100." });

                request.FarmingBatchId = id;

                var result = await _feedingLogService.GetAllFeedingLogsAsync(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy lịch sử cho ăn cho lô nuôi {Id}", id);
                return StatusCode(500, new { Message = "Đã xảy ra lỗi khi lấy lịch sử cho ăn." });
            }
        }

        [HttpPost("{id}/feeding")]
        [Authorize(Roles = "Supervisor, Operator")]
        public async Task<IActionResult> RecordFeeding(Guid id, [FromBody] CreateFeedingLogDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var userId = _httpContextUtils.GetUserId();
                if (userId == null)
                {
                    return Unauthorized(new { Message = "Không xác thực được người dùng" });
                }
                dto.FarmingBatchId = id;
                dto.UserId = userId.Value;
                // Set current UTC timestamp if not explicitly provided
                if (dto.CreatedDate == default)
                {
                    dto.CreatedDate = DateTime.UtcNow;
                }

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
                _logger.LogError(
                    ex,
                    "Lỗi khi ghi nhận sự kiện cho ăn cho lô nuôi {Id}",
                    dto.FarmingBatchId
                );
                return StatusCode(
                    500,
                    new { Message = "Đã xảy ra lỗi khi ghi nhận sự kiện cho ăn." }
                );
            }
        }
    }
}

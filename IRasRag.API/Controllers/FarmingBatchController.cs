using IRasRag.Application.Common.Interfaces;
using IRasRag.Application.Common.Models;
using IRasRag.Application.DTOs;
using IRasRag.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IRasRag.API.Controllers
{
    [ApiController]
    [Route("api/farming-batches")]
    [Authorize]
    public class FarmingBatchController : ControllerBase
    {
        private readonly IFarmingBatchService _farmingBatchService;
        private readonly ILogger<FarmingBatchController> _logger;

        public FarmingBatchController(
            IFarmingBatchService farmingBatchService,
            ILogger<FarmingBatchController> logger
        )
        {
            _farmingBatchService = farmingBatchService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllFarmingBatches(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10
        )
        {
            try
            {
                if (page <= 0 || pageSize <= 0)
                {
                    return BadRequest(new { Message = "Page và PageSize phải lớn hơn 0" });
                }

                if (pageSize > 100)
                {
                    return BadRequest(new { Message = "PageSize không được vượt quá 100" });
                }

                var result = await _farmingBatchService.GetAllFarmingBatchesAsync(page, pageSize);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy danh sách lô nuôi");
                return StatusCode(500, new { Message = "Đã xảy ra lỗi khi lấy danh sách lô nuôi" });
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
    }
}

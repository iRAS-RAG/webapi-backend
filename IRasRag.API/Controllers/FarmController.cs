using IRasRag.Application.Common.Models;
using IRasRag.Application.DTOs;
using IRasRag.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IRasRag.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/farms")]
    public class FarmController : ControllerBase
    {
        private readonly IFarmService _farmService;
        private readonly IFishTankService _fishTankService;
        private readonly ILogger<FarmController> _logger;

        public FarmController(
            IFarmService farmService,
            IFishTankService fishTankService,
            ILogger<FarmController> logger
        )
        {
            _farmService = farmService;
            _fishTankService = fishTankService;
            _logger = logger;
        }

        [Authorize(Roles = "Supervisor")]
        [HttpPost]
        public async Task<IActionResult> CreateFarm([FromBody] CreateFarmDto dto)
        {
            try
            {
                var result = await _farmService.CreateFarmAsync(dto);

                return result.Type switch
                {
                    ResultType.Ok => Ok(new { result.Message, result.Data }),
                    ResultType.Conflict => Conflict(new { result.Message }),
                    ResultType.BadRequest => BadRequest(new { result.Message }),
                    _ => StatusCode(500, new { result.Message }),
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Đã xảy ra lỗi khi tạo trang trại: {FarmName}", dto.Name);
                return StatusCode(500, new { Message = "Có lỗi xảy ra, vui lòng thử lại sau." });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllFarms([FromQuery] FarmListRequest request)
        {
            try
            {
                if (request.Page <= 0 || request.PageSize <= 0)
                {
                    return BadRequest(
                        new { Message = "Số trang và kích thước trang phải lớn hơn 0." }
                    );
                }

                if (request.PageSize > 100)
                {
                    return BadRequest(new { Message = "Kích thước trang tối đa là 100." });
                }
                var result = await _farmService.GetAllFarmsAsync(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Đã xảy ra lỗi khi lấy danh sách trang trại.");
                return StatusCode(500, new { Message = "Có lỗi xảy ra, vui lòng thử lại sau." });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetFarmById(Guid id)
        {
            try
            {
                var result = await _farmService.GetFarmByIdAsync(id);
                return result.Type switch
                {
                    ResultType.Ok => Ok(new { result.Message, result.Data }),
                    ResultType.NotFound => NotFound(new { result.Message }),
                    _ => StatusCode(500, new { result.Message }),
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Đã xảy ra lỗi khi lấy thông tin trang trại với ID: {FarmId}",
                    id
                );
                return StatusCode(500, new { Message = "Có lỗi xảy ra, vui lòng thử lại sau." });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateFarm(Guid id, [FromBody] UpdateFarmDto dto)
        {
            try
            {
                var result = await _farmService.UpdateFarmAsync(id, dto);
                return result.Type switch
                {
                    ResultType.Ok => Ok(new { result.Message }),
                    ResultType.NotFound => NotFound(new { result.Message }),
                    ResultType.Conflict => Conflict(new { result.Message }),
                    ResultType.BadRequest => BadRequest(new { result.Message }),
                    _ => StatusCode(500, new { result.Message }),
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Đã xảy ra lỗi khi cập nhật trang trại với ID: {FarmId}", id);
                return StatusCode(500, new { Message = "Có lỗi xảy ra, vui lòng thử lại sau." });
            }
        }

        [HttpGet("{farmId}/tanks")]
        public async Task<IActionResult> GetTanksByFarmId(
            Guid farmId,
            [FromQuery] FishTankListRequest request
        )
        {
            try
            {
                if (request.Page <= 0 || request.PageSize <= 0)
                    return BadRequest(
                        new { Message = "Số trang và kích thước trang phải lớn hơn 0." }
                    );

                if (request.PageSize > 100)
                    return BadRequest(new { Message = "Kích thước trang tối đa là 100." });

                var farmResult = await _farmService.GetFarmByIdAsync(farmId);
                if (farmResult.Type == ResultType.NotFound)
                    return NotFound(new { farmResult.Message });

                request.FarmId = farmId;
                var result = await _fishTankService.GetAllFishTanksAsync(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Đã xảy ra lỗi khi lấy danh sách bể cá của trang trại: {FarmId}",
                    farmId
                );
                return StatusCode(500, new { Message = "Có lỗi xảy ra, vui lòng thử lại sau." });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFarm(Guid id)
        {
            try
            {
                var result = await _farmService.DeleteFarmAsync(id);
                return result.Type switch
                {
                    ResultType.Ok => Ok(new { result.Message }),
                    ResultType.NotFound => NotFound(new { result.Message }),
                    _ => StatusCode(500, new { result.Message }),
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Đã xảy ra lỗi khi xóa trang trại với ID: {FarmId}", id);
                return StatusCode(500, new { Message = "Có lỗi xảy ra, vui lòng thử lại sau." });
            }
        }
    }
}

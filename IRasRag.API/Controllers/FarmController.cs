using IRasRag.API.Utils;
using IRasRag.Application.Common.Models;
using IRasRag.Application.DTOs;
using IRasRag.Application.Services.Implementations;
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
        private readonly ISensorService _sensorService;
        private readonly IFishTankService _fishTankService;
        private readonly ILogger<FarmController> _logger;
        private readonly HttpContextUtils _httpContextUtils;

        public FarmController(
            IFarmService farmService,
            ISensorService sensorService,
            IFishTankService fishTankService,
            ILogger<FarmController> logger,
            HttpContextUtils httpContextUtils
        )
        {
            _farmService = farmService;
            _sensorService = sensorService;
            _fishTankService = fishTankService;
            _logger = logger;
            _httpContextUtils = httpContextUtils;
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
                request.UserId = _httpContextUtils.GetUserId();
                if (request.UserId == null)
                {
                    return Unauthorized(new { Message = "Người dùng chưa được xác thực." });
                }

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

        [HttpGet("{farmId}/tanks/status-summary")]
        public async Task<IActionResult> GetLatestFishTankMetricsByFarm(Guid farmId)
        {
            try
            {
                var result = await _fishTankService.GetLatestFishTankMetricsByFarmAsync(farmId);
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
                    "Đã xảy ra lỗi khi lấy thông tin bể cá cho nông trại với ID: {FarmId}",
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

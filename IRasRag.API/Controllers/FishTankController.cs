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
    [Route("api/tanks")]
    public class FishTankController : ControllerBase
    {
        private readonly IFishTankService _fishTankService;
        private readonly ILogger<FishTankController> _logger;
        private readonly IControlDeviceService _controlDeviceService;

        public FishTankController(
            IFishTankService fishTankService,
            ILogger<FishTankController> logger,
            IControlDeviceService controlDeviceService
        )
        {
            _fishTankService = fishTankService;
            _logger = logger;
            _controlDeviceService = controlDeviceService;
        }

        [Authorize(Roles = "Supervisor")]
        [HttpPost]
        public async Task<IActionResult> CreateFishTank([FromBody] CreateFishTankDto dto)
        {
            try
            {
                var result = await _fishTankService.CreateFishTankAsync(dto);

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
                _logger.LogError(ex, "Đã xảy ra lỗi khi tạo bể cá: {FishTankName}", dto.Name);
                return StatusCode(500, new { Message = "Có lỗi xảy ra, vui lòng thử lại sau." });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllFishTanks([FromQuery] FishTankListRequest request)
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

                var result = await _fishTankService.GetAllFishTanksAsync(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Đã xảy ra lỗi khi lấy danh sách bể cá.");
                return StatusCode(500, new { Message = "Có lỗi xảy ra, vui lòng thử lại sau." });
            }
        }

        [HttpGet("/{id}/control-devices")]
        public async Task<IActionResult> GetAllControlDevicesByTank(Guid id, [FromQuery] ControlDeviceListRequest request)
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
                var result = await _controlDeviceService.GetAllControlDevicesByTankAsync(id, request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving control devices.");
                return StatusCode(500, new { Message = "Có lỗi xảy ra, vui lòng thử lại sau." });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetFishTankById(Guid id)
        {
            try
            {
                var result = await _fishTankService.GetFishTankByIdAsync(id);
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
                    "Đã xảy ra lỗi khi lấy thông tin bể cá với ID: {FishTankId}",
                    id
                );
                return StatusCode(500, new { Message = "Có lỗi xảy ra, vui lòng thử lại sau." });
            }
        }

        [Authorize(Roles = "Supervisor")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateFishTank(Guid id, [FromBody] UpdateFishTankDto dto)
        {
            try
            {
                var result = await _fishTankService.UpdateFishTankAsync(id, dto);
                return result.Type switch
                {
                    ResultType.Ok => Ok(new { result.Message, result.Data }),
                    ResultType.NotFound => NotFound(new { result.Message }),
                    ResultType.Conflict => Conflict(new { result.Message }),
                    ResultType.BadRequest => BadRequest(new { result.Message }),
                    _ => StatusCode(500, new { result.Message }),
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Đã xảy ra lỗi khi cập nhật bể cá với ID: {FishTankId}", id);
                return StatusCode(500, new { Message = "Có lỗi xảy ra, vui lòng thử lại sau." });
            }
        }

        [HttpGet("{id}/cameras")]
        public async Task<IActionResult> GetCamerasByTankId(Guid id)
        {
            try
            {
                var result = await _fishTankService.GetFishTankByIdAsync(id);
                return result.Type switch
                {
                    ResultType.NotFound => NotFound(new { result.Message }),
                    ResultType.Ok => Ok(
                        new
                        {
                            Message = "Lấy thông tin camera thành công.",
                            Data = new[]
                            {
                                new
                                {
                                    TankId = result.Data!.Id,
                                    TankName = result.Data.Name,
                                    CameraUrl = result.Data.CameraUrl,
                                },
                            },
                        }
                    ),
                    _ => StatusCode(500, new { result.Message }),
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Đã xảy ra lỗi khi lấy thông tin camera của bể cá với ID: {TankId}",
                    id
                );
                return StatusCode(500, new { Message = "Có lỗi xảy ra, vui lòng thử lại sau." });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFishTank(Guid id)
        {
            try
            {
                var result = await _fishTankService.DeleteFishTankAsync(id);
                return result.Type switch
                {
                    ResultType.Ok => Ok(new { result.Message }),
                    ResultType.NotFound => NotFound(new { result.Message }),
                    _ => StatusCode(500, new { result.Message }),
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Đã xảy ra lỗi khi xóa bể cá với ID: {FishTankId}", id);
                return StatusCode(500, new { Message = "Có lỗi xảy ra, vui lòng thử lại sau." });
            }
        }
    }
}

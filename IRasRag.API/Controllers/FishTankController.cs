using IRasRag.Application.Common.Models;
using IRasRag.Application.DTOs;
using IRasRag.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IRasRag.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/fish-tanks")]
    public class FishTankController : ControllerBase
    {
        private readonly IFishTankService _fishTankService;
        private readonly ILogger<FishTankController> _logger;

        public FishTankController(
            IFishTankService fishTankService,
            ILogger<FishTankController> logger
        )
        {
            _fishTankService = fishTankService;
            _logger = logger;
        }

        [Authorize(Roles = "Supervisor")]
        [HttpGet("{id}/recommended-initials")]
        public async Task<IActionResult> GetRecommendedInitials(Guid id)
        {
            try
            {
                var result = await _fishTankService.GetRecommendedInitialsAsync(id);
                return result.Type switch
                {
                    ResultType.Ok => Ok(new { result.Message, result.Data }),
                    ResultType.NotFound => NotFound(new { result.Message }),
                    _ => StatusCode(500, new { result.Message }),
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy mức đề nghị cho bể: {TankId}", id);
                return StatusCode(500, new { Message = "Đã xảy ra lỗi khi lấy mức đề nghị" });
            }
        }

        [Authorize(Roles = "Admin")]
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

        [Authorize(Roles = "Admin")]
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

        /// <summary>
        /// Lấy trạng thái nhanh của bể (Normal/Warning) dựa trên ngưỡng cảm biến
        /// </summary>
        [HttpGet("{id}/status")]
        public async Task<IActionResult> GetTankStatus(Guid id)
        {
            try
            {
                var result = await _fishTankService.GetTankStatusAsync(id);
                return result.Type switch
                {
                    ResultType.Ok => Ok(new { result.Message, result.Data }),
                    ResultType.NotFound => NotFound(new { result.Message }),
                    _ => StatusCode(500, new { result.Message }),
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy trạng thái bể: {TankId}", id);
                return StatusCode(500, new { Message = "Có lỗi xảy ra, vui lòng thử lại sau." });
            }
        }

        /// <summary>
        /// Lấy dữ liệu cảm biến mới nhất của bể cá theo ID.
        /// Trả về danh sách các cảm biến cùng với giá trị đo gần nhất, loại cảm biến và trạng thái cảnh báo.
        /// </summary>
        /// <param name="id">ID của bể cá cần lấy dữ liệu cảm biến mới nhất.</param>
        /// <returns>
        /// 200 OK – Danh sách dữ liệu cảm biến mới nhất.<br/>
        /// 404 Not Found – Không tìm thấy bể cá với ID đã cho.<br/>
        /// 500 Internal Server Error – Lỗi hệ thống.
        /// </returns>
        [HttpGet("{id}/latest-data")]
        public async Task<IActionResult> GetTankLatestData(Guid id)
        {
            try
            {
                var result = await _fishTankService.GetTankLatestDataAsync(id);
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
                    "Đã xảy ra lỗi khi lấy dữ liệu cảm biến mới nhất của bể với ID: {TankId}",
                    id
                );
                return StatusCode(500, new { Message = "Có lỗi xảy ra, vui lòng thử lại sau." });
            }
        }

        /// <summary>
        /// Lấy thông tin URL camera của bể cá theo ID.
        /// </summary>
        /// <param name="id">ID của bể cá cần lấy thông tin camera.</param>
        /// <returns>
        /// 200 OK – Thông tin camera của bể cá bao gồm TankId, TankName và CameraUrl.<br/>
        /// 404 Not Found – Không tìm thấy bể cá với ID đã cho.<br/>
        /// 500 Internal Server Error – Lỗi hệ thống.
        /// </returns>
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

        [Authorize(Roles = "Admin")]
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

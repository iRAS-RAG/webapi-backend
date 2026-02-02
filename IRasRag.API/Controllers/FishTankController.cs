using IRasRag.Application.Common.Models;
using IRasRag.Application.DTOs;
using IRasRag.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace IRasRag.API.Controllers
{
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
        public async Task<IActionResult> GetAllFishTanks()
        {
            try
            {
                var result = await _fishTankService.GetAllFishTanksAsync();
                return result.Type switch
                {
                    ResultType.Ok => Ok(new { result.Message, result.Data }),
                    _ => StatusCode(500, new { result.Message }),
                };
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

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateFishTank(Guid id, [FromBody] UpdateFishTankDto dto)
        {
            try
            {
                var result = await _fishTankService.UpdateFishTankAsync(id, dto);
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
                _logger.LogError(ex, "Đã xảy ra lỗi khi cập nhật bể cá với ID: {FishTankId}", id);
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

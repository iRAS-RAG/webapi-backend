using IRasRag.Application.Common.Models;
using IRasRag.Application.DTOs;
using IRasRag.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IRasRag.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/cameras")]
    public class CameraController : ControllerBase
    {
        private readonly ICameraService _cameraService;
        private readonly ILogger<CameraController> _logger;

        public CameraController(ICameraService cameraService, ILogger<CameraController> logger)
        {
            _cameraService = cameraService;
            _logger = logger;
        }

        [Authorize(Roles = "Supervisor")]
        [HttpPost]
        public async Task<IActionResult> CreateCamera([FromBody] CreateCameraDto dto)
        {
            try
            {
                var result = await _cameraService.CreateCameraAsync(dto);

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
                _logger.LogError(
                    ex,
                    "An error occurred while creating camera: {CameraName}",
                    dto.Name
                );
                return StatusCode(500, new { Message = "Có lỗi xảy ra, vui lòng thử lại sau." });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCameras()
        {
            try
            {
                var result = await _cameraService.GetAllCamerasAsync();
                return result.Type switch
                {
                    ResultType.Ok => Ok(new { result.Message, result.Data }),
                    _ => StatusCode(500, new { result.Message }),
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving cameras.");
                return StatusCode(500, new { Message = "Có lỗi xảy ra, vui lòng thử lại sau." });
            }
        }

        [Authorize(Roles = "Supervisor")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCameraById(Guid id)
        {
            try
            {
                var result = await _cameraService.GetCameraByIdAsync(id);
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
                _logger.LogError(ex, "An error occurred while retrieving camera: {CameraId}", id);
                return StatusCode(500, new { Message = "Có lỗi xảy ra, vui lòng thử lại sau." });
            }
        }

        [Authorize(Roles = "Supervisor")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCamera(Guid id, [FromBody] UpdateCameraDto dto)
        {
            try
            {
                var result = await _cameraService.UpdateCameraAsync(id, dto);

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
                _logger.LogError(ex, "An error occurred while updating camera: {CameraId}", id);
                return StatusCode(500, new { Message = "Có lỗi xảy ra, vui lòng thử lại sau." });
            }
        }

        [Authorize(Roles = "Supervisor")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCamera(Guid id)
        {
            try
            {
                var result = await _cameraService.DeleteCameraAsync(id);

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
                _logger.LogError(ex, "An error occurred while deleting camera: {CameraId}", id);
                return StatusCode(500, new { Message = "Có lỗi xảy ra, vui lòng thử lại sau." });
            }
        }
    }
}

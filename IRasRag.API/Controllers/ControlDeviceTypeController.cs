using IRasRag.Application.Common.Models;
using IRasRag.Application.DTOs;
using IRasRag.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IRasRag.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/control-device-types")]
    public class ControlDeviceTypeController : ControllerBase
    {
        private readonly IControlDeviceTypeService _controlDeviceTypeService;
        private readonly ILogger<ControlDeviceTypeController> _logger;

        public ControlDeviceTypeController(
            IControlDeviceTypeService controlDeviceTypeService,
            ILogger<ControlDeviceTypeController> logger
        )
        {
            _controlDeviceTypeService = controlDeviceTypeService;
            _logger = logger;
        }

        [Authorize(Roles = "Supervisor")]
        [HttpPost]
        public async Task<IActionResult> CreateControlDeviceType(
            [FromBody] CreateControlDeviceTypeDto dto
        )
        {
            try
            {
                var result = await _controlDeviceTypeService.CreateControlDeviceTypeAsync(dto);

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
                _logger.LogError(
                    ex,
                    "An error occurred while creating control device type: {ControlDeviceTypeName}",
                    dto.Name
                );
                return StatusCode(500, new { Message = "Có lỗi xảy ra, vui lòng thử lại sau." });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllControlDeviceTypes(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10
        )
        {
            try
            {
                if (page <= 0 || pageSize <= 0)
                {
                    return BadRequest(
                        new { Message = "Số trang và kích thước trang phải lớn hơn 0." }
                    );
                }

                if (pageSize > 100)
                {
                    return BadRequest(new { Message = "Kích thước trang tối đa là 100." });
                }

                var result = await _controlDeviceTypeService.GetAllControlDeviceTypesAsync(
                    page,
                    pageSize
                );
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving control device types.");
                return StatusCode(500, new { Message = "Có lỗi xảy ra, vui lòng thử lại sau." });
            }
        }

        [Authorize(Roles = "Supervisor")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetControlDeviceTypeById(Guid id)
        {
            try
            {
                var result = await _controlDeviceTypeService.GetControlDeviceTypeByIdAsync(id);
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
                    "An error occurred while retrieving control device type: {ControlDeviceTypeId}",
                    id
                );
                return StatusCode(500, new { Message = "Có lỗi xảy ra, vui lòng thử lại sau." });
            }
        }

        [Authorize(Roles = "Supervisor")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateControlDeviceType(
            Guid id,
            [FromBody] UpdateControlDeviceTypeDto dto
        )
        {
            try
            {
                var result = await _controlDeviceTypeService.UpdateControlDeviceTypeAsync(id, dto);

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
                _logger.LogError(
                    ex,
                    "An error occurred while updating control device type: {ControlDeviceTypeId}",
                    id
                );
                return StatusCode(500, new { Message = "Có lỗi xảy ra, vui lòng thử lại sau." });
            }
        }

        [Authorize(Roles = "Supervisor")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteControlDeviceType(Guid id)
        {
            try
            {
                var result = await _controlDeviceTypeService.DeleteControlDeviceTypeAsync(id);

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
                _logger.LogError(
                    ex,
                    "An error occurred while deleting control device type: {ControlDeviceTypeId}",
                    id
                );
                return StatusCode(500, new { Message = "Có lỗi xảy ra, vui lòng thử lại sau." });
            }
        }
    }
}

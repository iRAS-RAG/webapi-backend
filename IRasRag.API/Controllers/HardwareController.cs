using IRasRag.Application.Common.Models;
using IRasRag.Application.DTOs;
using IRasRag.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IRasRag.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/hardware")]
    public class HardwareController : ControllerBase
    {
        private ILogger<HardwareController> _logger;
        private readonly ISensorTypeService _sensorTypeService;
        private readonly ISensorService _sensorService;
        private readonly IMasterBoardService _masterBoardService;
        private readonly IControlDeviceService _controlDeviceService;

        public HardwareController(
            ILogger<HardwareController> logger,
            ISensorTypeService sensorTypeService,
            ISensorService sensorService,
            IMasterBoardService masterBoardService,
            IControlDeviceService controlDeviceService
        )
        {
            _logger = logger;
            _sensorTypeService = sensorTypeService;
            _sensorService = sensorService;
            _masterBoardService = masterBoardService;
            _controlDeviceService = controlDeviceService;
        }

        [HttpGet("sensor-types")]
        public async Task<IActionResult> GetAllSensorTypes(
            [FromQuery] SensorTypeListRequest request
        )
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
                var result = await _sensorTypeService.GetAllSensorTypesAsync(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving sensor types.");
                return StatusCode(
                    500,
                    new { Message = "Có lỗi xảy ra, vui lòng thử lại sau." }
                );
            }
        }

        [HttpGet("sensors")]
        public async Task<IActionResult> GetAllSensors([FromQuery] SensorListRequest request)
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
                var result = await _sensorService.GetAllSensorsAsync(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving sensors.");
                return StatusCode(500, new { Message = "Có lỗi xảy ra, vui lòng thử lại sau." });
            }
        }

        [HttpGet("masterboards")]
        public async Task<IActionResult> GetAllMasterBoards(
            [FromQuery] MasterBoardListRequest request
        )
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
                var result = await _masterBoardService.GetAllMasterBoardsAsync(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving master boards.");
                return StatusCode(500, new { Message = "Có lỗi xảy ra, vui lòng thử lại sau." });
            }
        }

        [HttpGet("control-devices")]
        public async Task<IActionResult> GetAllControlDevicesByTank([FromQuery] ControlDeviceListRequest request)
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
                var result = await _controlDeviceService.GetAllControlDevicesAsync(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving control devices.");
                return StatusCode(500, new { Message = "Có lỗi xảy ra, vui lòng thử lại sau." });
            }
        }

        [HttpPost("masterboards")]
        public async Task<IActionResult> CreateMasterBoard([FromBody] CreateMasterBoardDto dto)
        {
            try
            {
                var result = await _masterBoardService.CreateMasterBoardAsync(dto);
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
                    "An error occurred while creating master board: {MasterBoardName}",
                    dto.Name
                );
                return StatusCode(500, new { Message = "Có lỗi xảy ra, vui lòng thử lại sau." });
            }
        }

        [HttpPost("control-devices")]
        public async Task<IActionResult> CreateControlDevice([FromBody] CreateControlDeviceDto dto)
        {
            try
            {
                var result = await _controlDeviceService.CreateControlDeviceAsync(dto);
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
                    "An error occurred while creating control device: {ControlDeviceName}",
                    dto.Name
                );
                return StatusCode(500, new { Message = "Có lỗi xảy ra, vui lòng thử lại sau." });
            }
        }

        [HttpPost("sensors")]
        public async Task<IActionResult> CreateSensor([FromBody] CreateSensorDto dto)
        {
            try
            {
                var result = await _sensorService.CreateSensorAsync(dto);
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
                    "An error occurred while creating sensor: {SensorName}",
                    dto.Name
                );
                return StatusCode(500, new { Message = "Có lỗi xảy ra, vui lòng thử lại sau." });
            }
        }

        [HttpPut("sensors/{id}")]
        public async Task<IActionResult> UpdateSensor(Guid id, [FromBody] UpdateSensorDto dto)
        {
            try
            {
                var result = await _sensorService.UpdateSensorAsync(id, dto);
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
                _logger.LogError(ex, "An error occurred while updating sensor: {SensorId}", id);
                return StatusCode(500, new { Message = "Có lỗi xảy ra, vui lòng thử lại sau." });
            }
        }
    }
}

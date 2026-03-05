using IRasRag.Application.Common.Models;
using IRasRag.Application.DTOs;
using IRasRag.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IRasRag.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/hardwares")]
    public class HardwareController : ControllerBase
    {
        private ILogger<HardwareController> _logger;
        private readonly ISensorTypeService _sensorTypeService;
        private readonly ISensorService _sensorService;
        private readonly IMasterBoardService _masterBoardService;
        private readonly IControlDeviceService _controlDeviceService;
        private readonly IControlDeviceTypeService _controlDeviceTypeService;

        public HardwareController(
            ILogger<HardwareController> logger,
            ISensorTypeService sensorTypeService,
            ISensorService sensorService,
            IMasterBoardService masterBoardService,
            IControlDeviceService controlDeviceService,
            IControlDeviceTypeService controlDeviceTypeService
        )
        {
            _logger = logger;
            _sensorTypeService = sensorTypeService;
            _sensorService = sensorService;
            _masterBoardService = masterBoardService;
            _controlDeviceService = controlDeviceService;
            _controlDeviceTypeService = controlDeviceTypeService;
        }

        #region Sensor Types

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
                return StatusCode(500, new { Message = "Có lỗi xảy ra, vui lòng thử lại sau." });
            }
        }

        [HttpGet("sensor-types/{id}")]
        [Authorize(Roles = "Supervisor")]
        public async Task<IActionResult> GetSensorTypeById(Guid id)
        {
            try
            {
                var result = await _sensorTypeService.GetSensorTypeByIdAsync(id);
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
                    "An error occurred while retrieving sensor type: {SensorTypeId}",
                    id
                );
                return StatusCode(500, new { Message = "Có lỗi xảy ra, vui lòng thử lại sau." });
            }
        }

        [HttpPost("sensor-types")]
        [Authorize(Roles = "Supervisor")]
        public async Task<IActionResult> CreateSensorType([FromBody] CreateSensorTypeDto dto)
        {
            try
            {
                var result = await _sensorTypeService.CreateSensorTypeAsync(dto);
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
                    "An error occurred while creating sensor type: {SensorTypeName}",
                    dto.Name
                );
                return StatusCode(500, new { Message = "Có lỗi xảy ra, vui lòng thử lại sau." });
            }
        }

        [HttpPut("sensor-types/{id}")]
        [Authorize(Roles = "Supervisor")]
        public async Task<IActionResult> UpdateSensorType(
            Guid id,
            [FromBody] UpdateSensorTypeDto dto
        )
        {
            try
            {
                var result = await _sensorTypeService.UpdateSensorTypeAsync(id, dto);
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
                    "An error occurred while updating sensor type: {SensorTypeId}",
                    id
                );
                return StatusCode(500, new { Message = "Có lỗi xảy ra, vui lòng thử lại sau." });
            }
        }

        [HttpDelete("sensor-types/{id}")]
        [Authorize(Roles = "Supervisor")]
        public async Task<IActionResult> DeleteSensorType(Guid id)
        {
            try
            {
                var result = await _sensorTypeService.DeleteSensorTypeAsync(id);
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
                    "An error occurred while deleting sensor type: {SensorTypeId}",
                    id
                );
                return StatusCode(500, new { Message = "Có lỗi xảy ra, vui lòng thử lại sau." });
            }
        }

        #endregion

        #region Sensors

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

        [HttpGet("sensors/{id}")]
        [Authorize(Roles = "Supervisor")]
        public async Task<IActionResult> GetSensorById(Guid id)
        {
            try
            {
                var result = await _sensorService.GetSensorByIdAsync(id);
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
                _logger.LogError(ex, "An error occurred while retrieving sensor: {SensorId}", id);
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

        [HttpGet("sensors/{id}/history")]
        public async Task<IActionResult> GetSensorHistory(
            Guid id,
            [FromQuery] DateTime? from,
            [FromQuery] DateTime? to,
            [FromQuery] int interval = 60)
        {
            try
            {
                var fromDt = from ?? DateTime.Today;
                var toDt = to ?? DateTime.Today.AddDays(1);

                var result = await _sensorService.GetSensorHistoryAsync(id, fromDt, toDt, interval);

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
                    "An error occurred while retrieving sensor history: {SensorId}",
                    id
                );

                return StatusCode(500, new { Message = "Có lỗi xảy ra, vui lòng thử lại sau." });
            }
        }

        [HttpPut("sensors/{id}")]
        [Authorize(Roles = "Supervisor")]
        public async Task<IActionResult> UpdateSensor(Guid id, [FromBody] UpdateSensorDto dto)
        {
            try
            {
                var result = await _sensorService.UpdateSensorAsync(id, dto);
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
                _logger.LogError(ex, "An error occurred while updating sensor: {SensorId}", id);
                return StatusCode(500, new { Message = "Có lỗi xảy ra, vui lòng thử lại sau." });
            }
        }

        [HttpDelete("sensors/{id}")]
        [Authorize(Roles = "Supervisor")]
        public async Task<IActionResult> DeleteSensor(Guid id)
        {
            try
            {
                var result = await _sensorService.DeleteSensorAsync(id);
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
                _logger.LogError(ex, "An error occurred while deleting sensor: {SensorId}", id);
                return StatusCode(500, new { Message = "Có lỗi xảy ra, vui lòng thử lại sau." });
            }
        }

        #endregion

        #region Master Boards

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

        [HttpGet("masterboards/{id}")]
        [Authorize(Roles = "Supervisor")]
        public async Task<IActionResult> GetMasterBoardById(Guid id)
        {
            try
            {
                var result = await _masterBoardService.GetMasterBoardByIdAsync(id);
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
                    "An error occurred while retrieving master board: {MasterBoardId}",
                    id
                );
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

        [HttpPut("masterboards/{id}")]
        [Authorize(Roles = "Supervisor")]
        public async Task<IActionResult> UpdateMasterBoard(
            Guid id,
            [FromBody] UpdateMasterBoardDto dto
        )
        {
            try
            {
                var result = await _masterBoardService.UpdateMasterBoardAsync(id, dto);
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
                    "An error occurred while updating master board: {MasterBoardId}",
                    id
                );
                return StatusCode(500, new { Message = "Có lỗi xảy ra, vui lòng thử lại sau." });
            }
        }

        [HttpDelete("masterboards/{id}")]
        [Authorize(Roles = "Supervisor")]
        public async Task<IActionResult> DeleteMasterBoard(Guid id)
        {
            try
            {
                var result = await _masterBoardService.DeleteMasterBoardAsync(id);
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
                    "An error occurred while deleting master board: {MasterBoardId}",
                    id
                );
                return StatusCode(500, new { Message = "Có lỗi xảy ra, vui lòng thử lại sau." });
            }
        }

        #endregion

        #region Control Devices

        [HttpGet("control-devices")]
        public async Task<IActionResult> GetAllControlDevices(
            [FromQuery] ControlDeviceListRequest request
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
                var result = await _controlDeviceService.GetAllControlDevicesAsync(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving control devices.");
                return StatusCode(500, new { Message = "Có lỗi xảy ra, vui lòng thử lại sau." });
            }
        }

        [HttpGet("control-devices/{id}")]
        public async Task<IActionResult> GetControlDeviceById(Guid id)
        {
            try
            {
                var result = await _controlDeviceService.GetControlDeviceByIdAsync(id);
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
                    "An error occurred while retrieving control device: {ControlDeviceId}",
                    id
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

        [HttpPut("control-devices/{id}")]
        [Authorize(Roles = "Supervisor")]
        public async Task<IActionResult> UpdateControlDevice(
            Guid id,
            [FromBody] UpdateControlDeviceDto dto
        )
        {
            try
            {
                var result = await _controlDeviceService.UpdateControlDeviceAsync(id, dto);
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
                    "An error occurred while updating control device: {ControlDeviceId}",
                    id
                );
                return StatusCode(500, new { Message = "Có lỗi xảy ra, vui lòng thử lại sau." });
            }
        }

        [HttpDelete("control-devices/{id}")]
        [Authorize(Roles = "Supervisor")]
        public async Task<IActionResult> DeleteControlDevice(Guid id)
        {
            try
            {
                var result = await _controlDeviceService.DeleteControlDeviceAsync(id);
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
                    "An error occurred while deleting control device: {ControlDeviceId}",
                    id
                );
                return StatusCode(500, new { Message = "Có lỗi xảy ra, vui lòng thử lại sau." });
            }
        }

        /// <summary>
        /// Manual Override: Bật/tắt thiết bị điều khiển theo yêu cầu thủ công.
        /// </summary>
        [HttpPost("control-devices/{id}/toggle")]
        [Authorize(Roles = "Supervisor")]
        public async Task<IActionResult> ToggleControlDevice(Guid id,[FromBody] ToggleControlDeviceDto dto)
        {
            try
            {
                var result = await _controlDeviceService.ToggleControlDeviceAsync(id, dto);
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
                _logger.LogError(ex, "Error toggling control device with: {ControlDeviceId}", id);
                return StatusCode(500, new { Message = "Có lỗi xảy ra, vui lòng thử lại sau." });
            }
        }

        #endregion

        #region Control Device Types

        [HttpGet("control-device-types")]
        public async Task<IActionResult> GetAllControlDeviceTypes(
            [FromQuery] ControlDeviceTypeListRequest request
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
                var result = await _controlDeviceTypeService.GetAllControlDeviceTypesAsync(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving control device types.");
                return StatusCode(500, new { Message = "Có lỗi xảy ra, vui lòng thử lại sau." });
            }
        }

        [HttpGet("control-device-types/{id}")]
        [Authorize(Roles = "Supervisor")]
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

        [HttpPost("control-device-types")]
        [Authorize(Roles = "Supervisor")]
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

        [HttpPut("control-device-types/{id}")]
        [Authorize(Roles = "Supervisor")]
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

        [HttpDelete("control-device-types/{id}")]
        [Authorize(Roles = "Supervisor")]
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

        #endregion
    }
}

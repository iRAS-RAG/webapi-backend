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
            IControlDeviceService controlDeviceService)
        {
            _logger = logger;
            _sensorTypeService = sensorTypeService;
            _sensorService = sensorService;
            _masterBoardService = masterBoardService;
            _controlDeviceService = controlDeviceService;
        }

        [HttpGet("sensor-types")]
        public async Task<IActionResult> GetAllSensorTypes(int page = 1, int pageSize = 10)
        {
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

                    var result = await _sensorTypeService.GetAllSensorTypesAsync(page, pageSize);
                    return Ok(result);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred while retrieving sensor types.");
                    return StatusCode(500, new { Message = "Có lỗi xảy ra, vui lòng thử lại sau." });
                }
            }
        }

        [HttpGet("sensors")]
        public async Task<IActionResult> GetAllSensors(int page = 1, int pageSize = 10)
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
                var result = await _sensorService.GetAllSensorsAsync(page, pageSize);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving sensors.");
                return StatusCode(500, new { Message = "Có lỗi xảy ra, vui lòng thử lại sau." });
            }
        }

        [HttpGet("sensors/masterboard/{id}")]
        public async Task<IActionResult> GetAllSensorsByMasterBoardId(Guid id, int page = 1, int pageSize = 10)
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
                var result = await _sensorService.GetAllSensorsByMasterBoardIdAsync(id, page, pageSize);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving sensors by master board ID.");
                return StatusCode(500, new { Message = "Có lỗi xảy ra, vui lòng thử lại sau." });
            }
        }

        [HttpGet("masterboards")]
        public async Task<IActionResult> GetAllMasterBoards(int page = 1, int pageSize = 10)
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
                var result = await _masterBoardService.GetAllMasterBoardsAsync(page, pageSize);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving master boards.");
                return StatusCode(500, new { Message = "Có lỗi xảy ra, vui lòng thử lại sau." });
            }
        }

        [HttpGet("masterboards/tank/{id}")]
        public async Task<IActionResult> GetAllMasterBoardsByTankId(Guid id, int page = 1, int pageSize = 10)
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
                var result = await _masterBoardService.GetAllMasterBoardsByTankIdAsync(id, page, pageSize);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving master boards by tank ID.");
                return StatusCode(500, new { Message = "Có lỗi xảy ra, vui lòng thử lại sau." });
            }
        }

        [HttpGet("control-devices")]
        public async Task<IActionResult> GetAllControlDevices(int page = 1, int pageSize = 10)
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
                var result = await _controlDeviceService.GetAllControlDevicesAsync(page, pageSize);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving control devices.");
                return StatusCode(500, new { Message = "Có lỗi xảy ra, vui lòng thử lại sau." });
            }
        }

        [HttpGet("control-devices/masterboard/{id}")]
        public async Task<IActionResult> GetAllControlDevicesByMasterBoardId(Guid id, int page = 1, int pageSize = 10)
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
                var result = await _controlDeviceService.GetAllControlDevicesByMasterBoardIdAsync(id, page, pageSize);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving control devices by master board ID.");
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
                _logger.LogError(
                    ex,
                    "An error occurred while updating sensor: {SensorId}",
                    id
                );
                return StatusCode(500, new { Message = "Có lỗi xảy ra, vui lòng thử lại sau." });
            }
        }
    }
}

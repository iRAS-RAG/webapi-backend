using IRasRag.Application.Common.Interfaces;
using IRasRag.Application.Common.Models;
using IRasRag.Application.DTOs;
using IRasRag.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IRasRag.API.Controllers
{
    [ApiController]
    [Route("api/job-control-mappings")]
    [Authorize]
    public class JobControlMappingController : ControllerBase
    {
        private readonly IJobControlMappingService _jobControlMappingService;
        private readonly ILogger<JobControlMappingController> _logger;

        public JobControlMappingController(
            IJobControlMappingService jobControlMappingService,
            ILogger<JobControlMappingController> logger
        )
        {
            _jobControlMappingService = jobControlMappingService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllJobControlMappings(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10
        )
        {
            try
            {
                if (page <= 0 || pageSize <= 0)
                {
                    return BadRequest(new { Message = "Page và PageSize phải lớn hơn 0" });
                }

                if (pageSize > 100)
                {
                    return BadRequest(new { Message = "PageSize không được vượt quá 100" });
                }

                var result = await _jobControlMappingService.GetAllJobControlMappingsAsync(
                    page,
                    pageSize
                );
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy danh sách ánh xạ job-thiết bị điều khiển");
                return StatusCode(
                    500,
                    new
                    {
                        Message = "Đã xảy ra lỗi khi lấy danh sách ánh xạ job-thiết bị điều khiển",
                    }
                );
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetJobControlMappingById(Guid id)
        {
            try
            {
                var result = await _jobControlMappingService.GetJobControlMappingByIdAsync(id);

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
                    "Lỗi khi lấy thông tin ánh xạ job-thiết bị điều khiển với ID {Id}",
                    id
                );
                return StatusCode(
                    500,
                    new
                    {
                        Message = "Đã xảy ra lỗi khi lấy thông tin ánh xạ job-thiết bị điều khiển",
                    }
                );
            }
        }

        [HttpPost]
        [Authorize(Roles = "Supervisor")]
        public async Task<IActionResult> CreateJobControlMapping(
            [FromBody] CreateJobControlMappingDto createDto
        )
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var result = await _jobControlMappingService.CreateJobControlMappingAsync(
                    createDto
                );

                return result.Type switch
                {
                    ResultType.Ok => CreatedAtAction(
                        nameof(GetJobControlMappingById),
                        new { id = result.Data?.Id },
                        new { result.Message, result.Data }
                    ),
                    ResultType.BadRequest => BadRequest(new { result.Message }),
                    ResultType.Conflict => Conflict(new { result.Message }),
                    _ => StatusCode(500, new { result.Message }),
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tạo ánh xạ job-thiết bị điều khiển");
                return StatusCode(
                    500,
                    new { Message = "Đã xảy ra lỗi khi tạo ánh xạ job-thiết bị điều khiển" }
                );
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Supervisor")]
        public async Task<IActionResult> UpdateJobControlMapping(
            Guid id,
            [FromBody] UpdateJobControlMappingDto updateDto
        )
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var result = await _jobControlMappingService.UpdateJobControlMappingAsync(
                    id,
                    updateDto
                );

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
                    "Lỗi khi cập nhật ánh xạ job-thiết bị điều khiển với ID {Id}",
                    id
                );
                return StatusCode(
                    500,
                    new { Message = "Đã xảy ra lỗi khi cập nhật ánh xạ job-thiết bị điều khiển" }
                );
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Supervisor")]
        public async Task<IActionResult> DeleteJobControlMapping(Guid id)
        {
            try
            {
                var result = await _jobControlMappingService.DeleteJobControlMappingAsync(id);

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
                _logger.LogError(ex, "Lỗi khi xóa ánh xạ job-thiết bị điều khiển với ID {Id}", id);
                return StatusCode(
                    500,
                    new { Message = "Đã xảy ra lỗi khi xóa ánh xạ job-thiết bị điều khiển" }
                );
            }
        }
    }
}

using IRasRag.Application.Common.Models;
using IRasRag.Application.DTOs;
using IRasRag.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IRasRag.API.Controllers
{
    [ApiController]
    [Route("api/job-types")]
    [Authorize]
    public class JobTypeController : ControllerBase
    {
        private readonly IJobTypeService _jobTypeService;
        private readonly ILogger<JobTypeController> _logger;

        public JobTypeController(IJobTypeService jobTypeService, ILogger<JobTypeController> logger)
        {
            _jobTypeService = jobTypeService;
            _logger = logger;
        }

        /// <summary>
        /// Lấy danh sách tất cả loại công việc
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAllJobTypes()
        {
            try
            {
                var result = await _jobTypeService.GetAllJobTypesAsync();
                return result.Type switch
                {
                    ResultType.Ok => Ok(new { result.Message, result.Data }),
                    _ => StatusCode(500, new { result.Message }),
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving job types.");
                return StatusCode(500, new { Message = "Có lỗi xảy ra, vui lòng thử lại sau." });
            }
        }

        /// <summary>
        /// Lấy thông tin loại công việc theo Id
        /// </summary>
        [Authorize(Roles = "Supervisor")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetJobTypeById(Guid id)
        {
            try
            {
                var result = await _jobTypeService.GetJobTypeByIdAsync(id);
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
                    "An error occurred while retrieving job type: {JobTypeId}",
                    id
                );
                return StatusCode(500, new { Message = "Có lỗi xảy ra, vui lòng thử lại sau." });
            }
        }

        /// <summary>
        /// Tạo loại công việc mới
        /// </summary>
        [Authorize(Roles = "Supervisor")]
        [HttpPost]
        public async Task<IActionResult> CreateJobType([FromBody] CreateJobTypeDto dto)
        {
            try
            {
                var result = await _jobTypeService.CreateJobTypeAsync(dto);

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
                    "An error occurred while creating job type: {JobTypeName}",
                    dto.Name
                );
                return StatusCode(500, new { Message = "Có lỗi xảy ra, vui lòng thử lại sau." });
            }
        }

        /// <summary>
        /// Cập nhật thông tin loại công việc
        /// </summary>
        [Authorize(Roles = "Supervisor")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateJobType(Guid id, [FromBody] UpdateJobTypeDto dto)
        {
            try
            {
                var result = await _jobTypeService.UpdateJobTypeAsync(id, dto);

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
                _logger.LogError(ex, "An error occurred while updating job type: {JobTypeId}", id);
                return StatusCode(500, new { Message = "Có lỗi xảy ra, vui lòng thử lại sau." });
            }
        }

        /// <summary>
        /// Xóa loại công việc
        /// </summary>
        [Authorize(Roles = "Supervisor")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteJobType(Guid id)
        {
            try
            {
                var result = await _jobTypeService.DeleteJobTypeAsync(id);

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
                _logger.LogError(ex, "An error occurred while deleting job type: {JobTypeId}", id);
                return StatusCode(500, new { Message = "Có lỗi xảy ra, vui lòng thử lại sau." });
            }
        }
    }
}

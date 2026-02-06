using IRasRag.Application.Common.Interfaces;
using IRasRag.Application.Common.Models;
using IRasRag.Application.DTOs;
using IRasRag.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IRasRag.API.Controllers
{
    [ApiController]
    [Route("api/user-farms")]
    [Authorize]
    public class UserFarmController : ControllerBase
    {
        private readonly IUserFarmService _userFarmService;
        private readonly ILogger<UserFarmController> _logger;

        public UserFarmController(
            IUserFarmService userFarmService,
            ILogger<UserFarmController> logger
        )
        {
            _userFarmService = userFarmService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUserFarms(
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

                var result = await _userFarmService.GetAllUserFarmsAsync(page, pageSize);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy danh sách phân quyền người dùng-trang trại");
                return StatusCode(
                    500,
                    new
                    {
                        Message = "Đã xảy ra lỗi khi lấy danh sách phân quyền người dùng-trang trại",
                    }
                );
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserFarmById(Guid id)
        {
            try
            {
                var result = await _userFarmService.GetUserFarmByIdAsync(id);

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
                    "Lỗi khi lấy thông tin phân quyền người dùng-trang trại với ID {Id}",
                    id
                );
                return StatusCode(
                    500,
                    new
                    {
                        Message = "Đã xảy ra lỗi khi lấy thông tin phân quyền người dùng-trang trại",
                    }
                );
            }
        }

        [HttpPost]
        [Authorize(Roles = "Supervisor")]
        public async Task<IActionResult> CreateUserFarm([FromBody] CreateUserFarmDto createDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var result = await _userFarmService.CreateUserFarmAsync(createDto);

                return result.Type switch
                {
                    ResultType.Ok => CreatedAtAction(
                        nameof(GetUserFarmById),
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
                _logger.LogError(ex, "Lỗi khi tạo phân quyền người dùng-trang trại");
                return StatusCode(
                    500,
                    new { Message = "Đã xảy ra lỗi khi tạo phân quyền người dùng-trang trại" }
                );
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Supervisor")]
        public async Task<IActionResult> UpdateUserFarm(
            Guid id,
            [FromBody] UpdateUserFarmDto updateDto
        )
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var result = await _userFarmService.UpdateUserFarmAsync(id, updateDto);

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
                    "Lỗi khi cập nhật phân quyền người dùng-trang trại với ID {Id}",
                    id
                );
                return StatusCode(
                    500,
                    new { Message = "Đã xảy ra lỗi khi cập nhật phân quyền người dùng-trang trại" }
                );
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Supervisor")]
        public async Task<IActionResult> DeleteUserFarm(Guid id)
        {
            try
            {
                var result = await _userFarmService.DeleteUserFarmAsync(id);

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
                    "Lỗi khi xóa phân quyền người dùng-trang trại với ID {Id}",
                    id
                );
                return StatusCode(
                    500,
                    new { Message = "Đã xảy ra lỗi khi xóa phân quyền người dùng-trang trại" }
                );
            }
        }
    }
}

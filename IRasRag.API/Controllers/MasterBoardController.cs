using IRasRag.Application.Common.Models;
using IRasRag.Application.DTOs;
using IRasRag.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IRasRag.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/master-boards")]
    public class MasterBoardController : ControllerBase
    {
        private readonly IMasterBoardService _masterBoardService;
        private readonly ILogger<MasterBoardController> _logger;

        public MasterBoardController(
            IMasterBoardService masterBoardService,
            ILogger<MasterBoardController> logger
        )
        {
            _masterBoardService = masterBoardService;
            _logger = logger;
        }

        /// <summary>
        /// Lấy danh sách tất cả bảng mạch
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAllMasterBoards()
        {
            try
            {
                var result = await _masterBoardService.GetAllMasterBoardsAsync();
                return result.Type switch
                {
                    ResultType.Ok => Ok(new { result.Message, result.Data }),
                    _ => StatusCode(500, new { result.Message }),
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving master boards.");
                return StatusCode(500, new { Message = "Có lỗi xảy ra, vui lòng thử lại sau." });
            }
        }

        /// <summary>
        /// Lấy thông tin bảng mạch theo Id
        /// </summary>
        [Authorize(Roles = "Supervisor")]
        [HttpGet("{id}")]
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

        /// <summary>
        /// Tạo bảng mạch mới
        /// </summary>
        [Authorize(Roles = "Supervisor")]
        [HttpPost]
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
                    ResultType.NotFound => NotFound(new { result.Message }),
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

        /// <summary>
        /// Cập nhật thông tin bảng mạch
        /// </summary>
        [Authorize(Roles = "Supervisor")]
        [HttpPut("{id}")]
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

        /// <summary>
        /// Xóa bảng mạch
        /// </summary>
        [Authorize(Roles = "Supervisor")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMasterBoard(Guid id)
        {
            try
            {
                var result = await _masterBoardService.DeleteMasterBoardAsync(id);

                return result.Type switch
                {
                    ResultType.Ok => Ok(new { result.Message }),
                    ResultType.NotFound => NotFound(new { result.Message }),
                    ResultType.BadRequest => BadRequest(new { result.Message }),
                    ResultType.Conflict => Conflict(new { result.Message }),
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
    }
}

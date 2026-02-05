using IRasRag.Application.Common.Models;
using IRasRag.Application.DTOs;
using IRasRag.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IRasRag.API.Controllers
{
    [ApiController]
    [Route("api/recommendations")]
    [Authorize]
    public class RecommendationController : ControllerBase
    {
        private readonly IRecommendationService _recommendationService;
        private readonly ILogger<RecommendationController> _logger;

        public RecommendationController(
            IRecommendationService recommendationService,
            ILogger<RecommendationController> logger
        )
        {
            _recommendationService = recommendationService;
            _logger = logger;
        }

        /// <summary>
        /// Lấy danh sách tất cả khuyến nghị
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAllRecommendations(
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

                var result = await _recommendationService.GetAllRecommendationsAsync(
                    page,
                    pageSize
                );
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy danh sách khuyến nghị");
                return StatusCode(500, new { Message = "Đã xảy ra lỗi khi xử lý yêu cầu" });
            }
        }

        /// <summary>
        /// Lấy thông tin khuyến nghị theo Id
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetRecommendationById(Guid id)
        {
            try
            {
                var result = await _recommendationService.GetRecommendationByIdAsync(id);

                if (!result.IsSuccess)
                {
                    return result.Type switch
                    {
                        ResultType.NotFound => NotFound(new { result.Message }),
                        _ => BadRequest(new { result.Message }),
                    };
                }

                return Ok(new { result.Message, result.Data });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy khuyến nghị với Id: {Id}", id);
                return StatusCode(500, new { Message = "Đã xảy ra lỗi khi xử lý yêu cầu" });
            }
        }

        /// <summary>
        /// Tạo khuyến nghị mới
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Supervisor")]
        public async Task<IActionResult> CreateRecommendation(
            [FromBody] CreateRecommendationDto createDto
        )
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(
                        new { Message = "Dữ liệu không hợp lệ", Errors = ModelState }
                    );
                }

                var result = await _recommendationService.CreateRecommendationAsync(createDto);

                if (!result.IsSuccess)
                {
                    return result.Type switch
                    {
                        ResultType.NotFound => NotFound(new { result.Message }),
                        ResultType.Conflict => Conflict(new { result.Message }),
                        ResultType.BadRequest => BadRequest(new { result.Message }),
                        _ => BadRequest(new { result.Message }),
                    };
                }

                return CreatedAtAction(
                    nameof(GetRecommendationById),
                    new { id = result.Data!.Id },
                    new { result.Message, result.Data }
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tạo khuyến nghị");
                return StatusCode(500, new { Message = "Đã xảy ra lỗi khi xử lý yêu cầu" });
            }
        }

        /// <summary>
        /// Cập nhật khuyến nghị
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "Supervisor")]
        public async Task<IActionResult> UpdateRecommendation(
            Guid id,
            [FromBody] UpdateRecommendationDto updateDto
        )
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(
                        new { Message = "Dữ liệu không hợp lệ", Errors = ModelState }
                    );
                }

                var result = await _recommendationService.UpdateRecommendationAsync(id, updateDto);

                if (!result.IsSuccess)
                {
                    return result.Type switch
                    {
                        ResultType.NotFound => NotFound(new { result.Message }),
                        ResultType.Conflict => Conflict(new { result.Message }),
                        ResultType.BadRequest => BadRequest(new { result.Message }),
                        _ => BadRequest(new { result.Message }),
                    };
                }

                return Ok(new { result.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi cập nhật khuyến nghị với Id: {Id}", id);
                return StatusCode(500, new { Message = "Đã xảy ra lỗi khi xử lý yêu cầu" });
            }
        }

        /// <summary>
        /// Xóa khuyến nghị
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Supervisor")]
        public async Task<IActionResult> DeleteRecommendation(Guid id)
        {
            try
            {
                var result = await _recommendationService.DeleteRecommendationAsync(id);

                if (!result.IsSuccess)
                {
                    return result.Type switch
                    {
                        ResultType.NotFound => NotFound(new { result.Message }),
                        ResultType.Conflict => Conflict(new { result.Message }),
                        _ => BadRequest(new { result.Message }),
                    };
                }

                return Ok(new { result.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xóa khuyến nghị với Id: {Id}", id);
                return StatusCode(500, new { Message = "Đã xảy ra lỗi khi xử lý yêu cầu" });
            }
        }
    }
}

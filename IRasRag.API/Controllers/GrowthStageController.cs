using IRasRag.Application.Common.Models;
using IRasRag.Application.DTOs;
using IRasRag.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace IRasRag.API.Controllers
{
    [ApiController]
    [Route("api/growth-stages")]
    public class GrowthStageController : ControllerBase
    {
        private readonly IGrowthStageService _growthStageService;
        private readonly ILogger<GrowthStageController> _logger;

        public GrowthStageController(
            IGrowthStageService growthStageService,
            ILogger<GrowthStageController> logger
        )
        {
            _growthStageService = growthStageService;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> CreateGrowthStage([FromBody] CreateGrowthStageDto dto)
        {
            try
            {
                var result = await _growthStageService.CreateGrowthStageAsync(dto);

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
                    "An error occurred while creating growth stage: {GrowthStageName}",
                    dto.Name
                );
                return StatusCode(500, new { Message = "Có lỗi xảy ra, vui lòng thử lại sau." });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllGrowthStages()
        {
            try
            {
                var result = await _growthStageService.GetAllGrowthStagesAsync();
                return result.Type switch
                {
                    ResultType.Ok => Ok(new { result.Message, result.Data }),
                    _ => StatusCode(500, new { result.Message }),
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving growth stages.");
                return StatusCode(500, new { Message = "Có lỗi xảy ra, vui lòng thử lại sau." });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetGrowthStageById(Guid id)
        {
            try
            {
                var result = await _growthStageService.GetGrowthStageByIdAsync(id);
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
                    "An error occurred while retrieving growth stage with ID: {GrowthStageId}",
                    id
                );
                return StatusCode(500, new { Message = "Có lỗi xảy ra, vui lòng thử lại sau." });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateGrowthStage(
            Guid id,
            [FromBody] UpdateGrowthStageDto dto
        )
        {
            try
            {
                var result = await _growthStageService.UpdateGrowthStageAsync(id, dto);
                return result.Type switch
                {
                    ResultType.Ok => Ok(result.Message),
                    ResultType.Conflict => Conflict(new { result.Message }),
                    ResultType.BadRequest => BadRequest(new { result.Message }),
                    _ => StatusCode(500, new { result.Message }),
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "An error occurred while creating growth stage: {GrowthStageName}",
                    dto.Name
                );
                return StatusCode(500, new { Message = "Có lỗi xảy ra, vui lòng thử lại sau." });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGrowthStage(Guid id)
        {
            try
            {
                var result = await _growthStageService.DeleteGrowthStageAsync(id);
                return result.Type switch
                {
                    ResultType.Ok => Ok(result.Message),
                    ResultType.NotFound => NotFound(new { result.Message }),
                    _ => StatusCode(500, new { result.Message }),
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "An error occurred while deleting growth stage with ID: {GrowthStageId}",
                    id
                );
                return StatusCode(500, new { Message = "Có lỗi xảy ra, vui lòng thử lại sau." });
            }
        }
    }
}

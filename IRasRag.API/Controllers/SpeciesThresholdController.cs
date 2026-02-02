using IRasRag.Application.Common.Models;
using IRasRag.Application.DTOs;
using IRasRag.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace IRasRag.API.Controllers
{
    [ApiController]
    [Route("api/species-threshholds")]
    public class SpeciesThresholdController : ControllerBase
    {
        private readonly ILogger<SpeciesThresholdController> _logger;
        private readonly ISpeciesThresholdService _speciesThresholdService;

        public SpeciesThresholdController(
            ILogger<SpeciesThresholdController> logger,
            ISpeciesThresholdService speciesThresholdService
        )
        {
            _logger = logger;
            _speciesThresholdService = speciesThresholdService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllSpeciesThresholds()
        {
            try
            {
                var result = await _speciesThresholdService.GetAllSpeciesThresholdsAsync();
                return result.Type switch
                {
                    ResultType.Ok => Ok(new { result.Message, result.Data }),
                    _ => StatusCode(500, new { result.Message }),
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving species thresholds.");
                return StatusCode(500, new { Message = "Có lỗi xảy ra, vui lòng thử lại sau." });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetSpeciesThresholdById(Guid id)
        {
            try
            {
                var result = await _speciesThresholdService.GetSpeciesThresholdById(id);
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
                    "An error occurred while retrieving species threshold with ID: {SpeciesThresholdId}",
                    id
                );
                return StatusCode(500, new { Message = "Có lỗi xảy ra, vui lòng thử lại sau." });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateSpeciesThreshold(
            [FromBody] CreateSpeciesThresholdDto dto
        )
        {
            try
            {
                var result = await _speciesThresholdService.CreateSpeciesThreshold(dto);
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
                _logger.LogError(ex, "An error occurred while creating a new species threshold.");
                return StatusCode(500, new { Message = "Có lỗi xảy ra, vui lòng thử lại sau." });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSpeciesThreshold(
            Guid id,
            [FromBody] UpdateSpeciesThresholdDto dto
        )
        {
            try
            {
                var result = await _speciesThresholdService.UpdateSpeciesThreshold(id, dto);
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
                    "An error occurred while updating species threshold with ID: {SpeciesThresholdId}",
                    id
                );
                return StatusCode(500, new { Message = "Có lỗi xảy ra, vui lòng thử lại sau." });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSpeciesThreshold(Guid id)
        {
            try
            {
                var result = await _speciesThresholdService.DeleteSpeciesThreshold(id);
                return result.Type switch
                {
                    ResultType.Ok => Ok(new { result.Message }),
                    ResultType.NotFound => NotFound(new { result.Message }),
                    _ => StatusCode(500, new { result.Message }),
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "An error occurred while deleting species threshold with ID: {SpeciesThresholdId}",
                    id
                );
                return StatusCode(500, new { Message = "Có lỗi xảy ra, vui lòng thử lại sau." });
            }
        }
    }
}

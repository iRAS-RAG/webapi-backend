using IRasRag.Application.Common.Models;
using IRasRag.Application.DTOs;
using IRasRag.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IRasRag.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/species-stage-configs")]
    public class SpeciesStageConfigController : ControllerBase
    {
        private readonly ILogger<SpeciesStageConfigController> _logger;
        private readonly ISpeciesStageConfigService _speciesStageConfigService;

        public SpeciesStageConfigController(
            ILogger<SpeciesStageConfigController> logger,
            ISpeciesStageConfigService speciesStageConfigService
        )
        {
            _logger = logger;
            _speciesStageConfigService = speciesStageConfigService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllSpeciesStageConfigs()
        {
            try
            {
                var result = await _speciesStageConfigService.GetAllSpeciesStageConfigsAsync();
                return result.Type switch
                {
                    ResultType.Ok => Ok(new { result.Message, result.Data }),
                    _ => StatusCode(500, new { result.Message }),
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving species stage configs.");
                return StatusCode(500, new { Message = "Có lỗi xảy ra, vui lòng thử lại sau." });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetSpeciesStageConfigById(Guid id)
        {
            try
            {
                var result = await _speciesStageConfigService.GetSpeciesStageConfigById(id);
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
                    "An error occurred while retrieving species stage config with ID: {SpeciesStageConfigId}",
                    id
                );
                return StatusCode(500, new { Message = "Có lỗi xảy ra, vui lòng thử lại sau." });
            }
        }

        [Authorize(Roles = "Supervisor")]
        [HttpPost]
        public async Task<IActionResult> CreateSpeciesStageConfig(
            [FromBody] CreateSpeciesStageConfigDto dto
        )
        {
            try
            {
                var result = await _speciesStageConfigService.CreateSpeciesStageConfig(dto);
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
                _logger.LogError(ex, "An error occurred while creating species stage config");
                return StatusCode(500, new { Message = "Có lỗi xảy ra, vui lòng thử lại sau." });
            }
        }

        [Authorize(Roles = "Supervisor")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSpeciesStageConfig(
            Guid id,
            [FromBody] UpdateSpeciesStageConfigDto dto
        )
        {
            try
            {
                var result = await _speciesStageConfigService.UpdateSpeciesStageConfig(id, dto);
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
                    "An error occurred while updating species stage config with ID: {SpeciesStageConfigId}",
                    id
                );
                return StatusCode(500, new { Message = "Có lỗi xảy ra, vui lòng thử lại sau." });
            }
        }

        [Authorize(Roles = "Supervisor")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSpeciesStageConfig(Guid id)
        {
            try
            {
                var result = await _speciesStageConfigService.DeleteSpeciesStageConfig(id);
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
                    "An error occurred while deleting species stage config with ID: {SpeciesStageConfigId}",
                    id
                );
                return StatusCode(500, new { Message = "Có lỗi xảy ra, vui lòng thử lại sau." });
            }
        }
    }
}

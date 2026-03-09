using IRasRag.Application.Common.Models;
using IRasRag.Application.DTOs;
using IRasRag.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IRasRag.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/config")]
    public class ConfigController : ControllerBase
    {
        private readonly IGrowthStageService _growthStageService;
        private readonly ISpeciesThresholdService _speciesThresholdService;
        private readonly ISpeciesStageConfigService _speciesStageConfigService;
        private readonly ILogger<ConfigController> _logger;

        public ConfigController(
            IGrowthStageService growthStageService,
            ISpeciesThresholdService speciesThresholdService,
            ISpeciesStageConfigService speciesStageConfigService,
            ILogger<ConfigController> logger
        )
        {
            _growthStageService = growthStageService;
            _speciesThresholdService = speciesThresholdService;
            _speciesStageConfigService = speciesStageConfigService;
            _logger = logger;
        }

        // ─── Growth Stages ────────────────────────────────────────────────────────

        [Authorize(Roles = "Supervisor")]
        [HttpPost("growth-stages")]
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

        [HttpGet("growth-stages")]
        public async Task<IActionResult> GetAllGrowthStages(
            [FromQuery] GrowthStageListRequest request
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

                var result = await _growthStageService.GetAllGrowthStagesAsync(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving growth stages.");
                return StatusCode(500, new { Message = "Có lỗi xảy ra, vui lòng thử lại sau." });
            }
        }

        [Authorize(Roles = "Supervisor")]
        [HttpGet("growth-stages/{id}")]
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

        [Authorize(Roles = "Supervisor")]
        [HttpPut("growth-stages/{id}")]
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
                    "An error occurred while updating growth stage: {GrowthStageName}",
                    dto.Name
                );
                return StatusCode(500, new { Message = "Có lỗi xảy ra, vui lòng thử lại sau." });
            }
        }

        [Authorize(Roles = "Supervisor")]
        [HttpDelete("growth-stages/{id}")]
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

        // ─── Species Thresholds ───────────────────────────────────────────────────

        [HttpGet("thresholds")]
        public async Task<IActionResult> GetAllSpeciesThresholds(
            [FromQuery] SpeciesThresholdListRequest request
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

                var result = await _speciesThresholdService.GetAllSpeciesThresholdsAsync(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving species thresholds.");
                return StatusCode(500, new { Message = "Có lỗi xảy ra, vui lòng thử lại sau." });
            }
        }

        [HttpGet("thresholds/{id}")]
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

        [Authorize(Roles = "Supervisor")]
        [HttpPost("thresholds")]
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

        [Authorize(Roles = "Supervisor")]
        [HttpPut("thresholds/{id}")]
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

        [Authorize(Roles = "Supervisor")]
        [HttpDelete("thresholds/{id}")]
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

        // ─── Species Stage Configs ────────────────────────────────────────────────

        [HttpGet("species-stage-configs")]
        public async Task<IActionResult> GetAllSpeciesStageConfigs(
            [FromQuery] SpeciesStageConfigListRequest request
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

                var result = await _speciesStageConfigService.GetAllSpeciesStageConfigsAsync(
                    request
                );
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving species stage configs.");
                return StatusCode(500, new { Message = "Có lỗi xảy ra, vui lòng thử lại sau." });
            }
        }

        [HttpGet("species-stage-configs/{id}")]
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
        [HttpPost("species-stage-configs")]
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
        [HttpPut("species-stage-configs/{id}")]
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
        [HttpDelete("species-stage-configs/{id}")]
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

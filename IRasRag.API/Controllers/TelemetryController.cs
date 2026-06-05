using System.Security.Claims;
using IRasRag.Application.Common.Interfaces.Realtime;
using IRasRag.Application.Common.Interfaces.Simulation;
using IRasRag.Application.Common.Interfaces.Telemetry;
using IRasRag.Application.Common.Models.Realtime;
using IRasRag.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IRasRag.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/telemetry")]
    public class TelemetryController : ControllerBase
    {
        private readonly ITelemetryWindowService _window;
        private readonly IUserTankAccessService _tankAccess;
        private readonly ISimulationStateService _simulationState;
        private readonly ITelemetryCacheService _cache;
        private readonly ILiveDataNotifier _liveDataNotifier;
        private readonly ILatestTelemetryCacheService _latestTelemetryCache;
        private readonly ILogger<TelemetryController> _logger;

        public TelemetryController(
            ITelemetryWindowService window,
            IUserTankAccessService tankAccess,
            ISimulationStateService simulationState,
            ITelemetryCacheService cache,
            ILiveDataNotifier liveDataNotifier,
            ILatestTelemetryCacheService latestTelemetryCache,
            ILogger<TelemetryController> logger
        )
        {
            _window = window;
            _tankAccess = tankAccess;
            _simulationState = simulationState;
            _cache = cache;
            _liveDataNotifier = liveDataNotifier;
            _latestTelemetryCache = latestTelemetryCache;
            _logger = logger;
        }

        [HttpGet("tanks/{tankId}/window")]
        public async Task<IActionResult> GetWindow(Guid tankId)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!Guid.TryParse(userIdClaim, out var userId))
                return Unauthorized();

            if (!await _tankAccess.CanAccessTankAsync(userId, tankId))
                return Forbid();

            return Ok(_window.GetWindow(tankId));
        }

        /// <summary>
        /// Start simulation mode for a masterboard by its MAC address.
        /// While active, incoming MQTT readings for the temperature sensor are replaced with
        /// random dangerous values between 50-60°C. The chart continues updating at the normal rate.
        /// </summary>
        [HttpPost("simulate/{macAddress}/start")]
        public async Task<IActionResult> StartSimulation(string macAddress)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!Guid.TryParse(userIdClaim, out var userId))
                return Unauthorized();

            if (_simulationState.IsSimulating(macAddress))
            {
                return BadRequest(
                    new { Message = $"Simulation already active for MAC {macAddress}" }
                );
            }

            // Resolve masterboard by MAC
            var masterboard = await _cache.GetMasterboardByMacAsync(macAddress);
            if (masterboard == null)
            {
                return NotFound(new { Message = $"No masterboard found with MAC {macAddress}" });
            }

            // Check if user can access this tank
            if (!await _tankAccess.CanAccessTankAsync(userId, masterboard.FishTankId))
                return Forbid();

            // Start simulation — the TelemetryDispatchService will intercept incoming MQTT data
            // and replace temperature readings with random 50-60°C values.
            _simulationState.StartSimulation(macAddress);

            _logger.LogInformation(
                "Simulation started for MAC {MacAddress} (tank {TankId}). Incoming MQTT temperature readings will be replaced with random 50-60°C.",
                macAddress,
                masterboard.FishTankId
            );

            return Ok(
                new
                {
                    Message = $"Simulation started for {macAddress}. Temperature readings are now being replaced with random dangerous values (50-60°C).",
                }
            );
        }

        /// <summary>
        /// Stop simulation mode for a masterboard. Real MQTT data resumes.
        /// </summary>
        [HttpPost("simulate/{macAddress}/stop")]
        public async Task<IActionResult> StopSimulation(string macAddress)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!Guid.TryParse(userIdClaim, out var userId))
                return Unauthorized();

            if (!_simulationState.IsSimulating(macAddress))
            {
                return BadRequest(new { Message = $"No active simulation for MAC {macAddress}" });
            }

            // Resolve masterboard by MAC for authorization — if the masterboard was
            // deleted while simulation was active, still allow stopping (clean up the
            // orphaned simulation rather than leaving it stuck forever).
            var masterboard = await _cache.GetMasterboardByMacAsync(macAddress);
            if (masterboard != null)
            {
                if (!await _tankAccess.CanAccessTankAsync(userId, masterboard.FishTankId))
                    return Forbid();
            }

            // Stop simulation
            _simulationState.StopSimulation(macAddress);

            _logger.LogInformation(
                "Simulation stopped for MAC {MacAddress}. Real MQTT data will now resume.",
                macAddress
            );

            return Ok(
                new { Message = $"Simulation stopped for {macAddress}. Real MQTT data resumed." }
            );
        }

        /// <summary>
        /// Get simulation status for a MAC address.
        /// </summary>
        [HttpGet("simulate/{macAddress}")]
        public IActionResult GetSimulationStatus(string macAddress)
        {
            var isSimulating = _simulationState.IsSimulating(macAddress);
            return Ok(new { macAddress, isSimulating });
        }
    }
}

using System.Security.Claims;
using IRasRag.Application.Common.Interfaces.Telemetry;
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

        public TelemetryController(ITelemetryWindowService window, IUserTankAccessService tankAccess)
        {
            _window = window;
            _tankAccess = tankAccess;
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
    }
}

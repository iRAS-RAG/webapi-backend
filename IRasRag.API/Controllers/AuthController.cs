using IRasRag.Application.Common.Models;
using IRasRag.Application.DTOs.Auth;
using IRasRag.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace IRasRag.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            try
            {
                var response = await _authService.Login(request);
                return response.Type switch
                {
                    ResultType.BadRequest => BadRequest(new { response.Message }),
                    _ => Ok(new { Token = response.Data, response.Message }),
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "An error occurred during login for user: {UserName}",
                    request.UserName
                );
                return StatusCode(500, new { Message = "Có lỗi xảy ra, vui lòng thử lại sau." });
            }
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequest request)
        {
            try
            {
                var response = await _authService.Register(request);
                return response.Type switch
                {
                    ResultType.BadRequest => BadRequest(new { response.Message }),
                    _ => Ok(new { response.Message }),
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "An error occurred during registration for user: {UserName}",
                    request.UserName
                );
                return StatusCode(500, new { Message = "Có lỗi xảy ra, vui lòng thử lại sau." });
            }
        }
    }
}

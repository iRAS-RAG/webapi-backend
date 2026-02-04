using IRasRag.Application.Common.Models;
using IRasRag.Application.DTOs;
using IRasRag.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace IRasRag.API.Controllers
{
    [ApiController]
    [Route("api/auth")]
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
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
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
                    "An error occurred during login for user with email: {Email}",
                    request.Email
                );
                return StatusCode(500, new { Message = "Có lỗi xảy ra, vui lòng thử lại sau." });
            }
        }

        [HttpPost("request-password-reset")]
        public async Task<IActionResult> RequestPasswordReset(string email)
        {
            try
            {
                var response = await _authService.RequestPasswordReset(email);
                return response.Type switch
                {
                    ResultType.Unexpected => StatusCode(500, new { response.Message }),
                    _ => Ok(new { response.Message }),
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "An error occurred while requesting password reset for email: {Email}",
                    email
                );
                return StatusCode(500, new { Message = "Có lỗi xảy ra, vui lòng thử lại sau." });
            }
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
        {
            try
            {
                var response = await _authService.ResetPassword(request);
                return response.Type switch
                {
                    ResultType.Unauthorized => Unauthorized(new { response.Message }),
                    ResultType.BadRequest => BadRequest(new { response.Message }),
                    ResultType.Unexpected => StatusCode(500, new { response.Message }),
                    _ => Ok(new { response.Message }),
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "An error occurred while resetting password for email: {Email}",
                    request.Email
                );
                return StatusCode(500, new { Message = "Có lỗi xảy ra, vui lòng thử lại sau." });
            }
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout(string refreshToken)
        {
            try
            {
                var response = await _authService.Logout(refreshToken);
                return response.Type switch
                {
                    ResultType.Unexpected => StatusCode(500, new { response.Message }),
                    _ => Ok(new { response.Message }),
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during logout.");
                return StatusCode(500, new { Message = "Có lỗi xảy ra, vui lòng thử lại sau." });
            }
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshAccessToken(string refreshToken)
        {
            try
            {
                var response = await _authService.RefreshBothToken(refreshToken);
                return response.Type switch
                {
                    ResultType.Unauthorized => Unauthorized(new { response.Message }),
                    ResultType.BadRequest => BadRequest(new { response.Message }),
                    ResultType.Unexpected => StatusCode(500, new { response.Message }),
                    _ => Ok(new { Token = response.Data, response.Message }),
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while refreshing access token.");
                return StatusCode(500, new { Message = "Có lỗi xảy ra, vui lòng thử lại sau." });
            }
        }
    }
}

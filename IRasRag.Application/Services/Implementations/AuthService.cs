using System.Security.Cryptography;
using IRasRag.Application.Common.Interfaces.Auth;
using IRasRag.Application.Common.Interfaces.Email;
using IRasRag.Application.Common.Interfaces.Persistence;
using IRasRag.Application.Common.Models;
using IRasRag.Application.DTOs;
using IRasRag.Application.Services.Interfaces;
using IRasRag.Domain.Entities;
using IRasRag.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace IRasRag.Application.Services.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<AuthService> _logger;
        private readonly IHashingService _hasher;
        private readonly IJwtService _jwtService;
        private readonly IEmailService _emailService;

        public AuthService(
            IUnitOfWork unitOfWork,
            ILogger<AuthService> logger,
            IHashingService hasher,
            IJwtService jwtService,
            IEmailService emailService
        )
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _hasher = hasher;
            _jwtService = jwtService;
            _emailService = emailService;
        }

        public async Task<Result<TokenResponse>> Login(LoginRequest request)
        {
            try
            {
                var user = await _unitOfWork
                    .GetRepository<User>()
                    .FirstOrDefaultAsync(u => u.Email == request.Email);

                var isValidUser =
                    user != null && _hasher.VerifyPassword(request.Password, user.PasswordHash);

                if (!isValidUser)
                {
                    _logger.LogWarning(
                        $"Invalid login attempt for user with email: {request.Email}"
                    );
                    return Result<TokenResponse>.Failure(
                        "Sai tài khoản hoặc mật khẩu.",
                        ResultType.Unauthorized
                    );
                }

                var userRole = await _unitOfWork
                    .GetRepository<Role>()
                    .FirstOrDefaultAsync(r => r.Id == user.RoleId);

                if (userRole == null)
                {
                    _logger.LogError(
                        "Role not found for user {UserId} with RoleId {RoleId}",
                        user.Id,
                        user.RoleId
                    );
                    return Result<TokenResponse>.Failure(
                        "Đã xảy ra lỗi trong quá trình đăng nhập.",
                        ResultType.Unexpected
                    );
                }

                var token = _jwtService.GenerateAccessToken(user.Id, request.Email, userRole.Name);

                var refreshTokenResult = _jwtService.GenerateRefreshToken();

                var refreshToken = new RefreshToken
                {
                    TokenHash = _hasher.HashToken(refreshTokenResult.PlainToken),
                    UserId = user.Id,
                    ExpireDate = refreshTokenResult.ExpireDate,
                };

                await _unitOfWork.GetRepository<RefreshToken>().AddAsync(refreshToken);
                await _unitOfWork.SaveChangesAsync();

                return Result<TokenResponse>.Success(
                    new TokenResponse
                    {
                        AccessToken = token,
                        RefreshToken = refreshTokenResult.PlainToken,
                    },
                    "Đăng nhập thành công."
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Login transaction failed for user {Email}", request.Email);
                return Result<TokenResponse>.Failure(
                    "Đã xảy ra lỗi trong quá trình đăng nhập.",
                    ResultType.Unexpected
                );
            }
        }

        public async Task<Result> RequestPasswordReset(string email)
        {
            const int ResetCodeExpirationMinutes = 5;
            string? resetTokenKey = null;
            var user = await _unitOfWork
                .GetRepository<User>()
                .FirstOrDefaultAsync(u => u.Email == email);
            if (user != null)
            {
                await _unitOfWork.BeginTransactionAsync();
                try
                {
                    await ConsumeUserVerificationCodesAsync(
                        user.Id,
                        VerificationType.PasswordReset
                    );

                    resetTokenKey = RandomNumberGenerator.GetInt32(100000, 999999).ToString();
                    var resetToken = new Verification
                    {
                        UserId = user.Id,
                        CodeHash = _hasher.HashToken(resetTokenKey),
                        Type = VerificationType.PasswordReset,
                        ExpireDate = DateTime.UtcNow.AddMinutes(ResetCodeExpirationMinutes),
                    };

                    await _unitOfWork.GetRepository<Verification>().AddAsync(resetToken);
                    await _unitOfWork.CommitTransactionAsync();
                }
                catch (Exception dbEx)
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    _logger.LogError(
                        dbEx,
                        "Error occurred while creating password reset token for user with ID: {Id}",
                        user.Id
                    );
                    return Result.Failure(
                        "Đã xảy ra lỗi trong quá trình yêu cầu đặt lại mật khẩu.",
                        ResultType.Unexpected
                    );
                }

                try
                {
                    await _emailService.SendEmailAsync(
                        email,
                        "Yêu cầu đặt lại mật khẩu",
                        await _emailService.GenerateResetPasswordEmailBodyAsync(
                            resetTokenKey,
                            ResetCodeExpirationMinutes
                        )
                    );
                }
                catch (Exception emailEx)
                {
                    _logger.LogError(
                        emailEx,
                        "Failed to send password reset email to {Email}. Code was saved to database.",
                        email
                    );
                }
            }

            return Result.Success("Mã đặt lại mật khẩu sẽ được gửi nếu email có trong hệ thống.");
        }

        public async Task<Result> ResetPassword(ResetPasswordRequest request)
        {
            if (request.NewPassword != request.ConfirmNewPassword)
            {
                return Result.Failure(
                    "Mật khẩu mới và xác nhận mật khẩu không khớp.",
                    ResultType.BadRequest
                );
            }

            var user = await _unitOfWork
                .GetRepository<User>()
                .FirstOrDefaultAsync(u => u.Email == request.Email);

            if (user == null)
                return Result.Failure(
                    "Mã đặt lại mật khẩu không hợp lệ hoặc đã hết hạn.",
                    ResultType.Unauthorized
                );

            var existingCode = await _unitOfWork
                .GetRepository<Verification>()
                .FirstOrDefaultAsync(v =>
                    v.UserId == user.Id
                    && v.Type == VerificationType.PasswordReset
                    && v.ExpireDate > DateTime.UtcNow
                    && !v.IsConsumed
                );

            if (existingCode == null || !_hasher.VerifyToken(request.Code, existingCode.CodeHash))
                return Result.Failure(
                    "Mã đặt lại mật khẩu không hợp lệ hoặc đã hết hạn.",
                    ResultType.Unauthorized
                );

            await _unitOfWork.BeginTransactionAsync();
            try
            {
                user.PasswordHash = _hasher.HashPassword(request.NewPassword);
                existingCode.IsConsumed = true;
                await _unitOfWork.CommitTransactionAsync();
                return Result.Success("Mật khẩu đã được đặt lại thành công.");
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(
                    ex,
                    "Error occurred while resetting password for {Email}",
                    request.Email
                );
                return Result.Failure(
                    "Đã xảy ra lỗi trong quá trình đặt lại mật khẩu.",
                    ResultType.Unexpected
                );
            }
        }

        public async Task<Result<TokenResponse>> RefreshBothToken(string refreshToken)
        {
            var tokenHash = _hasher.HashToken(refreshToken);

            var storedToken = await _unitOfWork
                .GetRepository<RefreshToken>()
                .FirstOrDefaultAsync(rt => rt.TokenHash == tokenHash && !rt.IsRevoked);

            if (storedToken == null || storedToken.ExpireDate <= DateTime.UtcNow)
                return Result<TokenResponse>.Failure(
                    "Phiên đăng nhập không hợp lệ hoặc đã hết hạn, xin vui lòng đăng nhập lại.",
                    ResultType.Unauthorized
                );

            var user = await _unitOfWork
                .GetRepository<User>()
                .FirstOrDefaultAsync(u => u.Id == storedToken.UserId);

            if (user == null)
            {
                _logger.LogError(
                    "User not found for refresh token with ID: {TokenId}",
                    storedToken.Id
                );
                return Result<TokenResponse>.Failure(
                    "Đã xảy ra lỗi trong quá trình làm mới phiên đăng nhập.",
                    ResultType.Unexpected
                );
            }

            var userRole = await _unitOfWork
                .GetRepository<Role>()
                .FirstOrDefaultAsync(r => r.Id == user.RoleId);

            if (userRole == null)
            {
                _logger.LogError(
                    "Role not found for user {UserId} with RoleId {RoleId}",
                    user.Id,
                    user.RoleId
                );
                return Result<TokenResponse>.Failure(
                    "Đã xảy ra lỗi trong quá trình làm mới phiên đăng nhập.",
                    ResultType.Unexpected
                );
            }
            var newAccessToken = _jwtService.GenerateAccessToken(
                user.Id,
                user.Email,
                userRole.Name
            );

            await _unitOfWork.BeginTransactionAsync();
            try
            {
                storedToken.IsRevoked = true;
                var newRefreshTokenResult = _jwtService.GenerateRefreshToken();
                var newRefreshToken = new RefreshToken
                {
                    TokenHash = _hasher.HashToken(newRefreshTokenResult.PlainToken),
                    UserId = user.Id,
                    ExpireDate = newRefreshTokenResult.ExpireDate,
                };
                await _unitOfWork.GetRepository<RefreshToken>().AddAsync(newRefreshToken);
                await _unitOfWork.CommitTransactionAsync();

                return Result<TokenResponse>.Success(
                    new TokenResponse
                    {
                        AccessToken = newAccessToken,
                        RefreshToken = newRefreshTokenResult.PlainToken,
                    },
                    "Làm mới phiên đăng nhập thành công."
                );
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex, "Unexpected error occurred while refreshing access token.");
                return Result<TokenResponse>.Failure(
                    "Đã xảy ra lỗi trong quá trình làm mới phiên đăng nhập.",
                    ResultType.Unexpected
                );
            }
        }

        public async Task<Result> Logout(string token)
        {
            var hashedToken = _hasher.HashToken(token);
            var existingToken = await _unitOfWork
                .GetRepository<RefreshToken>()
                .FirstOrDefaultAsync(rt =>
                    rt.TokenHash == hashedToken && !rt.IsRevoked && rt.ExpireDate > DateTime.UtcNow
                );
            _logger.LogInformation("Client's hashed token: {HashedToken}", hashedToken);
            _logger.LogInformation("Server's existing token: {ExistingToken}", existingToken);
            if (existingToken != null)
            {
                existingToken.IsRevoked = true;
                _unitOfWork.GetRepository<RefreshToken>().Update(existingToken);
                await _unitOfWork.SaveChangesAsync();
            }
            return Result.Success("Đăng xuất thành công.");
        }

        private async Task ConsumeUserVerificationCodesAsync(Guid userId, VerificationType type)
        {
            var codes = await _unitOfWork
                .GetRepository<Verification>()
                .FindAllAsync(v => v.UserId == userId && v.Type == type && !v.IsConsumed);

            foreach (var code in codes)
                code.IsConsumed = true;
        }
    }
}

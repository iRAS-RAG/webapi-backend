using IRasRag.Application.Common.Interfaces;
using IRasRag.Application.Common.Models;
using IRasRag.Application.DTOs.Auth;
using IRasRag.Application.Services.Interfaces;
using IRasRag.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace IRasRag.Application.Services.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<AuthService> _logger;
        private readonly IPasswordHasher _hasher;
        private readonly IJwtService _jwtService;

        public AuthService(
            IUnitOfWork unitOfWork,
            ILogger<AuthService> logger,
            IPasswordHasher hasher,
            IJwtService jwtService
        )
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _hasher = hasher;
            _jwtService = jwtService;
        }

        public async Task<Result<string>> Login(LoginRequest request)
        {
            var normalizedUserName = request.UserName.Trim().ToLower();
            var user = await _unitOfWork
                .GetRepository<User>()
                .FirstOrDefaultAsync(u => normalizedUserName == u.UserName);

            var isValidUser =
                user != null && _hasher.VerifyPassword(request.Password, user.PasswordHash);

            if (!isValidUser)
            {
                _logger.LogWarning("Invalid login attempt for user: {UserName}", request.UserName);
                return Result<string>.Failure(
                    "Sai tài khoản hoặc mật khẩu.",
                    ResultType.Unauthorized
                );
            }

            var userRole = await _unitOfWork
                .GetRepository<Role>()
                .FirstOrDefaultAsync(r => r.Id == user.RoleId);

            var token = _jwtService.GenerateAccessToken(user.Id, normalizedUserName, userRole.Name);

            return Result<string>.Success(token, "Đăng nhập thành công.");
        }

        public async Task<Result<string>> Register(RegisterRequest request)
        {
            var normalizedUserName = request.UserName.Trim().ToLower();
            var isUserNameTaken = await _unitOfWork
                .GetRepository<User>()
                .AnyAsync(u => normalizedUserName == u.UserName);

            if (isUserNameTaken)
            {
                return Result<string>.Failure("Tên đăng nhập đã tồn tại.", ResultType.Conflict);
            }

            if (request.Password != request.ConfirmPassword)
            {
                return Result<string>.Failure(
                    "Mật khẩu và xác nhận mật khẩu không khớp.",
                    ResultType.BadRequest
                );
            }

            var passwordHash = _hasher.HashPassword(request.Password);
            var userRole = await _unitOfWork
                .GetRepository<Role>()
                .FirstOrDefaultAsync(r => r.Name == "User");
            var newUser = new User
            {
                UserName = normalizedUserName,
                FirstName = request.FirstName?.Trim(),
                LastName = request.LastName?.Trim(),
                PasswordHash = passwordHash,
                RoleId = userRole.Id,
            };

            await _unitOfWork.GetRepository<User>().AddAsync(newUser);
            await _unitOfWork.SaveChangesAsync();

            return Result<string>.Success(null, "Đăng ký thành công.");
        }
    }
}

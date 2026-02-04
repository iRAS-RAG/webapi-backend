using System.Linq.Expressions;
using FluentAssertions;
using IRasRag.Application.Common.Interfaces.Auth;
using IRasRag.Application.Common.Interfaces.Email;
using IRasRag.Application.Common.Interfaces.Persistence;
using IRasRag.Application.Common.Models;
using IRasRag.Application.Common.Models.Auth;
using IRasRag.Application.DTOs;
using IRasRag.Application.Services.Implementations;
using IRasRag.Domain.Entities;
using IRasRag.Domain.Enums;
using Microsoft.Extensions.Logging;
using Moq;

namespace IRasRag.Test.UnitTests.Application
{
    public class AuthServiceTest
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<ILogger<AuthService>> _loggerMock;
        private readonly Mock<IRepository<User>> _userRepositoryMock;
        private readonly Mock<IRepository<Role>> _roleRepositoryMock;
        private readonly Mock<IRepository<Verification>> _verificationRepositoryMock;
        private readonly Mock<IRepository<RefreshToken>> _refreshTokenRepositoryMock;
        private readonly Mock<IJwtService> _jwtServiceMock;
        private readonly Mock<IEmailService> _emailServiceMock;
        private readonly Mock<IHashingService> _hashingServiceMock;
        private readonly AuthService _sut;

        public AuthServiceTest()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _loggerMock = new Mock<ILogger<AuthService>>();
            _userRepositoryMock = new Mock<IRepository<User>>();
            _roleRepositoryMock = new Mock<IRepository<Role>>();
            _verificationRepositoryMock = new Mock<IRepository<Verification>>();
            _refreshTokenRepositoryMock = new Mock<IRepository<RefreshToken>>();
            _emailServiceMock = new Mock<IEmailService>();
            _jwtServiceMock = new Mock<IJwtService>();
            _hashingServiceMock = new Mock<IHashingService>();
            _unitOfWorkMock.Setup(u => u.GetRepository<User>()).Returns(_userRepositoryMock.Object);
            _unitOfWorkMock.Setup(u => u.GetRepository<Role>()).Returns(_roleRepositoryMock.Object);
            _unitOfWorkMock
                .Setup(u => u.GetRepository<Verification>())
                .Returns(_verificationRepositoryMock.Object);
            _unitOfWorkMock
                .Setup(u => u.GetRepository<RefreshToken>())
                .Returns(_refreshTokenRepositoryMock.Object);
            _sut = new AuthService(
                _unitOfWorkMock.Object,
                _loggerMock.Object,
                _hashingServiceMock.Object,
                _jwtServiceMock.Object,
                _emailServiceMock.Object
            );
        }

        #region Login Tests

        [Fact]
        public async Task Login_ShouldReturnOk_WhenSuccessful()
        {
            // Arrange
            var request = new LoginRequest
            {
                Email = "test@example.com",
                Password = "Password123!",
            };
            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = request.Email,
                PasswordHash = "hashedPassword",
                RoleId = Guid.NewGuid(),
            };

            _userRepositoryMock
                .Setup(r =>
                    r.FirstOrDefaultAsync(
                        It.IsAny<Expression<Func<User, bool>>>(),
                        It.IsAny<QueryType>()
                    )
                )
                .ReturnsAsync(user);

            _hashingServiceMock
                .Setup(h => h.VerifyPassword(request.Password, user.PasswordHash))
                .Returns(true);

            var role = new Role() { Id = user.RoleId, Name = "User" };

            _roleRepositoryMock
                .Setup(r =>
                    r.FirstOrDefaultAsync(
                        It.IsAny<Expression<Func<Role, bool>>>(),
                        It.IsAny<QueryType>()
                    )
                )
                .ReturnsAsync(role);

            _jwtServiceMock
                .Setup(j => j.GenerateAccessToken(user.Id, user.Email, role.Name))
                .Returns("mocked_jwt_token");

            var expireTime = DateTime.UtcNow.AddDays(7);
            _jwtServiceMock
                .Setup(j => j.GenerateRefreshToken())
                .Returns(new RefreshTokenResult("plain_refresh_token", expireTime));

            _hashingServiceMock
                .Setup(h => h.HashToken("plain_refresh_token"))
                .Returns("hashed_refresh_token");

            _refreshTokenRepositoryMock
                .Setup(r => r.AddAsync(It.IsAny<RefreshToken>()))
                .Returns(Task.CompletedTask);

            _unitOfWorkMock
                .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            var result = await _sut.Login(request);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Data.AccessToken.Should().Be("mocked_jwt_token");
            result.Data.RefreshToken.Should().Be("plain_refresh_token");
            result.Message.Should().Be("Đăng nhập thành công.");
            result.Type.Should().Be(ResultType.Ok);

            _userRepositoryMock.Verify(
                r =>
                    r.FirstOrDefaultAsync(
                        It.IsAny<Expression<Func<User, bool>>>(),
                        It.IsAny<QueryType>()
                    ),
                Times.Once
            );

            _hashingServiceMock.Verify(
                h => h.VerifyPassword(request.Password, user.PasswordHash),
                Times.Once
            );

            _roleRepositoryMock.Verify(
                r =>
                    r.FirstOrDefaultAsync(
                        It.IsAny<Expression<Func<Role, bool>>>(),
                        It.IsAny<QueryType>()
                    ),
                Times.Once
            );

            _jwtServiceMock.Verify(
                j => j.GenerateAccessToken(user.Id, request.Email, role.Name),
                Times.Once
            );

            _hashingServiceMock.Verify(h => h.HashToken("plain_refresh_token"), Times.Once);

            _hashingServiceMock.Verify(h => h.HashPassword(It.IsAny<string>()), Times.Never);

            _refreshTokenRepositoryMock.Verify(
                r =>
                    r.AddAsync(
                        It.Is<RefreshToken>(rt =>
                            rt.UserId == user.Id
                            && rt.TokenHash == "hashed_refresh_token"
                            && rt.ExpireDate == expireTime
                        )
                    ),
                Times.Once
            );

            _unitOfWorkMock.Verify(
                u => u.SaveChangesAsync(It.IsAny<CancellationToken>()),
                Times.Once
            );
        }

        [Fact]
        public async Task Login_ShouldReturnUnAuthorized_WhenPasswordIsInvalid()
        {
            var request = new LoginRequest
            {
                Email = "test@example.com",
                Password = "WrongPassword!",
            };

            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = request.Email,
                PasswordHash = "hashedPassword",
                RoleId = Guid.NewGuid(),
            };

            _userRepositoryMock
                .Setup(r =>
                    r.FirstOrDefaultAsync(
                        It.IsAny<Expression<Func<User, bool>>>(),
                        It.IsAny<QueryType>()
                    )
                )
                .ReturnsAsync(user);

            _hashingServiceMock
                .Setup(h => h.VerifyPassword(request.Password, user.PasswordHash))
                .Returns(false);

            var result = await _sut.Login(request);

            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.Data.Should().BeNull();
            result.Message.Should().Be("Sai tài khoản hoặc mật khẩu.");
            result.Type.Should().Be(ResultType.Unauthorized);

            _unitOfWorkMock.Verify(
                u => u.SaveChangesAsync(It.IsAny<CancellationToken>()),
                Times.Never
            );
            _refreshTokenRepositoryMock.Verify(
                r => r.AddAsync(It.IsAny<RefreshToken>()),
                Times.Never
            );
        }

        [Fact]
        public async Task Login_ShouldReturnUnAuthorized_WhenUserDoesNotExist()
        {
            var request = new LoginRequest { Email = "test@example.com", Password = "Password!" };

            _userRepositoryMock
                .Setup(r =>
                    r.FirstOrDefaultAsync(
                        It.IsAny<Expression<Func<User, bool>>>(),
                        It.IsAny<QueryType>()
                    )
                )
                .ReturnsAsync((User?)null);

            var result = await _sut.Login(request);

            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.Data.Should().BeNull();
            result.Message.Should().Be("Sai tài khoản hoặc mật khẩu.");
            result.Type.Should().Be(ResultType.Unauthorized);

            _hashingServiceMock.Verify(
                h => h.VerifyPassword(It.IsAny<string>(), It.IsAny<string>()),
                Times.Never
            );
        }

        [Fact]
        public async Task Login_ShouldFail_WhenRoleNotFound()
        {
            var request = new LoginRequest { Email = "test@example.com", Password = "Password!" };

            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = "test@example.com",
                PasswordHash = "hashed",
                RoleId = Guid.NewGuid(),
            };

            _userRepositoryMock
                .Setup(r =>
                    r.FirstOrDefaultAsync(
                        It.IsAny<Expression<Func<User, bool>>>(),
                        It.IsAny<QueryType>()
                    )
                )
                .ReturnsAsync(user);

            _hashingServiceMock
                .Setup(h => h.VerifyPassword(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(true);

            _roleRepositoryMock
                .Setup(r =>
                    r.FirstOrDefaultAsync(
                        It.IsAny<Expression<Func<Role, bool>>>(),
                        It.IsAny<QueryType>()
                    )
                )
                .ReturnsAsync((Role?)null);

            var result = await _sut.Login(request);

            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.Type.Should().Be(ResultType.Unexpected);
            result.Message.Should().Be("Đã xảy ra lỗi trong quá trình đăng nhập.");
            result.Data.Should().BeNull();

            _refreshTokenRepositoryMock.Verify(
                r => r.AddAsync(It.IsAny<RefreshToken>()),
                Times.Never
            );
            _unitOfWorkMock.Verify(
                u => u.SaveChangesAsync(It.IsAny<CancellationToken>()),
                Times.Never
            );
        }

        #endregion

        #region RequestPasswordReset Tests

        [Fact]
        public async Task RequestPasswordReset_ShouldReturnSuccess_WhenUserExists()
        {
            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = "test@example.com",
                PasswordHash = "hashedPassword",
                RoleId = Guid.NewGuid(),
            };

            _userRepositoryMock
                .Setup(r =>
                    r.FirstOrDefaultAsync(
                        It.IsAny<Expression<Func<User, bool>>>(),
                        It.IsAny<QueryType>()
                    )
                )
                .ReturnsAsync(user);

            _unitOfWorkMock
                .Setup(u => u.BeginTransactionAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            _verificationRepositoryMock
                .Setup(r =>
                    r.FindAllAsync(
                        It.IsAny<Expression<Func<Verification, bool>>>(),
                        It.IsAny<QueryType>()
                    )
                )
                .ReturnsAsync(Enumerable.Empty<Verification>());

            _verificationRepositoryMock
                .Setup(r => r.AddAsync(It.IsAny<Verification>()))
                .Returns(Task.CompletedTask);

            _unitOfWorkMock
                .Setup(u => u.CommitTransactionAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            _emailServiceMock
                .Setup(e => e.SendEmailAsync(user.Email, It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            var result = await _sut.RequestPasswordReset(user.Email);

            result.Should().NotBeNull();
            result
                .Message.Should()
                .Be("Mã đặt lại mật khẩu sẽ được gửi nếu email có trong hệ thống.");
            result.IsSuccess.Should().BeTrue();
            result.Type.Should().Be(ResultType.Ok);

            _verificationRepositoryMock.Verify(
                r =>
                    r.AddAsync(
                        It.Is<Verification>(v =>
                            v.UserId == user.Id
                            && v.Type == VerificationType.PasswordReset
                            && v.IsConsumed == false
                        )
                    ),
                Times.Once
            );

            _unitOfWorkMock.Verify(
                u => u.CommitTransactionAsync(It.IsAny<CancellationToken>()),
                Times.Once
            );
        }

        [Fact]
        public async Task RequestPasswordReset_ShouldReturnSuccess_WhenUserDoesNotExist()
        {
            _userRepositoryMock
                .Setup(r =>
                    r.FirstOrDefaultAsync(
                        It.IsAny<Expression<Func<User, bool>>>(),
                        It.IsAny<QueryType>()
                    )
                )
                .ReturnsAsync((User?)null);

            var result = await _sut.RequestPasswordReset("missing@example.com");

            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result
                .Message.Should()
                .Be("Mã đặt lại mật khẩu sẽ được gửi nếu email có trong hệ thống.");
            result.Type.Should().Be(ResultType.Ok);

            _unitOfWorkMock.Verify(
                u => u.BeginTransactionAsync(It.IsAny<CancellationToken>()),
                Times.Never
            );
            _verificationRepositoryMock.Verify(
                r => r.AddAsync(It.IsAny<Verification>()),
                Times.Never
            );
            _emailServiceMock.Verify(
                e => e.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()),
                Times.Never
            );
        }

        [Fact]
        public async Task RequestPasswordReset_ShouldRollback_WhenDatabaseFails()
        {
            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = "test@example.com",
                PasswordHash = "hashedPassword",
                RoleId = Guid.NewGuid(),
            };

            _userRepositoryMock
                .Setup(r =>
                    r.FirstOrDefaultAsync(
                        It.IsAny<Expression<Func<User, bool>>>(),
                        It.IsAny<QueryType>()
                    )
                )
                .ReturnsAsync(user);

            _verificationRepositoryMock
                .Setup(r =>
                    r.FindAllAsync(
                        It.IsAny<Expression<Func<Verification, bool>>>(),
                        It.IsAny<QueryType>()
                    )
                )
                .ThrowsAsync(new Exception(""));

            var result = await _sut.RequestPasswordReset(user.Email);

            result.IsSuccess.Should().BeFalse();
            result.Type.Should().Be(ResultType.Unexpected);
            result.Message.Should().Be("Đã xảy ra lỗi trong quá trình yêu cầu đặt lại mật khẩu.");

            _unitOfWorkMock.Verify(
                u => u.RollbackTransactionAsync(It.IsAny<CancellationToken>()),
                Times.Once
            );
        }

        [Fact]
        public async Task RequestPasswordReset_ShouldConsumeExistingVerificationCodes()
        {
            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = "test@example.com",
                PasswordHash = "hashedPassword",
                RoleId = Guid.NewGuid(),
            };

            var existingCodes = new List<Verification>
            {
                new()
                {
                    UserId = user.Id,
                    Type = VerificationType.PasswordReset,
                    IsConsumed = false,
                },
                new()
                {
                    UserId = user.Id,
                    Type = VerificationType.PasswordReset,
                    IsConsumed = false,
                },
            };

            _userRepositoryMock
                .Setup(r =>
                    r.FirstOrDefaultAsync(
                        It.IsAny<Expression<Func<User, bool>>>(),
                        It.IsAny<QueryType>()
                    )
                )
                .ReturnsAsync(user);

            _verificationRepositoryMock
                .Setup(r =>
                    r.FindAllAsync(
                        It.IsAny<Expression<Func<Verification, bool>>>(),
                        It.IsAny<QueryType>()
                    )
                )
                .ReturnsAsync(existingCodes);

            await _sut.RequestPasswordReset(user.Email);

            existingCodes.All(c => c.IsConsumed).Should().BeTrue();
        }

        #endregion

        #region ResetPassword Tests

        [Fact]
        public async Task ResetPassword_ShouldReturnBadRequest_WhenPasswordsDoNotMatch()
        {
            var request = new ResetPasswordRequest
            {
                Email = "test@example.com",
                Code = "123456",
                NewPassword = "NewPassword123!",
                ConfirmNewPassword = "NewPassword1234!",
            };

            var result = await _sut.ResetPassword(request);

            result.IsSuccess.Should().BeFalse();
            result.Type.Should().Be(ResultType.BadRequest);
            result.Message.Should().Be("Mật khẩu mới và xác nhận mật khẩu không khớp.");

            _unitOfWorkMock.Verify(
                u => u.BeginTransactionAsync(It.IsAny<CancellationToken>()),
                Times.Never
            );
        }

        [Fact]
        public async Task ResetPassword_ShouldFail_WhenUserNotFound()
        {
            var request = new ResetPasswordRequest
            {
                Email = "test@example.com",
                Code = "123456",
                NewPassword = "NewPassword123!",
                ConfirmNewPassword = "NewPassword123!",
            };

            _userRepositoryMock
                .Setup(r =>
                    r.FirstOrDefaultAsync(
                        It.IsAny<Expression<Func<User, bool>>>(),
                        It.IsAny<QueryType>()
                    )
                )
                .ReturnsAsync((User?)null);

            _unitOfWorkMock
                .Setup(u => u.BeginTransactionAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var result = await _sut.ResetPassword(request);

            result.IsSuccess.Should().BeFalse();
            result.Type.Should().Be(ResultType.Unauthorized);

            _unitOfWorkMock.Verify(
                u => u.CommitTransactionAsync(It.IsAny<CancellationToken>()),
                Times.Never
            );
        }

        [Fact]
        public async Task ResetPassword_ShouldFail_WhenVerificationCodeNotFound()
        {
            var request = new ResetPasswordRequest
            {
                Email = "test@example.com",
                Code = "123456",
                NewPassword = "NewPassword123!",
                ConfirmNewPassword = "NewPassword123!",
            };

            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = request.Email,
                PasswordHash = "hashedPassword",
                RoleId = Guid.NewGuid(),
            };

            _userRepositoryMock
                .Setup(r =>
                    r.FirstOrDefaultAsync(
                        It.IsAny<Expression<Func<User, bool>>>(),
                        It.IsAny<QueryType>()
                    )
                )
                .ReturnsAsync(user);

            _verificationRepositoryMock
                .Setup(r =>
                    r.FirstOrDefaultAsync(
                        It.IsAny<Expression<Func<Verification, bool>>>(),
                        It.IsAny<QueryType>()
                    )
                )
                .ReturnsAsync((Verification?)null);

            _unitOfWorkMock
                .Setup(u => u.BeginTransactionAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var result = await _sut.ResetPassword(request);

            result.IsSuccess.Should().BeFalse();
            result.Type.Should().Be(ResultType.Unauthorized);
        }

        [Fact]
        public async Task ResetPassword_ShouldFail_WhenCodeIsInvalid()
        {
            var request = new ResetPasswordRequest
            {
                Email = "test@example.com",
                Code = "123456",
                NewPassword = "NewPassword123!",
                ConfirmNewPassword = "NewPassword123!",
            };

            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = request.Email,
                PasswordHash = "hashedPassword",
                RoleId = Guid.NewGuid(),
            };

            var verification = new Verification
            {
                UserId = user.Id,
                CodeHash = "hashed_code",
                Type = VerificationType.PasswordReset,
                ExpireDate = DateTime.UtcNow.AddMinutes(5),
                IsConsumed = false,
            };

            _userRepositoryMock
                .Setup(r =>
                    r.FirstOrDefaultAsync(
                        It.IsAny<Expression<Func<User, bool>>>(),
                        It.IsAny<QueryType>()
                    )
                )
                .ReturnsAsync(user);

            _verificationRepositoryMock
                .Setup(r =>
                    r.FirstOrDefaultAsync(
                        It.IsAny<Expression<Func<Verification, bool>>>(),
                        It.IsAny<QueryType>()
                    )
                )
                .ReturnsAsync(verification);

            _hashingServiceMock
                .Setup(h => h.VerifyPassword(request.Code, verification.CodeHash))
                .Returns(false);

            _unitOfWorkMock
                .Setup(u => u.BeginTransactionAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var result = await _sut.ResetPassword(request);

            result.IsSuccess.Should().BeFalse();
            result.Type.Should().Be(ResultType.Unauthorized);
        }

        [Fact]
        public async Task ResetPassword_ShouldSucceed_WhenValid()
        {
            var request = new ResetPasswordRequest
            {
                Email = "test@example.com",
                Code = "123456",
                NewPassword = "NewPassword123!",
                ConfirmNewPassword = "NewPassword123!",
            };

            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = request.Email,
                PasswordHash = "hashedPassword",
                RoleId = Guid.NewGuid(),
            };

            var verification = new Verification
            {
                UserId = user.Id,
                CodeHash = "hashed_code",
                Type = VerificationType.PasswordReset,
                ExpireDate = DateTime.UtcNow.AddMinutes(5),
                IsConsumed = false,
            };

            _userRepositoryMock
                .Setup(r =>
                    r.FirstOrDefaultAsync(
                        It.IsAny<Expression<Func<User, bool>>>(),
                        It.IsAny<QueryType>()
                    )
                )
                .ReturnsAsync(user);

            _verificationRepositoryMock
                .Setup(r =>
                    r.FirstOrDefaultAsync(
                        It.IsAny<Expression<Func<Verification, bool>>>(),
                        It.IsAny<QueryType>()
                    )
                )
                .ReturnsAsync(verification);

            _hashingServiceMock
                .Setup(h => h.VerifyToken(request.Code, verification.CodeHash))
                .Returns(true);

            _hashingServiceMock
                .Setup(h => h.HashPassword(request.NewPassword))
                .Returns("new_password_hash");

            _unitOfWorkMock
                .Setup(u => u.BeginTransactionAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            _unitOfWorkMock
                .Setup(u => u.CommitTransactionAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var result = await _sut.ResetPassword(request);

            result.IsSuccess.Should().BeTrue();
            result.Message.Should().Be("Mật khẩu đã được đặt lại thành công.");

            user.PasswordHash.Should().Be("new_password_hash");
            verification.IsConsumed.Should().BeTrue();

            _unitOfWorkMock.Verify(
                u => u.CommitTransactionAsync(It.IsAny<CancellationToken>()),
                Times.Once
            );

            _hashingServiceMock.Verify(h => h.HashToken(request.NewPassword), Times.Never);

            _hashingServiceMock.Verify(
                h => h.VerifyToken(request.Code, verification.CodeHash),
                Times.Once
            );

            _hashingServiceMock.Verify(h => h.HashPassword("NewPassword123!"), Times.Once);

            _hashingServiceMock.Verify(
                h => h.VerifyPassword(It.IsAny<string>(), It.IsAny<string>()),
                Times.Never
            );
        }

        #endregion

        #region RefreshBothToken Tests

        [Fact]
        public async Task RefreshBothToken_ShouldReturnUnauthorized_WhenTokenNotFound()
        {
            // Arrange
            var rawToken = "refresh_token";
            var hash = "hashed_token";

            _hashingServiceMock.Setup(h => h.HashPassword(rawToken)).Returns(hash);

            _refreshTokenRepositoryMock
                .Setup(r =>
                    r.FirstOrDefaultAsync(
                        It.IsAny<Expression<Func<RefreshToken, bool>>>(),
                        It.IsAny<QueryType>()
                    )
                )
                .ReturnsAsync((RefreshToken?)null);

            // Act
            var result = await _sut.RefreshBothToken(rawToken);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Type.Should().Be(ResultType.Unauthorized);
            result
                .Message.Should()
                .Be("Phiên đăng nhập không hợp lệ hoặc đã hết hạn, xin vui lòng đăng nhập lại.");
        }

        [Fact]
        public async Task RefreshBothToken_ShouldReturnUnauthorized_WhenTokenExpired()
        {
            var rawToken = "refresh_token";
            var hash = "hashed_token";

            var expiredToken = new RefreshToken
            {
                TokenHash = hash,
                IsRevoked = false,
                ExpireDate = DateTime.UtcNow.AddMinutes(-1),
            };

            _hashingServiceMock.Setup(h => h.HashPassword(rawToken)).Returns(hash);

            _refreshTokenRepositoryMock
                .Setup(r =>
                    r.FirstOrDefaultAsync(
                        It.IsAny<Expression<Func<RefreshToken, bool>>>(),
                        It.IsAny<QueryType>()
                    )
                )
                .ReturnsAsync(expiredToken);

            var result = await _sut.RefreshBothToken(rawToken);

            result.Type.Should().Be(ResultType.Unauthorized);
            result.IsSuccess.Should().BeFalse();
            result
                .Message.Should()
                .Be("Phiên đăng nhập không hợp lệ hoặc đã hết hạn, xin vui lòng đăng nhập lại.");
        }

        [Fact]
        public async Task RefreshBothToken_ShouldReturnUnexpected_WhenUserNotFound()
        {
            var rawToken = "refresh_token";
            var hash = "hashed_token";
            var storedToken = new RefreshToken
            {
                Id = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                TokenHash = hash,
                IsRevoked = false,
                ExpireDate = DateTime.UtcNow.AddDays(1),
            };

            _hashingServiceMock.Setup(h => h.HashPassword(rawToken)).Returns(hash);

            _refreshTokenRepositoryMock
                .Setup(r =>
                    r.FirstOrDefaultAsync(
                        It.IsAny<Expression<Func<RefreshToken, bool>>>(),
                        It.IsAny<QueryType>()
                    )
                )
                .ReturnsAsync(storedToken);

            _userRepositoryMock
                .Setup(r =>
                    r.FirstOrDefaultAsync(
                        It.IsAny<Expression<Func<User, bool>>>(),
                        It.IsAny<QueryType>()
                    )
                )
                .ReturnsAsync((User?)null);

            var result = await _sut.RefreshBothToken(rawToken);

            result.Type.Should().Be(ResultType.Unexpected);
            result.IsSuccess.Should().BeFalse();
            result.Message.Should().Be("Đã xảy ra lỗi trong quá trình làm mới phiên đăng nhập.");
        }

        [Fact]
        public async Task RefreshBothToken_ShouldReturnUnexpected_WhenRoleNotFound()
        {
            var rawToken = "refresh_token";
            var hash = "hashed_token";
            var roleId = Guid.NewGuid();
            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = "test@example.com",
                PasswordHash = "hashedPassword",
                RoleId = roleId,
            };

            var storedToken = new RefreshToken
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                TokenHash = hash,
                IsRevoked = false,
                ExpireDate = DateTime.UtcNow.AddDays(1),
            };

            _hashingServiceMock.Setup(h => h.HashPassword(rawToken)).Returns(hash);

            _refreshTokenRepositoryMock
                .Setup(r =>
                    r.FirstOrDefaultAsync(
                        It.IsAny<Expression<Func<RefreshToken, bool>>>(),
                        It.IsAny<QueryType>()
                    )
                )
                .ReturnsAsync(storedToken);

            _userRepositoryMock
                .Setup(r =>
                    r.FirstOrDefaultAsync(
                        It.IsAny<Expression<Func<User, bool>>>(),
                        It.IsAny<QueryType>()
                    )
                )
                .ReturnsAsync(user);

            _roleRepositoryMock
                .Setup(r =>
                    r.FirstOrDefaultAsync(
                        It.IsAny<Expression<Func<Role, bool>>>(),
                        It.IsAny<QueryType>()
                    )
                )
                .ReturnsAsync((Role?)null);

            var result = await _sut.RefreshBothToken(rawToken);

            result.Type.Should().Be(ResultType.Unexpected);
            result.IsSuccess.Should().BeFalse();
            result.Message.Should().Be("Đã xảy ra lỗi trong quá trình làm mới phiên đăng nhập.");
        }

        [Fact]
        public async Task RefreshBothToken_ShouldReturnNewTokens_WhenSuccessful()
        {
            var rawToken = "refresh_token";
            var oldHash = "old_hash";
            var newHash = "new_hash";
            var role = new Role { Id = Guid.NewGuid(), Name = "User" };

            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = "test@example.com",
                PasswordHash = "hashedPassword",
                RoleId = role.Id,
            };

            var storedToken = new RefreshToken
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                TokenHash = oldHash,
                IsRevoked = false,
                ExpireDate = DateTime.UtcNow.AddDays(1),
            };

            var newStoredToken = new RefreshToken
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                TokenHash = newHash,
                IsRevoked = false,
                ExpireDate = DateTime.UtcNow.AddDays(1),
            };

            var refreshTokenResult = new RefreshTokenResult(
                "new_refresh",
                DateTime.UtcNow.AddDays(1)
            );

            _hashingServiceMock.Setup(h => h.HashToken(rawToken)).Returns(oldHash);

            _refreshTokenRepositoryMock
                .Setup(r =>
                    r.FirstOrDefaultAsync(
                        It.IsAny<Expression<Func<RefreshToken, bool>>>(),
                        It.IsAny<QueryType>()
                    )
                )
                .ReturnsAsync(storedToken);

            _userRepositoryMock
                .Setup(r =>
                    r.FirstOrDefaultAsync(
                        It.IsAny<Expression<Func<User, bool>>>(),
                        It.IsAny<QueryType>()
                    )
                )
                .ReturnsAsync(user);

            _roleRepositoryMock
                .Setup(r =>
                    r.FirstOrDefaultAsync(
                        It.IsAny<Expression<Func<Role, bool>>>(),
                        It.IsAny<QueryType>()
                    )
                )
                .ReturnsAsync(role);

            _jwtServiceMock
                .Setup(j => j.GenerateAccessToken(user.Id, user.Email, role.Name))
                .Returns("new_access");

            _unitOfWorkMock
                .Setup(u => u.BeginTransactionAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            _jwtServiceMock.Setup(j => j.GenerateRefreshToken()).Returns(refreshTokenResult);

            _hashingServiceMock
                .Setup(h => h.HashToken(refreshTokenResult.PlainToken))
                .Returns(newHash);

            _refreshTokenRepositoryMock
                .Setup(r => r.AddAsync(It.IsAny<RefreshToken>()))
                .Returns(Task.CompletedTask);

            _unitOfWorkMock
                .Setup(u => u.CommitTransactionAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var result = await _sut.RefreshBothToken(rawToken);

            result.IsSuccess.Should().BeTrue();
            result.Data.AccessToken.Should().Be("new_access");
            result.Data.RefreshToken.Should().Be("new_refresh");
            storedToken.IsRevoked.Should().BeTrue();

            _unitOfWorkMock.Verify(
                u => u.CommitTransactionAsync(It.IsAny<CancellationToken>()),
                Times.Once
            );
            _refreshTokenRepositoryMock.Verify(
                r =>
                    r.AddAsync(
                        It.Is<RefreshToken>(rt =>
                            rt.UserId == user.Id && rt.TokenHash == newHash && rt.IsRevoked == false
                        )
                    ),
                Times.Once
            );
            _hashingServiceMock.Verify(h => h.HashToken(rawToken), Times.Once);
            _hashingServiceMock.Verify(h => h.HashPassword(It.IsAny<string>()), Times.Never);
            _hashingServiceMock.Verify(
                h => h.VerifyPassword(It.IsAny<string>(), It.IsAny<string>()),
                Times.Never
            );
            _hashingServiceMock.Verify(h => h.HashToken(refreshTokenResult.PlainToken), Times.Once);
        }

        #endregion

        #region Logout Tests

        [Fact]
        public async Task Logout_ShouldRevokeToken_AndSaveChanges_WhenValidTokenExists()
        {
            // Arrange
            var plainToken = "refresh_token";
            var hashedToken = "hashed_refresh_token";

            var refreshToken = new RefreshToken
            {
                TokenHash = hashedToken,
                IsRevoked = false,
                ExpireDate = DateTime.UtcNow.AddMinutes(10),
            };

            _hashingServiceMock.Setup(h => h.HashToken(plainToken)).Returns(hashedToken);

            _refreshTokenRepositoryMock
                .Setup(r =>
                    r.FirstOrDefaultAsync(
                        It.IsAny<Expression<Func<RefreshToken, bool>>>(),
                        It.IsAny<QueryType>()
                    )
                )
                .ReturnsAsync(refreshToken);

            // Act
            var result = await _sut.Logout(plainToken);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Message.Should().Be("Đăng xuất thành công.");

            refreshToken.IsRevoked.Should().BeTrue();

            _unitOfWorkMock.Verify(
                u => u.SaveChangesAsync(It.IsAny<CancellationToken>()),
                Times.Once
            );

            _refreshTokenRepositoryMock.Verify(r => r.Update(refreshToken), Times.Once);

            _hashingServiceMock.Verify(h => h.HashToken(plainToken), Times.Once);

            _hashingServiceMock.Verify(h => h.HashPassword(It.IsAny<string>()), Times.Never);

            _refreshTokenRepositoryMock.Verify(
                r =>
                    r.FirstOrDefaultAsync(
                        It.IsAny<Expression<Func<RefreshToken, bool>>>(),
                        It.IsAny<QueryType>()
                    ),
                Times.Once
            );
        }

        [Fact]
        public async Task Logout_ShouldNotSaveChanges_WhenTokenDoesNotExist()
        {
            // Arrange
            var plainToken = "refresh_token";
            var hashedToken = "hashed_refresh_token";

            _hashingServiceMock.Setup(h => h.HashToken(plainToken)).Returns(hashedToken);

            _refreshTokenRepositoryMock
                .Setup(r =>
                    r.FirstOrDefaultAsync(
                        It.IsAny<Expression<Func<RefreshToken, bool>>>(),
                        It.IsAny<QueryType>()
                    )
                )
                .ReturnsAsync((RefreshToken)null);

            // Act
            var result = await _sut.Logout(plainToken);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Message.Should().Be("Đăng xuất thành công.");

            _unitOfWorkMock.Verify(
                u => u.SaveChangesAsync(It.IsAny<CancellationToken>()),
                Times.Never
            );

            _refreshTokenRepositoryMock.Verify(
                r => r.Update(It.IsAny<RefreshToken>()),
                Times.Never
            );

            _hashingServiceMock.Verify(h => h.HashToken(plainToken), Times.Once);

            _hashingServiceMock.Verify(h => h.HashPassword(It.IsAny<string>()), Times.Never);
        }

        #endregion
    }
}

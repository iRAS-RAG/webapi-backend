using System.Linq.Expressions;
using FluentAssertions;
using IRasRag.Application.Common.Interfaces.Auth;
using IRasRag.Application.Common.Interfaces.BackgroundJobs;
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
        private readonly Mock<IBackgroundJobService> _backgroundJobServiceMock;
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
            _backgroundJobServiceMock = new Mock<IBackgroundJobService>();

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
                _emailServiceMock.Object,
                _backgroundJobServiceMock.Object
            );
        }

        #region Login Tests

        [Fact]
        public async Task Login_ShouldReturnSuccess_WhenCredentialsAreValid()
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
            var role = new Role { Id = user.RoleId, Name = "User" };

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
                .Returns("access_token");
            _jwtServiceMock
                .Setup(j => j.GenerateRefreshToken())
                .Returns(new RefreshTokenResult("refresh_token", DateTime.UtcNow.AddDays(7)));
            _hashingServiceMock.Setup(h => h.HashToken("refresh_token")).Returns("hashed_refresh");

            // Act
            var result = await _sut.Login(request);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Data.AccessToken.Should().Be("access_token");
            result.Data.RefreshToken.Should().Be("refresh_token");
            _refreshTokenRepositoryMock.Verify(
                r => r.AddAsync(It.IsAny<RefreshToken>()),
                Times.Once
            );
            _unitOfWorkMock.Verify(
                u => u.SaveChangesAsync(It.IsAny<CancellationToken>()),
                Times.Once
            );

            // Critical: Verify correct hashing methods used
            _hashingServiceMock.Verify(h => h.HashToken("refresh_token"), Times.Once);
            _hashingServiceMock.Verify(h => h.HashPassword(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task Login_ShouldReturnUnauthorized_WhenPasswordIsInvalid()
        {
            // Arrange
            var request = new LoginRequest
            {
                Email = "test@example.com",
                Password = "WrongPassword",
            };
            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = request.Email,
                PasswordHash = "hashedPassword",
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

            // Act
            var result = await _sut.Login(request);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Type.Should().Be(ResultType.Unauthorized);
            _unitOfWorkMock.Verify(
                u => u.SaveChangesAsync(It.IsAny<CancellationToken>()),
                Times.Never
            );
        }

        [Fact]
        public async Task Login_ShouldReturnUnauthorized_WhenUserDoesNotExist()
        {
            // Arrange
            var request = new LoginRequest { Email = "test@example.com", Password = "Password!" };

            _userRepositoryMock
                .Setup(r =>
                    r.FirstOrDefaultAsync(
                        It.IsAny<Expression<Func<User, bool>>>(),
                        It.IsAny<QueryType>()
                    )
                )
                .ReturnsAsync((User?)null);

            // Act
            var result = await _sut.Login(request);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Type.Should().Be(ResultType.Unauthorized);
            _hashingServiceMock.Verify(
                h => h.VerifyPassword(It.IsAny<string>(), It.IsAny<string>()),
                Times.Never
            );
        }

        [Fact]
        public async Task Login_ShouldReturnUnexpected_WhenRoleNotFound()
        {
            // Arrange
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

            // Act
            var result = await _sut.Login(request);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Type.Should().Be(ResultType.Unexpected);
            _unitOfWorkMock.Verify(
                u => u.SaveChangesAsync(It.IsAny<CancellationToken>()),
                Times.Never
            );
        }

        [Fact]
        public async Task Login_ShouldReturnUnexpected_WhenSaveChangesFails()
        {
            // Arrange
            var request = new LoginRequest { Email = "test@example.com", Password = "Password!" };
            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = request.Email,
                PasswordHash = "hashed",
                RoleId = Guid.NewGuid(),
            };
            var role = new Role { Id = user.RoleId, Name = "User" };

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
                .ReturnsAsync(role);
            _jwtServiceMock
                .Setup(j =>
                    j.GenerateAccessToken(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>())
                )
                .Returns("token");
            _jwtServiceMock
                .Setup(j => j.GenerateRefreshToken())
                .Returns(new RefreshTokenResult("refresh", DateTime.UtcNow.AddDays(1)));
            _hashingServiceMock.Setup(h => h.HashToken(It.IsAny<string>())).Returns("hash");
            _unitOfWorkMock
                .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _sut.Login(request);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Type.Should().Be(ResultType.Unexpected);
        }

        #endregion

        #region RequestPasswordReset Tests

        [Fact]
        public async Task RequestPasswordReset_ShouldReturnSuccess_WhenUserExists()
        {
            // Arrange
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
                .ReturnsAsync(Array.Empty<Verification>());
            _emailServiceMock
                .Setup(e =>
                    e.GenerateResetPasswordEmailBodyAsync(It.IsAny<string>(), It.IsAny<int>())
                )
                .ReturnsAsync("email_body");

            // Act
            var result = await _sut.RequestPasswordReset(user.Email);

            // Assert
            result.IsSuccess.Should().BeTrue();
            _verificationRepositoryMock.Verify(
                r =>
                    r.AddAsync(
                        It.Is<Verification>(v =>
                            v.UserId == user.Id
                            && v.Type == VerificationType.PasswordReset
                            && !v.IsConsumed
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
        public async Task RequestPasswordReset_ShouldReturnSuccess_WhenUserDoesNotExist()
        {
            // Arrange
            _userRepositoryMock
                .Setup(r =>
                    r.FirstOrDefaultAsync(
                        It.IsAny<Expression<Func<User, bool>>>(),
                        It.IsAny<QueryType>()
                    )
                )
                .ReturnsAsync((User?)null);

            // Act
            var result = await _sut.RequestPasswordReset("missing@example.com");

            // Assert
            result.IsSuccess.Should().BeTrue();
            _verificationRepositoryMock.Verify(
                r => r.AddAsync(It.IsAny<Verification>()),
                Times.Never
            );
            _unitOfWorkMock.Verify(
                u => u.SaveChangesAsync(It.IsAny<CancellationToken>()),
                Times.Never
            );
        }

        [Fact]
        public async Task RequestPasswordReset_ShouldReturnFailure_WhenDatabaseFails()
        {
            // Arrange
            var user = new User { Id = Guid.NewGuid(), Email = "test@example.com" };

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
                .ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _sut.RequestPasswordReset(user.Email);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Type.Should().Be(ResultType.Unexpected);
        }

        [Fact]
        public async Task RequestPasswordReset_ShouldConsumeExistingCodes()
        {
            // Arrange
            var user = new User { Id = Guid.NewGuid(), Email = "test@example.com" };
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
            _emailServiceMock
                .Setup(e =>
                    e.GenerateResetPasswordEmailBodyAsync(It.IsAny<string>(), It.IsAny<int>())
                )
                .ReturnsAsync("email_body");

            // Act
            await _sut.RequestPasswordReset(user.Email);

            // Assert
            existingCodes.Should().AllSatisfy(c => c.IsConsumed.Should().BeTrue());
        }

        #endregion

        #region ResetPassword Tests

        [Fact]
        public async Task ResetPassword_ShouldReturnBadRequest_WhenPasswordsDoNotMatch()
        {
            // Arrange
            var request = new ResetPasswordRequest
            {
                Email = "test@example.com",
                Code = "123456",
                NewPassword = "Password1",
                ConfirmNewPassword = "Password2",
            };

            // Act
            var result = await _sut.ResetPassword(request);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Type.Should().Be(ResultType.BadRequest);
        }

        [Fact]
        public async Task ResetPassword_ShouldReturnUnauthorized_WhenUserNotFound()
        {
            // Arrange
            var request = new ResetPasswordRequest
            {
                Email = "test@example.com",
                Code = "123456",
                NewPassword = "Password123!",
                ConfirmNewPassword = "Password123!",
            };

            _userRepositoryMock
                .Setup(r =>
                    r.FirstOrDefaultAsync(
                        It.IsAny<Expression<Func<User, bool>>>(),
                        It.IsAny<QueryType>()
                    )
                )
                .ReturnsAsync((User?)null);

            // Act
            var result = await _sut.ResetPassword(request);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Type.Should().Be(ResultType.Unauthorized);
        }

        [Fact]
        public async Task ResetPassword_ShouldReturnUnauthorized_WhenCodeNotFound()
        {
            // Arrange
            var request = new ResetPasswordRequest
            {
                Email = "test@example.com",
                Code = "123456",
                NewPassword = "Password123!",
                ConfirmNewPassword = "Password123!",
            };
            var user = new User { Id = Guid.NewGuid(), Email = request.Email };

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

            // Act
            var result = await _sut.ResetPassword(request);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Type.Should().Be(ResultType.Unauthorized);
        }

        [Fact]
        public async Task ResetPassword_ShouldReturnUnauthorized_WhenCodeIsInvalid()
        {
            // Arrange
            var request = new ResetPasswordRequest
            {
                Email = "test@example.com",
                Code = "123456",
                NewPassword = "Password123!",
                ConfirmNewPassword = "Password123!",
            };
            var user = new User { Id = Guid.NewGuid(), Email = request.Email };
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
                .Returns(false);

            // Act
            var result = await _sut.ResetPassword(request);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Type.Should().Be(ResultType.Unauthorized);
        }

        [Fact]
        public async Task ResetPassword_ShouldReturnSuccess_WhenValid()
        {
            // Arrange
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
                PasswordHash = "old_hash",
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

            // Act
            var result = await _sut.ResetPassword(request);

            // Assert
            result.IsSuccess.Should().BeTrue();
            user.PasswordHash.Should().Be("new_password_hash");
            verification.IsConsumed.Should().BeTrue();
            _unitOfWorkMock.Verify(
                u => u.SaveChangesAsync(It.IsAny<CancellationToken>()),
                Times.Once
            );

            // Critical: Verify correct hashing methods used
            _hashingServiceMock.Verify(h => h.HashPassword(request.NewPassword), Times.Once);
            _hashingServiceMock.Verify(
                h => h.VerifyToken(request.Code, verification.CodeHash),
                Times.Once
            );
            _hashingServiceMock.Verify(h => h.HashToken(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task ResetPassword_ShouldReturnUnexpected_WhenSaveChangesFails()
        {
            // Arrange
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
                PasswordHash = "old_hash",
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
                .Setup(h => h.VerifyToken(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(true);
            _hashingServiceMock.Setup(h => h.HashPassword(It.IsAny<string>())).Returns("new_hash");
            _unitOfWorkMock
                .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _sut.ResetPassword(request);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Type.Should().Be(ResultType.Unexpected);
        }

        #endregion

        #region RefreshBothToken Tests

        [Fact]
        public async Task RefreshBothToken_ShouldReturnUnauthorized_WhenTokenNotFound()
        {
            // Arrange
            var rawToken = "refresh_token";
            _hashingServiceMock.Setup(h => h.HashToken(rawToken)).Returns("hashed");
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
        }

        [Fact]
        public async Task RefreshBothToken_ShouldReturnUnauthorized_WhenTokenExpired()
        {
            // Arrange
            var rawToken = "refresh_token";
            var expiredToken = new RefreshToken
            {
                TokenHash = "hashed",
                IsRevoked = false,
                ExpireDate = DateTime.UtcNow.AddMinutes(-1),
            };

            _hashingServiceMock.Setup(h => h.HashToken(rawToken)).Returns("hashed");
            _refreshTokenRepositoryMock
                .Setup(r =>
                    r.FirstOrDefaultAsync(
                        It.IsAny<Expression<Func<RefreshToken, bool>>>(),
                        It.IsAny<QueryType>()
                    )
                )
                .ReturnsAsync(expiredToken);

            // Act
            var result = await _sut.RefreshBothToken(rawToken);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Type.Should().Be(ResultType.Unauthorized);
        }

        [Fact]
        public async Task RefreshBothToken_ShouldReturnUnexpected_WhenUserNotFound()
        {
            // Arrange
            var rawToken = "refresh_token";
            var storedToken = new RefreshToken
            {
                UserId = Guid.NewGuid(),
                TokenHash = "hashed",
                IsRevoked = false,
                ExpireDate = DateTime.UtcNow.AddDays(1),
            };

            _hashingServiceMock.Setup(h => h.HashToken(rawToken)).Returns("hashed");
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

            // Act
            var result = await _sut.RefreshBothToken(rawToken);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Type.Should().Be(ResultType.Unexpected);
        }

        [Fact]
        public async Task RefreshBothToken_ShouldReturnUnexpected_WhenRoleNotFound()
        {
            // Arrange
            var rawToken = "refresh_token";
            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = "test@example.com",
                RoleId = Guid.NewGuid(),
            };
            var storedToken = new RefreshToken
            {
                UserId = user.Id,
                TokenHash = "hashed",
                IsRevoked = false,
                ExpireDate = DateTime.UtcNow.AddDays(1),
            };

            _hashingServiceMock.Setup(h => h.HashToken(rawToken)).Returns("hashed");
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

            // Act
            var result = await _sut.RefreshBothToken(rawToken);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Type.Should().Be(ResultType.Unexpected);
        }

        [Fact]
        public async Task RefreshBothToken_ShouldReturnNewTokens_WhenSuccessful()
        {
            // Arrange
            var rawToken = "old_refresh";
            var role = new Role { Id = Guid.NewGuid(), Name = "User" };
            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = "test@example.com",
                RoleId = role.Id,
            };
            var storedToken = new RefreshToken
            {
                UserId = user.Id,
                TokenHash = "old_hash",
                IsRevoked = false,
                ExpireDate = DateTime.UtcNow.AddDays(1),
            };

            _hashingServiceMock.Setup(h => h.HashToken(rawToken)).Returns("old_hash");
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
            _jwtServiceMock
                .Setup(j => j.GenerateRefreshToken())
                .Returns(new RefreshTokenResult("new_refresh", DateTime.UtcNow.AddDays(1)));
            _hashingServiceMock.Setup(h => h.HashToken("new_refresh")).Returns("new_hash");

            // Act
            var result = await _sut.RefreshBothToken(rawToken);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Data.AccessToken.Should().Be("new_access");
            result.Data.RefreshToken.Should().Be("new_refresh");
            storedToken.IsRevoked.Should().BeTrue();
            _refreshTokenRepositoryMock.Verify(
                r =>
                    r.AddAsync(
                        It.Is<RefreshToken>(rt =>
                            rt.UserId == user.Id && rt.TokenHash == "new_hash"
                        )
                    ),
                Times.Once
            );
            _unitOfWorkMock.Verify(
                u => u.SaveChangesAsync(It.IsAny<CancellationToken>()),
                Times.Once
            );

            // Critical: Verify correct hashing methods used
            _hashingServiceMock.Verify(h => h.HashToken(rawToken), Times.Once);
            _hashingServiceMock.Verify(h => h.HashToken("new_refresh"), Times.Once);
            _hashingServiceMock.Verify(h => h.HashPassword(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task RefreshBothToken_ShouldReturnUnexpected_WhenSaveChangesFails()
        {
            // Arrange
            var rawToken = "refresh";
            var role = new Role { Id = Guid.NewGuid(), Name = "User" };
            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = "test@example.com",
                RoleId = role.Id,
            };
            var storedToken = new RefreshToken
            {
                UserId = user.Id,
                TokenHash = "hash",
                IsRevoked = false,
                ExpireDate = DateTime.UtcNow.AddDays(1),
            };

            _hashingServiceMock.Setup(h => h.HashToken(It.IsAny<string>())).Returns("hash");
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
                .Setup(j =>
                    j.GenerateAccessToken(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>())
                )
                .Returns("token");
            _jwtServiceMock
                .Setup(j => j.GenerateRefreshToken())
                .Returns(new RefreshTokenResult("new", DateTime.UtcNow.AddDays(1)));
            _unitOfWorkMock
                .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _sut.RefreshBothToken(rawToken);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Type.Should().Be(ResultType.Unexpected);
        }

        #endregion

        #region Logout Tests

        [Fact]
        public async Task Logout_ShouldRevokeToken_WhenTokenExists()
        {
            // Arrange
            var plainToken = "refresh_token";
            var refreshToken = new RefreshToken
            {
                TokenHash = "hashed",
                IsRevoked = false,
                ExpireDate = DateTime.UtcNow.AddMinutes(10),
            };

            _hashingServiceMock.Setup(h => h.HashToken(plainToken)).Returns("hashed");
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
            refreshToken.IsRevoked.Should().BeTrue();
            _refreshTokenRepositoryMock.Verify(r => r.Update(refreshToken), Times.Once);
            _unitOfWorkMock.Verify(
                u => u.SaveChangesAsync(It.IsAny<CancellationToken>()),
                Times.Once
            );

            // Critical: Verify correct hashing method used
            _hashingServiceMock.Verify(h => h.HashToken(plainToken), Times.Once);
            _hashingServiceMock.Verify(h => h.HashPassword(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task Logout_ShouldReturnSuccess_WhenTokenDoesNotExist()
        {
            // Arrange
            var plainToken = "refresh_token";

            _hashingServiceMock.Setup(h => h.HashToken(plainToken)).Returns("hashed");
            _refreshTokenRepositoryMock
                .Setup(r =>
                    r.FirstOrDefaultAsync(
                        It.IsAny<Expression<Func<RefreshToken, bool>>>(),
                        It.IsAny<QueryType>()
                    )
                )
                .ReturnsAsync((RefreshToken?)null);

            // Act
            var result = await _sut.Logout(plainToken);

            // Assert
            result.IsSuccess.Should().BeTrue();
            _unitOfWorkMock.Verify(
                u => u.SaveChangesAsync(It.IsAny<CancellationToken>()),
                Times.Never
            );
        }

        #endregion
    }
}

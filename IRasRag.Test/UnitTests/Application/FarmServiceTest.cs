using System.Linq.Expressions;
using AutoMapper;
using FluentAssertions;
using IRasRag.Application.Common.Interfaces;
using IRasRag.Application.Common.Mappings;
using IRasRag.Application.Common.Models;
using IRasRag.Application.DTOs;
using IRasRag.Application.Services.Implementations;
using IRasRag.Domain.Entities;
using IRasRag.Domain.Enums;
using IRasRag.Test.UnitTests.Helpers;
using Microsoft.Extensions.Logging;
using Moq;

namespace IRasRag.Test.UnitTests.Application
{
    public class FarmServiceTest
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<ILogger<FarmService>> _loggerMock;
        private readonly IMapper _mapper;
        private readonly Mock<IRepository<Farm>> _repositoryMock;
        private readonly FarmService _sut;

        public FarmServiceTest()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _loggerMock = new Mock<ILogger<FarmService>>();
            _mapper = AutoMapperTestHelper.GetMapper(new FarmProfile());
            _repositoryMock = new Mock<IRepository<Farm>>();
            _unitOfWorkMock.Setup(x => x.GetRepository<Farm>()).Returns(_repositoryMock.Object);

            _sut = new FarmService(_unitOfWorkMock.Object, _loggerMock.Object, _mapper);
        }

        #region CreateFarmAsync Tests

        [Fact]
        public async Task CreateFarmAsync_ShouldReturnOk_WhenSuccessful()
        {
            // Arrange
            var createDto = new CreateFarmDto
            {
                Name = "Trang trại ABC",
                Address = "123 Đường ABC, Quận 1, TP.HCM",
                PhoneNumber = "0123456789",
                Email = "abc@farm.com",
            };

            _repositoryMock
                .Setup(r =>
                    r.FirstOrDefaultAsync(
                        It.IsAny<Expression<Func<Farm, bool>>>(),
                        It.IsAny<QueryType>()
                    )
                )
                .ReturnsAsync((Farm?)null);
            _unitOfWorkMock
                .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            var result = await _sut.CreateFarmAsync(createDto);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Type.Should().Be(ResultType.Ok);
            result.Message.Should().Be("Tạo trang trại thành công.");
            result.Data.Should().NotBeNull();
            result.Data!.Name.Should().Be(createDto.Name);
            result.Data.Address.Should().Be(createDto.Address);
            result.Data.PhoneNumber.Should().Be(createDto.PhoneNumber);
            result.Data.Email.Should().Be(createDto.Email);

            _repositoryMock.Verify(
                r =>
                    r.AddAsync(
                        It.Is<Farm>(f =>
                            f.Name == createDto.Name
                            && f.Address == createDto.Address
                            && f.PhoneNumber == createDto.PhoneNumber
                            && f.Email == createDto.Email
                            && f.IsDeleted == false
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
        public async Task CreateFarmAsync_ShouldTrimInputs_WhenCreating()
        {
            // Arrange
            var createDto = new CreateFarmDto
            {
                Name = "  Trang trại ABC  ",
                Address = "  123 Đường ABC  ",
                PhoneNumber = "  0123456789  ",
                Email = "  abc@farm.com  ",
            };

            _repositoryMock
                .Setup(r =>
                    r.FirstOrDefaultAsync(
                        It.IsAny<Expression<Func<Farm, bool>>>(),
                        It.IsAny<QueryType>()
                    )
                )
                .ReturnsAsync((Farm?)null);
            _unitOfWorkMock
                .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            var result = await _sut.CreateFarmAsync(createDto);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Data!.Name.Should().Be("Trang trại ABC");
            result.Data.Address.Should().Be("123 Đường ABC");
            result.Data.PhoneNumber.Should().Be("0123456789");
            result.Data.Email.Should().Be("abc@farm.com");

            _repositoryMock.Verify(
                r =>
                    r.AddAsync(
                        It.Is<Farm>(f =>
                            f.Name == "Trang trại ABC"
                            && f.Address == "123 Đường ABC"
                            && f.PhoneNumber == "0123456789"
                            && f.Email == "abc@farm.com"
                        )
                    ),
                Times.Once
            );
        }

        [Theory]
        [InlineData("", "Tên trang trại không được để trống.")]
        [InlineData(" ", "Tên trang trại không được để trống.")]
        [InlineData("  ", "Tên trang trại không được để trống.")]
        [InlineData("\t", "Tên trang trại không được để trống.")]
        [InlineData("\n", "Tên trang trại không được để trống.")]
        public async Task CreateFarmAsync_ShouldReturnBadRequest_WhenNameIsInvalid(
            string name,
            string expectedMessage
        )
        {
            // Arrange
            var createDto = new CreateFarmDto
            {
                Name = name,
                Address = "123 Đường ABC",
                PhoneNumber = "0123456789",
                Email = "abc@farm.com",
            };
            _unitOfWorkMock
                .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            var result = await _sut.CreateFarmAsync(createDto);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Type.Should().Be(ResultType.BadRequest);
            result.Message.Should().Be(expectedMessage);

            _repositoryMock.Verify(r => r.AddAsync(It.IsAny<Farm>()), Times.Never);
            _unitOfWorkMock.Verify(
                u => u.SaveChangesAsync(It.IsAny<CancellationToken>()),
                Times.Never
            );
        }

        [Theory]
        [InlineData("", "Địa chỉ không được để trống.")]
        [InlineData(" ", "Địa chỉ không được để trống.")]
        [InlineData("  ", "Địa chỉ không được để trống.")]
        [InlineData("\t", "Địa chỉ không được để trống.")]
        [InlineData("\n", "Địa chỉ không được để trống.")]
        public async Task CreateFarmAsync_ShouldReturnBadRequest_WhenAddressIsInvalid(
            string address,
            string expectedMessage
        )
        {
            // Arrange
            var createDto = new CreateFarmDto
            {
                Name = "Trang trại ABC",
                Address = address,
                PhoneNumber = "0123456789",
                Email = "abc@farm.com",
            };
            _unitOfWorkMock
                .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            var result = await _sut.CreateFarmAsync(createDto);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Type.Should().Be(ResultType.BadRequest);
            result.Message.Should().Be(expectedMessage);

            _repositoryMock.Verify(r => r.AddAsync(It.IsAny<Farm>()), Times.Never);
            _unitOfWorkMock.Verify(
                u => u.SaveChangesAsync(It.IsAny<CancellationToken>()),
                Times.Never
            );
        }

        [Theory]
        [InlineData("", "Số điện thoại không được để trống.")]
        [InlineData(" ", "Số điện thoại không được để trống.")]
        [InlineData("  ", "Số điện thoại không được để trống.")]
        [InlineData("\t", "Số điện thoại không được để trống.")]
        [InlineData("\n", "Số điện thoại không được để trống.")]
        public async Task CreateFarmAsync_ShouldReturnBadRequest_WhenPhoneNumberIsInvalid(
            string phoneNumber,
            string expectedMessage
        )
        {
            // Arrange
            var createDto = new CreateFarmDto
            {
                Name = "Trang trại ABC",
                Address = "123 Đường ABC",
                PhoneNumber = phoneNumber,
                Email = "abc@farm.com",
            };
            _unitOfWorkMock
                .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            var result = await _sut.CreateFarmAsync(createDto);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Type.Should().Be(ResultType.BadRequest);
            result.Message.Should().Be(expectedMessage);

            _repositoryMock.Verify(r => r.AddAsync(It.IsAny<Farm>()), Times.Never);
            _unitOfWorkMock.Verify(
                u => u.SaveChangesAsync(It.IsAny<CancellationToken>()),
                Times.Never
            );
        }

        [Theory]
        [InlineData("", "Email không được để trống.")]
        [InlineData(" ", "Email không được để trống.")]
        [InlineData("  ", "Email không được để trống.")]
        [InlineData("\t", "Email không được để trống.")]
        [InlineData("\n", "Email không được để trống.")]
        public async Task CreateFarmAsync_ShouldReturnBadRequest_WhenEmailIsInvalid(
            string email,
            string expectedMessage
        )
        {
            // Arrange
            var createDto = new CreateFarmDto
            {
                Name = "Trang trại ABC",
                Address = "123 Đường ABC",
                PhoneNumber = "0123456789",
                Email = email,
            };
            _unitOfWorkMock
                .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            var result = await _sut.CreateFarmAsync(createDto);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Type.Should().Be(ResultType.BadRequest);
            result.Message.Should().Be(expectedMessage);

            _repositoryMock.Verify(r => r.AddAsync(It.IsAny<Farm>()), Times.Never);
            _unitOfWorkMock.Verify(
                u => u.SaveChangesAsync(It.IsAny<CancellationToken>()),
                Times.Never
            );
        }

        [Fact]
        public async Task CreateFarmAsync_ShouldReturnConflict_WhenDuplicateEmailExists()
        {
            // Arrange
            var createDto = new CreateFarmDto
            {
                Name = "Trang trại ABC",
                Address = "123 Đường ABC",
                PhoneNumber = "0123456789",
                Email = "abc@farm.com",
            };
            var existingFarm = new Farm
            {
                Id = Guid.NewGuid(),
                Name = "Trang trại XYZ",
                Address = "456 Đường XYZ",
                PhoneNumber = "0987654321",
                Email = "abc@farm.com",
                IsDeleted = false,
            };

            _repositoryMock
                .Setup(r =>
                    r.FirstOrDefaultAsync(
                        It.IsAny<Expression<Func<Farm, bool>>>(),
                        It.IsAny<QueryType>()
                    )
                )
                .ReturnsAsync(existingFarm);
            _unitOfWorkMock
                .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            var result = await _sut.CreateFarmAsync(createDto);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Type.Should().Be(ResultType.Conflict);
            result.Message.Should().Be("Email này đã được sử dụng cho trang trại khác.");

            _repositoryMock.Verify(r => r.AddAsync(It.IsAny<Farm>()), Times.Never);
            _unitOfWorkMock.Verify(
                u => u.SaveChangesAsync(It.IsAny<CancellationToken>()),
                Times.Never
            );
        }

        [Fact]
        public async Task CreateFarmAsync_ShouldReturnUnexpected_WhenExceptionThrown()
        {
            // Arrange
            var createDto = new CreateFarmDto
            {
                Name = "Trang trại ABC",
                Address = "123 Đường ABC",
                PhoneNumber = "0123456789",
                Email = "abc@farm.com",
            };

            _repositoryMock
                .Setup(r =>
                    r.FirstOrDefaultAsync(
                        It.IsAny<Expression<Func<Farm, bool>>>(),
                        It.IsAny<QueryType>()
                    )
                )
                .ThrowsAsync(new Exception());
            _unitOfWorkMock
                .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            var result = await _sut.CreateFarmAsync(createDto);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Type.Should().Be(ResultType.Unexpected);
            result.Message.Should().Be("Lỗi khi tạo trang trại.");
        }

        #endregion

        #region GetFarmByIdAsync Tests

        [Fact]
        public async Task GetFarmByIdAsync_ShouldReturnOk_WhenFarmExists()
        {
            // Arrange
            var farmId = Guid.NewGuid();
            var farm = new Farm
            {
                Id = farmId,
                Name = "Trang trại ABC",
                Address = "123 Đường ABC",
                PhoneNumber = "0123456789",
                Email = "abc@farm.com",
                IsDeleted = false,
            };

            _repositoryMock
                .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), QueryType.ActiveOnly))
                .ReturnsAsync(farm);

            // Act
            var result = await _sut.GetFarmByIdAsync(farmId);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Type.Should().Be(ResultType.Ok);
            result.Message.Should().Be("Lấy thông tin trang trại thành công.");
            result.Data.Should().NotBeNull();
            result.Data!.Id.Should().Be(farmId);
            result.Data.Name.Should().Be(farm.Name);
            result.Data.Address.Should().Be(farm.Address);
            result.Data.PhoneNumber.Should().Be(farm.PhoneNumber);
            result.Data.Email.Should().Be(farm.Email);

            _repositoryMock.Verify(
                r => r.GetByIdAsync(It.Is<Guid>(id => id == farmId), QueryType.ActiveOnly),
                Times.Once
            );
        }

        [Fact]
        public async Task GetFarmByIdAsync_ShouldReturnNotFound_WhenFarmNotExists()
        {
            // Arrange
            var farmId = Guid.NewGuid();
            _repositoryMock
                .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), QueryType.ActiveOnly))
                .ReturnsAsync((Farm?)null);

            // Act
            var result = await _sut.GetFarmByIdAsync(farmId);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Type.Should().Be(ResultType.NotFound);
            result.Message.Should().Be("Trang trại không tồn tại.");
            result.Data.Should().BeNull();

            _repositoryMock.Verify(
                r => r.GetByIdAsync(It.Is<Guid>(id => id == farmId), QueryType.ActiveOnly),
                Times.Once
            );
        }

        [Fact]
        public async Task GetFarmByIdAsync_ShouldReturnNotFound_WhenFarmIsDeleted()
        {
            // Arrange
            var farmId = Guid.NewGuid();
            var deletedFarm = new Farm
            {
                Id = farmId,
                Name = "Trang trại ABC",
                Address = "123 Đường ABC",
                PhoneNumber = "0123456789",
                Email = "abc@farm.com",
                IsDeleted = true,
                DeletedAt = DateTime.UtcNow,
            };

            _repositoryMock
                .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), QueryType.ActiveOnly))
                .ReturnsAsync(deletedFarm);

            // Act
            var result = await _sut.GetFarmByIdAsync(farmId);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Type.Should().Be(ResultType.NotFound);
            result.Message.Should().Be("Trang trại không tồn tại.");

            _repositoryMock.Verify(
                r => r.GetByIdAsync(It.Is<Guid>(id => id == farmId), QueryType.ActiveOnly),
                Times.Once
            );
        }

        [Fact]
        public async Task GetFarmByIdAsync_ShouldReturnUnexpected_WhenThrownException()
        {
            // Arrange
            var farmId = Guid.NewGuid();
            _repositoryMock
                .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), QueryType.ActiveOnly))
                .ThrowsAsync(new Exception());

            // Act
            var result = await _sut.GetFarmByIdAsync(farmId);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Type.Should().Be(ResultType.Unexpected);
            result.Message.Should().Be("Lỗi khi truy xuất thông tin trang trại.");

            _repositoryMock.Verify(
                r => r.GetByIdAsync(It.Is<Guid>(id => id == farmId), QueryType.ActiveOnly),
                Times.Once
            );
        }

        #endregion

        #region GetAllFarmsAsync Tests

        [Fact]
        public async Task GetAllFarmsAsync_ShouldReturnOk_WhenSuccessful()
        {
            // Arrange
            var farmList = new List<Farm>
            {
                new Farm
                {
                    Id = Guid.NewGuid(),
                    Name = "Trang trại ABC",
                    Address = "123 Đường ABC",
                    PhoneNumber = "0123456789",
                    Email = "abc@farm.com",
                    IsDeleted = false,
                },
                new Farm
                {
                    Id = Guid.NewGuid(),
                    Name = "Trang trại XYZ",
                    Address = "456 Đường XYZ",
                    PhoneNumber = "0987654321",
                    Email = "xyz@farm.com",
                    IsDeleted = false,
                },
            };

            _repositoryMock
                .Setup(r =>
                    r.FindAllAsync(It.IsAny<Expression<Func<Farm, bool>>>(), It.IsAny<QueryType>())
                )
                .ReturnsAsync(farmList);

            // Act
            var result = await _sut.GetAllFarmsAsync();

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Type.Should().Be(ResultType.Ok);
            result.Message.Should().Be("Lấy danh sách trang trại thành công.");
            result.Data.Should().NotBeNull();
            result.Data!.Count().Should().Be(2);

            _repositoryMock.Verify(
                r =>
                    r.FindAllAsync(It.IsAny<Expression<Func<Farm, bool>>>(), It.IsAny<QueryType>()),
                Times.Once
            );
        }

        [Fact]
        public async Task GetAllFarmsAsync_ShouldReturnEmptyList_WhenNoRecordsExist()
        {
            // Arrange
            var emptyList = new List<Farm>();
            _repositoryMock
                .Setup(r =>
                    r.FindAllAsync(It.IsAny<Expression<Func<Farm, bool>>>(), It.IsAny<QueryType>())
                )
                .ReturnsAsync(emptyList);

            // Act
            var result = await _sut.GetAllFarmsAsync();

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Type.Should().Be(ResultType.Ok);
            result.Message.Should().Be("Lấy danh sách trang trại thành công.");
            result.Data.Should().NotBeNull();
            result.Data!.Should().BeEmpty();

            _repositoryMock.Verify(
                r =>
                    r.FindAllAsync(It.IsAny<Expression<Func<Farm, bool>>>(), It.IsAny<QueryType>()),
                Times.Once
            );
        }

        [Fact]
        public async Task GetAllFarmsAsync_ShouldExcludeDeletedFarms_WhenRetrievingList()
        {
            // Arrange
            var farmList = new List<Farm>
            {
                new Farm
                {
                    Id = Guid.NewGuid(),
                    Name = "Trang trại ABC",
                    Email = "abc@farm.com",
                    IsDeleted = false,
                },
            };

            _repositoryMock
                .Setup(r =>
                    r.FindAllAsync(It.IsAny<Expression<Func<Farm, bool>>>(), It.IsAny<QueryType>())
                )
                .ReturnsAsync(farmList);

            // Act
            var result = await _sut.GetAllFarmsAsync();

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Data!.All(f => !f.Id.Equals(Guid.Empty)).Should().BeTrue();

            _repositoryMock.Verify(
                r =>
                    r.FindAllAsync(
                        It.Is<Expression<Func<Farm, bool>>>(expr => expr != null),
                        It.IsAny<QueryType>()
                    ),
                Times.Once
            );
        }

        [Fact]
        public async Task GetAllFarmsAsync_ShouldReturnUnexpected_WhenThrownException()
        {
            // Arrange
            _repositoryMock
                .Setup(r =>
                    r.FindAllAsync(It.IsAny<Expression<Func<Farm, bool>>>(), It.IsAny<QueryType>())
                )
                .ThrowsAsync(new Exception());

            // Act
            var result = await _sut.GetAllFarmsAsync();

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Type.Should().Be(ResultType.Unexpected);
            result.Message.Should().Be("Lỗi khi truy xuất danh sách trang trại.");

            _repositoryMock.Verify(
                r =>
                    r.FindAllAsync(It.IsAny<Expression<Func<Farm, bool>>>(), It.IsAny<QueryType>()),
                Times.Once
            );
        }

        #endregion

        #region DeleteFarmAsync Tests

        [Fact]
        public async Task DeleteFarmAsync_ShouldReturnOk_WhenSuccessful()
        {
            // Arrange
            var farmId = Guid.NewGuid();
            var farm = new Farm
            {
                Id = farmId,
                Name = "Trang trại ABC",
                Address = "123 Đường ABC",
                PhoneNumber = "0123456789",
                Email = "abc@farm.com",
                IsDeleted = false,
            };

            _repositoryMock
                .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), QueryType.ActiveOnly))
                .ReturnsAsync(farm);
            _repositoryMock.Setup(r => r.Update(It.IsAny<Farm>())).Verifiable();
            _unitOfWorkMock
                .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            var result = await _sut.DeleteFarmAsync(farmId);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Type.Should().Be(ResultType.Ok);
            result.Message.Should().Be("Xóa trang trại thành công.");
            farm.IsDeleted.Should().BeTrue();
            farm.DeletedAt.Should().NotBeNull();
            farm.DeletedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));

            _repositoryMock.Verify(
                r => r.GetByIdAsync(It.Is<Guid>(id => id == farmId), QueryType.ActiveOnly),
                Times.Once
            );
            _repositoryMock.Verify(
                r => r.Update(It.Is<Farm>(f => f.Id == farmId && f.IsDeleted == true)),
                Times.Once
            );
            _unitOfWorkMock.Verify(
                u => u.SaveChangesAsync(It.IsAny<CancellationToken>()),
                Times.Once
            );
        }

        [Fact]
        public async Task DeleteFarmAsync_ShouldReturnNotFound_WhenFarmNotExists()
        {
            // Arrange
            var farmId = Guid.NewGuid();
            _repositoryMock
                .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), QueryType.ActiveOnly))
                .ReturnsAsync((Farm?)null);

            // Act
            var result = await _sut.DeleteFarmAsync(farmId);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Type.Should().Be(ResultType.NotFound);
            result.Message.Should().Be("Trang trại không tồn tại.");

            _repositoryMock.Verify(
                r => r.GetByIdAsync(It.Is<Guid>(id => id == farmId), QueryType.ActiveOnly),
                Times.Once
            );
            _repositoryMock.Verify(r => r.Update(It.IsAny<Farm>()), Times.Never);
            _unitOfWorkMock.Verify(
                u => u.SaveChangesAsync(It.IsAny<CancellationToken>()),
                Times.Never
            );
        }

        [Fact]
        public async Task DeleteFarmAsync_ShouldReturnNotFound_WhenFarmAlreadyDeleted()
        {
            // Arrange
            var farmId = Guid.NewGuid();
            var deletedFarm = new Farm
            {
                Id = farmId,
                Name = "Trang trại ABC",
                IsDeleted = true,
                DeletedAt = DateTime.UtcNow.AddDays(-1),
            };

            _repositoryMock
                .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), QueryType.ActiveOnly))
                .ReturnsAsync(deletedFarm);

            // Act
            var result = await _sut.DeleteFarmAsync(farmId);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Type.Should().Be(ResultType.NotFound);
            result.Message.Should().Be("Trang trại không tồn tại.");

            _repositoryMock.Verify(r => r.Update(It.IsAny<Farm>()), Times.Never);
            _unitOfWorkMock.Verify(
                u => u.SaveChangesAsync(It.IsAny<CancellationToken>()),
                Times.Never
            );
        }

        [Fact]
        public async Task DeleteFarmAsync_ShouldReturnUnexpected_WhenThrownException()
        {
            // Arrange
            var farmId = Guid.NewGuid();
            _repositoryMock
                .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), QueryType.ActiveOnly))
                .ThrowsAsync(new Exception());

            // Act
            var result = await _sut.DeleteFarmAsync(farmId);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Type.Should().Be(ResultType.Unexpected);
            result.Message.Should().Be("Lỗi khi xóa trang trại.");

            _repositoryMock.Verify(
                r => r.GetByIdAsync(It.Is<Guid>(id => id == farmId), QueryType.ActiveOnly),
                Times.Once
            );
        }

        #endregion

        #region UpdateFarmAsync Tests

        [Fact]
        public async Task UpdateFarmAsync_ShouldReturnOk_WhenSuccessful()
        {
            // Arrange
            var farmId = Guid.NewGuid();
            var existingFarm = new Farm
            {
                Id = farmId,
                Name = "Trang trại ABC",
                Address = "123 Đường ABC",
                PhoneNumber = "0123456789",
                Email = "abc@farm.com",
                IsDeleted = false,
            };
            var updateDto = new UpdateFarmDto
            {
                Name = "Trang trại ABC Mới",
                Address = "456 Đường XYZ",
                PhoneNumber = "0987654321",
                Email = "new@farm.com",
            };

            _repositoryMock
                .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), QueryType.ActiveOnly))
                .ReturnsAsync(existingFarm);
            _repositoryMock
                .Setup(r =>
                    r.FirstOrDefaultAsync(
                        It.IsAny<Expression<Func<Farm, bool>>>(),
                        It.IsAny<QueryType>()
                    )
                )
                .ReturnsAsync((Farm?)null);
            _repositoryMock.Setup(r => r.Update(It.IsAny<Farm>())).Verifiable();
            _unitOfWorkMock
                .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            var result = await _sut.UpdateFarmAsync(farmId, updateDto);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Type.Should().Be(ResultType.Ok);
            result.Message.Should().Be("Cập nhật trang trại thành công.");

            existingFarm.Name.Should().Be(updateDto.Name);
            existingFarm.Address.Should().Be(updateDto.Address);
            existingFarm.PhoneNumber.Should().Be(updateDto.PhoneNumber);
            existingFarm.Email.Should().Be(updateDto.Email);

            _repositoryMock.Verify(r => r.Update(It.Is<Farm>(f => f.Id == farmId)), Times.Once);
            _unitOfWorkMock.Verify(
                u => u.SaveChangesAsync(It.IsAny<CancellationToken>()),
                Times.Once
            );
        }

        [Theory]
        [InlineData(null, "Trang trại ABC")]
        [InlineData("", "Trang trại ABC")]
        [InlineData(" ", "Trang trại ABC")]
        [InlineData("  ", "Trang trại ABC")]
        public async Task UpdateFarmAsync_ShouldNotUpdateName_WhenNameIsNullOrWhitespace(
            string nameInput,
            string expectedName
        )
        {
            // Arrange
            var farmId = Guid.NewGuid();
            var existingFarm = new Farm
            {
                Id = farmId,
                Name = "Trang trại ABC",
                Address = "123 Đường ABC",
                PhoneNumber = "0123456789",
                Email = "abc@farm.com",
                IsDeleted = false,
            };
            var updateDto = new UpdateFarmDto { Name = nameInput };

            _repositoryMock
                .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), QueryType.ActiveOnly))
                .ReturnsAsync(existingFarm);
            _repositoryMock.Setup(r => r.Update(It.IsAny<Farm>())).Verifiable();
            _unitOfWorkMock
                .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            var result = await _sut.UpdateFarmAsync(farmId, updateDto);

            // Assert
            result.IsSuccess.Should().BeTrue();
            existingFarm.Name.Should().Be(expectedName);
        }

        [Fact]
        public async Task UpdateFarmAsync_ShouldTrimInputs_WhenUpdating()
        {
            // Arrange
            var farmId = Guid.NewGuid();
            var existingFarm = new Farm
            {
                Id = farmId,
                Name = "Trang trại ABC",
                Address = "123 Đường ABC",
                PhoneNumber = "0123456789",
                Email = "abc@farm.com",
                IsDeleted = false,
            };
            var updateDto = new UpdateFarmDto
            {
                Name = "  Trang trại ABC Mới  ",
                Address = "  456 Đường XYZ  ",
                PhoneNumber = "  0987654321  ",
                Email = "  new@farm.com  ",
            };

            _repositoryMock
                .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), QueryType.ActiveOnly))
                .ReturnsAsync(existingFarm);
            _repositoryMock
                .Setup(r =>
                    r.FirstOrDefaultAsync(
                        It.IsAny<Expression<Func<Farm, bool>>>(),
                        It.IsAny<QueryType>()
                    )
                )
                .ReturnsAsync((Farm?)null);
            _repositoryMock.Setup(r => r.Update(It.IsAny<Farm>())).Verifiable();
            _unitOfWorkMock
                .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            var result = await _sut.UpdateFarmAsync(farmId, updateDto);

            // Assert
            result.IsSuccess.Should().BeTrue();
            existingFarm.Name.Should().Be("Trang trại ABC Mới");
            existingFarm.Address.Should().Be("456 Đường XYZ");
            existingFarm.PhoneNumber.Should().Be("0987654321");
            existingFarm.Email.Should().Be("new@farm.com");
        }

        [Fact]
        public async Task UpdateFarmAsync_ShouldReturnNotFound_WhenFarmNotExists()
        {
            // Arrange
            var farmId = Guid.NewGuid();
            var updateDto = new UpdateFarmDto { Name = "Trang trại ABC Mới" };

            _repositoryMock
                .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), QueryType.ActiveOnly))
                .ReturnsAsync((Farm?)null);

            // Act
            var result = await _sut.UpdateFarmAsync(farmId, updateDto);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Type.Should().Be(ResultType.NotFound);
            result.Message.Should().Be("Trang trại không tồn tại.");

            _repositoryMock.Verify(
                r => r.GetByIdAsync(It.Is<Guid>(id => id == farmId), QueryType.ActiveOnly),
                Times.Once
            );
            _repositoryMock.Verify(r => r.Update(It.IsAny<Farm>()), Times.Never);
            _unitOfWorkMock.Verify(
                u => u.SaveChangesAsync(It.IsAny<CancellationToken>()),
                Times.Never
            );
        }

        [Fact]
        public async Task UpdateFarmAsync_ShouldReturnNotFound_WhenFarmIsDeleted()
        {
            // Arrange
            var farmId = Guid.NewGuid();
            var deletedFarm = new Farm
            {
                Id = farmId,
                Name = "Trang trại ABC",
                IsDeleted = true,
                DeletedAt = DateTime.UtcNow.AddDays(-1),
            };
            var updateDto = new UpdateFarmDto { Name = "Trang trại ABC Mới" };

            _repositoryMock
                .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), QueryType.ActiveOnly))
                .ReturnsAsync(deletedFarm);

            // Act
            var result = await _sut.UpdateFarmAsync(farmId, updateDto);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Type.Should().Be(ResultType.NotFound);
            result.Message.Should().Be("Trang trại không tồn tại.");

            _repositoryMock.Verify(r => r.Update(It.IsAny<Farm>()), Times.Never);
            _unitOfWorkMock.Verify(
                u => u.SaveChangesAsync(It.IsAny<CancellationToken>()),
                Times.Never
            );
        }

        [Fact]
        public async Task UpdateFarmAsync_ShouldReturnConflict_WhenDuplicateEmailExists()
        {
            // Arrange
            var farmId = Guid.NewGuid();
            var existingFarm = new Farm
            {
                Id = farmId,
                Name = "Trang trại ABC",
                Email = "abc@farm.com",
                IsDeleted = false,
            };
            var anotherFarm = new Farm
            {
                Id = Guid.NewGuid(),
                Name = "Trang trại XYZ",
                Email = "xyz@farm.com",
                IsDeleted = false,
            };
            var updateDto = new UpdateFarmDto { Email = "xyz@farm.com" };

            _repositoryMock
                .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), QueryType.ActiveOnly))
                .ReturnsAsync(existingFarm);
            _repositoryMock
                .Setup(r =>
                    r.FirstOrDefaultAsync(
                        It.IsAny<Expression<Func<Farm, bool>>>(),
                        It.IsAny<QueryType>()
                    )
                )
                .ReturnsAsync(anotherFarm);

            // Act
            var result = await _sut.UpdateFarmAsync(farmId, updateDto);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Type.Should().Be(ResultType.Conflict);
            result.Message.Should().Be("Email này đã được sử dụng cho trang trại khác.");

            _repositoryMock.Verify(r => r.Update(It.IsAny<Farm>()), Times.Never);
            _unitOfWorkMock.Verify(
                u => u.SaveChangesAsync(It.IsAny<CancellationToken>()),
                Times.Never
            );
        }

        [Fact]
        public async Task UpdateFarmAsync_ShouldReturnUnexpected_WhenExceptionThrown()
        {
            // Arrange
            var farmId = Guid.NewGuid();
            var updateDto = new UpdateFarmDto { Name = "Trang trại ABC Mới" };

            _repositoryMock
                .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), QueryType.ActiveOnly))
                .ThrowsAsync(new Exception());

            // Act
            var result = await _sut.UpdateFarmAsync(farmId, updateDto);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Type.Should().Be(ResultType.Unexpected);
            result.Message.Should().Be("Lỗi khi cập nhật trang trại.");

            _repositoryMock.Verify(
                r => r.GetByIdAsync(It.Is<Guid>(id => id == farmId), QueryType.ActiveOnly),
                Times.Once
            );
        }

        #endregion
    }
}

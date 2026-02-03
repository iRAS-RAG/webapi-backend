using System.Linq.Expressions;
using AutoMapper;
using FluentAssertions;
using IRasRag.Application.Common.Interfaces.Persistence;
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
    public class FishTankServiceTest
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<ILogger<FishTankService>> _loggerMock;
        private readonly IMapper _mapper;
        private readonly Mock<IRepository<FishTank>> _fishTankRepositoryMock;
        private readonly Mock<IRepository<Farm>> _farmRepositoryMock;
        private readonly FishTankService _sut;

        public FishTankServiceTest()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _loggerMock = new Mock<ILogger<FishTankService>>();
            _mapper = AutoMapperTestHelper.GetMapper(new FishTankProfile());
            _fishTankRepositoryMock = new Mock<IRepository<FishTank>>();
            _farmRepositoryMock = new Mock<IRepository<Farm>>();

            _unitOfWorkMock
                .Setup(x => x.GetRepository<FishTank>())
                .Returns(_fishTankRepositoryMock.Object);
            _unitOfWorkMock.Setup(x => x.GetRepository<Farm>()).Returns(_farmRepositoryMock.Object);

            _sut = new FishTankService(_unitOfWorkMock.Object, _loggerMock.Object, _mapper);
        }

        #region CreateFishTankAsync Tests

        [Fact]
        public async Task CreateFishTankAsync_ShouldReturnOk_WhenSuccessful()
        {
            // Arrange
            var farmId = Guid.NewGuid();
            var createDto = new CreateFishTankDto
            {
                Name = "Bể cá số 1",
                Height = 2.5f,
                Radius = 3.0f,
                FarmId = farmId,
                TopicCode = "TOPIC001",
                CameraUrl = "http://camera.com/stream1",
            };

            var farm = new Farm
            {
                Id = farmId,
                Name = "Trang trại ABC",
                Email = "abc@farm.com",
                Address = "123 ABC",
                PhoneNumber = "0123456789",
                IsDeleted = false,
            };

            _farmRepositoryMock
                .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), QueryType.ActiveOnly))
                .ReturnsAsync(farm);
            _unitOfWorkMock
                .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            var result = await _sut.CreateFishTankAsync(createDto);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Type.Should().Be(ResultType.Ok);
            result.Message.Should().Be("Tạo bể cá thành công.");
            result.Data.Should().NotBeNull();
            result.Data!.Name.Should().Be(createDto.Name);
            result.Data.Height.Should().Be(createDto.Height);
            result.Data.Radius.Should().Be(createDto.Radius);
            result.Data.FarmId.Should().Be(createDto.FarmId);
            result.Data.FarmName.Should().Be(farm.Name);
            result.Data.TopicCode.Should().Be(createDto.TopicCode);
            result.Data.CameraUrl.Should().Be(createDto.CameraUrl);

            _fishTankRepositoryMock.Verify(
                r =>
                    r.AddAsync(
                        It.Is<FishTank>(ft =>
                            ft.Name == createDto.Name
                            && ft.Height == createDto.Height
                            && ft.Radius == createDto.Radius
                            && ft.FarmId == createDto.FarmId
                            && ft.TopicCode == createDto.TopicCode
                            && ft.CameraUrl == createDto.CameraUrl
                            && ft.IsDeleted == false
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
        public async Task CreateFishTankAsync_ShouldTrimInputs_WhenCreating()
        {
            // Arrange
            var farmId = Guid.NewGuid();
            var createDto = new CreateFishTankDto
            {
                Name = "  Bể cá số 1  ",
                Height = 2.5f,
                Radius = 3.0f,
                FarmId = farmId,
                TopicCode = "  TOPIC001  ",
                CameraUrl = "  http://camera.com/stream1  ",
            };

            var farm = new Farm
            {
                Id = farmId,
                Name = "Trang trại ABC",
                IsDeleted = false,
            };

            _farmRepositoryMock
                .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), QueryType.ActiveOnly))
                .ReturnsAsync(farm);
            _unitOfWorkMock
                .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            var result = await _sut.CreateFishTankAsync(createDto);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Data!.Name.Should().Be("Bể cá số 1");
            result.Data.TopicCode.Should().Be("TOPIC001");
            result.Data.CameraUrl.Should().Be("http://camera.com/stream1");

            _fishTankRepositoryMock.Verify(
                r =>
                    r.AddAsync(
                        It.Is<FishTank>(ft =>
                            ft.Name == "Bể cá số 1"
                            && ft.TopicCode == "TOPIC001"
                            && ft.CameraUrl == "http://camera.com/stream1"
                        )
                    ),
                Times.Once
            );
        }

        [Theory]
        [InlineData("", "Tên bể cá không được để trống.")]
        [InlineData(" ", "Tên bể cá không được để trống.")]
        [InlineData("  ", "Tên bể cá không được để trống.")]
        [InlineData("\t", "Tên bể cá không được để trống.")]
        [InlineData("\n", "Tên bể cá không được để trống.")]
        public async Task CreateFishTankAsync_ShouldReturnBadRequest_WhenNameIsInvalid(
            string name,
            string expectedMessage
        )
        {
            // Arrange
            var createDto = new CreateFishTankDto
            {
                Name = name,
                Height = 2.5f,
                Radius = 3.0f,
                FarmId = Guid.NewGuid(),
                CameraUrl = "http://camera.com/stream1",
            };
            _unitOfWorkMock
                .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            var result = await _sut.CreateFishTankAsync(createDto);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Type.Should().Be(ResultType.BadRequest);
            result.Message.Should().Be(expectedMessage);

            _fishTankRepositoryMock.Verify(r => r.AddAsync(It.IsAny<FishTank>()), Times.Never);
            _unitOfWorkMock.Verify(
                u => u.SaveChangesAsync(It.IsAny<CancellationToken>()),
                Times.Never
            );
        }

        [Theory]
        [InlineData(0, "Chiều cao phải lớn hơn 0.")]
        [InlineData(-1, "Chiều cao phải lớn hơn 0.")]
        [InlineData(-2.5f, "Chiều cao phải lớn hơn 0.")]
        public async Task CreateFishTankAsync_ShouldReturnBadRequest_WhenHeightIsInvalid(
            float height,
            string expectedMessage
        )
        {
            // Arrange
            var createDto = new CreateFishTankDto
            {
                Name = "Bể cá số 1",
                Height = height,
                Radius = 3.0f,
                FarmId = Guid.NewGuid(),
                CameraUrl = "http://camera.com/stream1",
            };
            _unitOfWorkMock
                .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            var result = await _sut.CreateFishTankAsync(createDto);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Type.Should().Be(ResultType.BadRequest);
            result.Message.Should().Be(expectedMessage);

            _fishTankRepositoryMock.Verify(r => r.AddAsync(It.IsAny<FishTank>()), Times.Never);
            _unitOfWorkMock.Verify(
                u => u.SaveChangesAsync(It.IsAny<CancellationToken>()),
                Times.Never
            );
        }

        [Theory]
        [InlineData(0, "Bán kính phải lớn hơn 0.")]
        [InlineData(-1, "Bán kính phải lớn hơn 0.")]
        [InlineData(-3.5f, "Bán kính phải lớn hơn 0.")]
        public async Task CreateFishTankAsync_ShouldReturnBadRequest_WhenRadiusIsInvalid(
            float radius,
            string expectedMessage
        )
        {
            // Arrange
            var createDto = new CreateFishTankDto
            {
                Name = "Bể cá số 1",
                Height = 2.5f,
                Radius = radius,
                FarmId = Guid.NewGuid(),
                CameraUrl = "http://camera.com/stream1",
            };
            _unitOfWorkMock
                .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            var result = await _sut.CreateFishTankAsync(createDto);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Type.Should().Be(ResultType.BadRequest);
            result.Message.Should().Be(expectedMessage);

            _fishTankRepositoryMock.Verify(r => r.AddAsync(It.IsAny<FishTank>()), Times.Never);
            _unitOfWorkMock.Verify(
                u => u.SaveChangesAsync(It.IsAny<CancellationToken>()),
                Times.Never
            );
        }

        [Theory]
        [InlineData("", "URL camera không được để trống.")]
        [InlineData(" ", "URL camera không được để trống.")]
        [InlineData("  ", "URL camera không được để trống.")]
        [InlineData("\t", "URL camera không được để trống.")]
        [InlineData("\n", "URL camera không được để trống.")]
        public async Task CreateFishTankAsync_ShouldReturnBadRequest_WhenCameraUrlIsInvalid(
            string cameraUrl,
            string expectedMessage
        )
        {
            // Arrange
            var createDto = new CreateFishTankDto
            {
                Name = "Bể cá số 1",
                Height = 2.5f,
                Radius = 3.0f,
                FarmId = Guid.NewGuid(),
                CameraUrl = cameraUrl,
            };
            _unitOfWorkMock
                .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            var result = await _sut.CreateFishTankAsync(createDto);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Type.Should().Be(ResultType.BadRequest);
            result.Message.Should().Be(expectedMessage);

            _fishTankRepositoryMock.Verify(r => r.AddAsync(It.IsAny<FishTank>()), Times.Never);
            _unitOfWorkMock.Verify(
                u => u.SaveChangesAsync(It.IsAny<CancellationToken>()),
                Times.Never
            );
        }

        [Fact]
        public async Task CreateFishTankAsync_ShouldReturnBadRequest_WhenFarmNotExists()
        {
            // Arrange
            var createDto = new CreateFishTankDto
            {
                Name = "Bể cá số 1",
                Height = 2.5f,
                Radius = 3.0f,
                FarmId = Guid.NewGuid(),
                CameraUrl = "http://camera.com/stream1",
            };

            _farmRepositoryMock
                .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), QueryType.ActiveOnly))
                .ReturnsAsync((Farm?)null);
            _unitOfWorkMock
                .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            var result = await _sut.CreateFishTankAsync(createDto);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Type.Should().Be(ResultType.BadRequest);
            result.Message.Should().Be("Trang trại không tồn tại.");

            _fishTankRepositoryMock.Verify(r => r.AddAsync(It.IsAny<FishTank>()), Times.Never);
            _unitOfWorkMock.Verify(
                u => u.SaveChangesAsync(It.IsAny<CancellationToken>()),
                Times.Never
            );
        }

        [Fact]
        public async Task CreateFishTankAsync_ShouldReturnBadRequest_WhenFarmIsDeleted()
        {
            // Arrange
            var farmId = Guid.NewGuid();
            var createDto = new CreateFishTankDto
            {
                Name = "Bể cá số 1",
                Height = 2.5f,
                Radius = 3.0f,
                FarmId = farmId,
                CameraUrl = "http://camera.com/stream1",
            };

            var deletedFarm = new Farm
            {
                Id = farmId,
                Name = "Trang trại ABC",
                IsDeleted = true,
                DeletedAt = DateTime.UtcNow,
            };

            _farmRepositoryMock
                .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), QueryType.ActiveOnly))
                .ReturnsAsync(deletedFarm);
            _unitOfWorkMock
                .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            var result = await _sut.CreateFishTankAsync(createDto);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Type.Should().Be(ResultType.BadRequest);
            result.Message.Should().Be("Trang trại không tồn tại.");

            _fishTankRepositoryMock.Verify(r => r.AddAsync(It.IsAny<FishTank>()), Times.Never);
            _unitOfWorkMock.Verify(
                u => u.SaveChangesAsync(It.IsAny<CancellationToken>()),
                Times.Never
            );
        }

        [Fact]
        public async Task CreateFishTankAsync_ShouldReturnUnexpected_WhenExceptionThrown()
        {
            // Arrange
            var createDto = new CreateFishTankDto
            {
                Name = "Bể cá số 1",
                Height = 2.5f,
                Radius = 3.0f,
                FarmId = Guid.NewGuid(),
                CameraUrl = "http://camera.com/stream1",
            };

            _farmRepositoryMock
                .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), QueryType.ActiveOnly))
                .ThrowsAsync(new Exception());
            _unitOfWorkMock
                .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            var result = await _sut.CreateFishTankAsync(createDto);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Type.Should().Be(ResultType.Unexpected);
            result.Message.Should().Be("Lỗi khi tạo bể cá.");
        }

        #endregion

        #region GetFishTankByIdAsync Tests

        [Fact]
        public async Task GetFishTankByIdAsync_ShouldReturnOk_WhenFishTankExists()
        {
            // Arrange
            var fishTankId = Guid.NewGuid();
            var farmId = Guid.NewGuid();
            var fishTank = new FishTank
            {
                Id = fishTankId,
                Name = "Bể cá số 1",
                Height = 2.5f,
                Radius = 3.0f,
                FarmId = farmId,
                TopicCode = "TOPIC001",
                CameraUrl = "http://camera.com/stream1",
                IsDeleted = false,
            };

            var farm = new Farm
            {
                Id = farmId,
                Name = "Trang trại ABC",
                IsDeleted = false,
            };

            _fishTankRepositoryMock
                .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), QueryType.ActiveOnly))
                .ReturnsAsync(fishTank);
            _farmRepositoryMock
                .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), QueryType.ActiveOnly))
                .ReturnsAsync(farm);

            // Act
            var result = await _sut.GetFishTankByIdAsync(fishTankId);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Type.Should().Be(ResultType.Ok);
            result.Message.Should().Be("Lấy thông tin bể cá thành công.");
            result.Data.Should().NotBeNull();
            result.Data!.Id.Should().Be(fishTankId);
            result.Data.Name.Should().Be(fishTank.Name);
            result.Data.Height.Should().Be(fishTank.Height);
            result.Data.Radius.Should().Be(fishTank.Radius);
            result.Data.FarmId.Should().Be(fishTank.FarmId);
            result.Data.FarmName.Should().Be(farm.Name);
            result.Data.TopicCode.Should().Be(fishTank.TopicCode);
            result.Data.CameraUrl.Should().Be(fishTank.CameraUrl);

            _fishTankRepositoryMock.Verify(
                r => r.GetByIdAsync(It.Is<Guid>(id => id == fishTankId), QueryType.ActiveOnly),
                Times.Once
            );
        }

        [Fact]
        public async Task GetFishTankByIdAsync_ShouldReturnNotFound_WhenFishTankNotExists()
        {
            // Arrange
            var fishTankId = Guid.NewGuid();
            _fishTankRepositoryMock
                .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), QueryType.ActiveOnly))
                .ReturnsAsync((FishTank?)null);

            // Act
            var result = await _sut.GetFishTankByIdAsync(fishTankId);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Type.Should().Be(ResultType.NotFound);
            result.Message.Should().Be("Bể cá không tồn tại.");
            result.Data.Should().BeNull();

            _fishTankRepositoryMock.Verify(
                r => r.GetByIdAsync(It.Is<Guid>(id => id == fishTankId), QueryType.ActiveOnly),
                Times.Once
            );
        }

        [Fact]
        public async Task GetFishTankByIdAsync_ShouldReturnNotFound_WhenFishTankIsDeleted()
        {
            // Arrange
            var fishTankId = Guid.NewGuid();
            var deletedFishTank = new FishTank
            {
                Id = fishTankId,
                Name = "Bể cá số 1",
                Height = 2.5f,
                Radius = 3.0f,
                FarmId = Guid.NewGuid(),
                CameraUrl = "http://camera.com/stream1",
                IsDeleted = true,
                DeletedAt = DateTime.UtcNow,
            };

            _fishTankRepositoryMock
                .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), QueryType.ActiveOnly))
                .ReturnsAsync(deletedFishTank);

            // Act
            var result = await _sut.GetFishTankByIdAsync(fishTankId);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Type.Should().Be(ResultType.NotFound);
            result.Message.Should().Be("Bể cá không tồn tại.");

            _fishTankRepositoryMock.Verify(
                r => r.GetByIdAsync(It.Is<Guid>(id => id == fishTankId), QueryType.ActiveOnly),
                Times.Once
            );
        }

        [Fact]
        public async Task GetFishTankByIdAsync_ShouldReturnUnexpected_WhenThrownException()
        {
            // Arrange
            var fishTankId = Guid.NewGuid();
            _fishTankRepositoryMock
                .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), QueryType.ActiveOnly))
                .ThrowsAsync(new Exception());

            // Act
            var result = await _sut.GetFishTankByIdAsync(fishTankId);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Type.Should().Be(ResultType.Unexpected);
            result.Message.Should().Be("Lỗi khi truy xuất thông tin bể cá.");

            _fishTankRepositoryMock.Verify(
                r => r.GetByIdAsync(It.Is<Guid>(id => id == fishTankId), QueryType.ActiveOnly),
                Times.Once
            );
        }

        #endregion

        #region GetAllFishTanksAsync Tests

        [Fact]
        public async Task GetAllFishTanksAsync_ShouldReturnOk_WhenSuccessful()
        {
            // Arrange
            var farmId1 = Guid.NewGuid();
            var farmId2 = Guid.NewGuid();

            var fishTankList = new List<FishTank>
            {
                new FishTank
                {
                    Id = Guid.NewGuid(),
                    Name = "Bể cá số 1",
                    Height = 2.5f,
                    Radius = 3.0f,
                    FarmId = farmId1,
                    CameraUrl = "http://camera.com/stream1",
                    IsDeleted = false,
                },
                new FishTank
                {
                    Id = Guid.NewGuid(),
                    Name = "Bể cá số 2",
                    Height = 3.0f,
                    Radius = 4.0f,
                    FarmId = farmId2,
                    CameraUrl = "http://camera.com/stream2",
                    IsDeleted = false,
                },
            };

            var farmList = new List<Farm>
            {
                new Farm
                {
                    Id = farmId1,
                    Name = "Trang trại ABC",
                    IsDeleted = false,
                },
                new Farm
                {
                    Id = farmId2,
                    Name = "Trang trại XYZ",
                    IsDeleted = false,
                },
            };

            _fishTankRepositoryMock
                .Setup(r =>
                    r.FindAllAsync(
                        It.IsAny<Expression<Func<FishTank, bool>>>(),
                        It.IsAny<QueryType>()
                    )
                )
                .ReturnsAsync(fishTankList);
            _farmRepositoryMock
                .Setup(r =>
                    r.FindAllAsync(It.IsAny<Expression<Func<Farm, bool>>>(), It.IsAny<QueryType>())
                )
                .ReturnsAsync(farmList);

            // Act
            var result = await _sut.GetAllFishTanksAsync();

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Type.Should().Be(ResultType.Ok);
            result.Message.Should().Be("Lấy danh sách bể cá thành công.");
            result.Data.Should().NotBeNull();
            result.Data!.Count().Should().Be(2);

            _fishTankRepositoryMock.Verify(
                r =>
                    r.FindAllAsync(
                        It.IsAny<Expression<Func<FishTank, bool>>>(),
                        It.IsAny<QueryType>()
                    ),
                Times.Once
            );
        }

        [Fact]
        public async Task GetAllFishTanksAsync_ShouldReturnEmptyList_WhenNoRecordsExist()
        {
            // Arrange
            var emptyFishTankList = new List<FishTank>();
            var emptyFarmList = new List<Farm>();

            _fishTankRepositoryMock
                .Setup(r =>
                    r.FindAllAsync(
                        It.IsAny<Expression<Func<FishTank, bool>>>(),
                        It.IsAny<QueryType>()
                    )
                )
                .ReturnsAsync(emptyFishTankList);
            _farmRepositoryMock
                .Setup(r =>
                    r.FindAllAsync(It.IsAny<Expression<Func<Farm, bool>>>(), It.IsAny<QueryType>())
                )
                .ReturnsAsync(emptyFarmList);

            // Act
            var result = await _sut.GetAllFishTanksAsync();

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Type.Should().Be(ResultType.Ok);
            result.Message.Should().Be("Lấy danh sách bể cá thành công.");
            result.Data.Should().NotBeNull();
            result.Data!.Should().BeEmpty();

            _fishTankRepositoryMock.Verify(
                r =>
                    r.FindAllAsync(
                        It.IsAny<Expression<Func<FishTank, bool>>>(),
                        It.IsAny<QueryType>()
                    ),
                Times.Once
            );
        }

        [Fact]
        public async Task GetAllFishTanksAsync_ShouldReturnUnexpected_WhenThrownException()
        {
            // Arrange
            _fishTankRepositoryMock
                .Setup(r =>
                    r.FindAllAsync(
                        It.IsAny<Expression<Func<FishTank, bool>>>(),
                        It.IsAny<QueryType>()
                    )
                )
                .ThrowsAsync(new Exception());

            // Act
            var result = await _sut.GetAllFishTanksAsync();

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Type.Should().Be(ResultType.Unexpected);
            result.Message.Should().Be("Lỗi khi truy xuất danh sách bể cá.");

            _fishTankRepositoryMock.Verify(
                r =>
                    r.FindAllAsync(
                        It.IsAny<Expression<Func<FishTank, bool>>>(),
                        It.IsAny<QueryType>()
                    ),
                Times.Once
            );
        }

        #endregion

        #region DeleteFishTankAsync Tests

        [Fact]
        public async Task DeleteFishTankAsync_ShouldReturnOk_WhenSuccessful()
        {
            // Arrange
            var fishTankId = Guid.NewGuid();
            var fishTank = new FishTank
            {
                Id = fishTankId,
                Name = "Bể cá số 1",
                Height = 2.5f,
                Radius = 3.0f,
                FarmId = Guid.NewGuid(),
                CameraUrl = "http://camera.com/stream1",
                IsDeleted = false,
            };

            _fishTankRepositoryMock
                .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), QueryType.ActiveOnly))
                .ReturnsAsync(fishTank);
            _fishTankRepositoryMock.Setup(r => r.Update(It.IsAny<FishTank>())).Verifiable();
            _unitOfWorkMock
                .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            var result = await _sut.DeleteFishTankAsync(fishTankId);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Type.Should().Be(ResultType.Ok);
            result.Message.Should().Be("Xóa bể cá thành công.");
            fishTank.IsDeleted.Should().BeTrue();
            fishTank.DeletedAt.Should().NotBeNull();
            fishTank.DeletedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));

            _fishTankRepositoryMock.Verify(
                r => r.GetByIdAsync(It.Is<Guid>(id => id == fishTankId), QueryType.ActiveOnly),
                Times.Once
            );
            _fishTankRepositoryMock.Verify(
                r => r.Update(It.Is<FishTank>(ft => ft.Id == fishTankId && ft.IsDeleted == true)),
                Times.Once
            );
            _unitOfWorkMock.Verify(
                u => u.SaveChangesAsync(It.IsAny<CancellationToken>()),
                Times.Once
            );
        }

        [Fact]
        public async Task DeleteFishTankAsync_ShouldReturnNotFound_WhenFishTankNotExists()
        {
            // Arrange
            var fishTankId = Guid.NewGuid();
            _fishTankRepositoryMock
                .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), QueryType.ActiveOnly))
                .ReturnsAsync((FishTank?)null);

            // Act
            var result = await _sut.DeleteFishTankAsync(fishTankId);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Type.Should().Be(ResultType.NotFound);
            result.Message.Should().Be("Bể cá không tồn tại.");

            _fishTankRepositoryMock.Verify(
                r => r.GetByIdAsync(It.Is<Guid>(id => id == fishTankId), QueryType.ActiveOnly),
                Times.Once
            );
            _fishTankRepositoryMock.Verify(r => r.Update(It.IsAny<FishTank>()), Times.Never);
            _unitOfWorkMock.Verify(
                u => u.SaveChangesAsync(It.IsAny<CancellationToken>()),
                Times.Never
            );
        }

        [Fact]
        public async Task DeleteFishTankAsync_ShouldReturnNotFound_WhenFishTankAlreadyDeleted()
        {
            // Arrange
            var fishTankId = Guid.NewGuid();
            var deletedFishTank = new FishTank
            {
                Id = fishTankId,
                Name = "Bể cá số 1",
                IsDeleted = true,
                DeletedAt = DateTime.UtcNow.AddDays(-1),
            };

            _fishTankRepositoryMock
                .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), QueryType.ActiveOnly))
                .ReturnsAsync(deletedFishTank);

            // Act
            var result = await _sut.DeleteFishTankAsync(fishTankId);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Type.Should().Be(ResultType.NotFound);
            result.Message.Should().Be("Bể cá không tồn tại.");

            _fishTankRepositoryMock.Verify(r => r.Update(It.IsAny<FishTank>()), Times.Never);
            _unitOfWorkMock.Verify(
                u => u.SaveChangesAsync(It.IsAny<CancellationToken>()),
                Times.Never
            );
        }

        [Fact]
        public async Task DeleteFishTankAsync_ShouldReturnUnexpected_WhenThrownException()
        {
            // Arrange
            var fishTankId = Guid.NewGuid();
            _fishTankRepositoryMock
                .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), QueryType.ActiveOnly))
                .ThrowsAsync(new Exception());

            // Act
            var result = await _sut.DeleteFishTankAsync(fishTankId);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Type.Should().Be(ResultType.Unexpected);
            result.Message.Should().Be("Lỗi khi xóa bể cá.");

            _fishTankRepositoryMock.Verify(
                r => r.GetByIdAsync(It.Is<Guid>(id => id == fishTankId), QueryType.ActiveOnly),
                Times.Once
            );
        }

        #endregion

        #region UpdateFishTankAsync Tests

        [Fact]
        public async Task UpdateFishTankAsync_ShouldReturnOk_WhenSuccessful()
        {
            // Arrange
            var fishTankId = Guid.NewGuid();
            var newFarmId = Guid.NewGuid();
            var existingFishTank = new FishTank
            {
                Id = fishTankId,
                Name = "Bể cá số 1",
                Height = 2.5f,
                Radius = 3.0f,
                FarmId = Guid.NewGuid(),
                TopicCode = "TOPIC001",
                CameraUrl = "http://camera.com/stream1",
                IsDeleted = false,
            };
            var updateDto = new UpdateFishTankDto
            {
                Name = "Bể cá số 1 - Cập nhật",
                Height = 3.5f,
                Radius = 4.5f,
                FarmId = newFarmId,
                TopicCode = "TOPIC002",
                CameraUrl = "http://camera.com/stream2",
            };

            var newFarm = new Farm
            {
                Id = newFarmId,
                Name = "Trang trại mới",
                IsDeleted = false,
            };

            _fishTankRepositoryMock
                .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), QueryType.ActiveOnly))
                .ReturnsAsync(existingFishTank);
            _farmRepositoryMock
                .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), QueryType.ActiveOnly))
                .ReturnsAsync(newFarm);
            _fishTankRepositoryMock.Setup(r => r.Update(It.IsAny<FishTank>())).Verifiable();
            _unitOfWorkMock
                .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            var result = await _sut.UpdateFishTankAsync(fishTankId, updateDto);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Type.Should().Be(ResultType.Ok);
            result.Message.Should().Be("Cập nhật bể cá thành công.");

            existingFishTank.Name.Should().Be(updateDto.Name);
            existingFishTank.Height.Should().Be(updateDto.Height!.Value);
            existingFishTank.Radius.Should().Be(updateDto.Radius!.Value);
            existingFishTank.FarmId.Should().Be(updateDto.FarmId!.Value);
            existingFishTank.TopicCode.Should().Be(updateDto.TopicCode);
            existingFishTank.CameraUrl.Should().Be(updateDto.CameraUrl);

            _fishTankRepositoryMock.Verify(
                r => r.Update(It.Is<FishTank>(ft => ft.Id == fishTankId)),
                Times.Once
            );
            _unitOfWorkMock.Verify(
                u => u.SaveChangesAsync(It.IsAny<CancellationToken>()),
                Times.Once
            );
        }

        [Theory]
        [InlineData(null, "Bể cá số 1")]
        [InlineData("", "Bể cá số 1")]
        [InlineData(" ", "Bể cá số 1")]
        [InlineData("  ", "Bể cá số 1")]
        public async Task UpdateFishTankAsync_ShouldNotUpdateName_WhenNameIsNullOrWhitespace(
            string nameInput,
            string expectedName
        )
        {
            // Arrange
            var fishTankId = Guid.NewGuid();
            var existingFishTank = new FishTank
            {
                Id = fishTankId,
                Name = "Bể cá số 1",
                Height = 2.5f,
                Radius = 3.0f,
                FarmId = Guid.NewGuid(),
                CameraUrl = "http://camera.com/stream1",
                IsDeleted = false,
            };
            var updateDto = new UpdateFishTankDto { Name = nameInput };

            _fishTankRepositoryMock
                .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), QueryType.ActiveOnly))
                .ReturnsAsync(existingFishTank);
            _fishTankRepositoryMock.Setup(r => r.Update(It.IsAny<FishTank>())).Verifiable();
            _unitOfWorkMock
                .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            var result = await _sut.UpdateFishTankAsync(fishTankId, updateDto);

            // Assert
            result.IsSuccess.Should().BeTrue();
            existingFishTank.Name.Should().Be(expectedName);
        }

        [Fact]
        public async Task UpdateFishTankAsync_ShouldTrimInputs_WhenUpdating()
        {
            // Arrange
            var fishTankId = Guid.NewGuid();
            var existingFishTank = new FishTank
            {
                Id = fishTankId,
                Name = "Bể cá số 1",
                Height = 2.5f,
                Radius = 3.0f,
                FarmId = Guid.NewGuid(),
                CameraUrl = "http://camera.com/stream1",
                IsDeleted = false,
            };
            var updateDto = new UpdateFishTankDto
            {
                Name = "  Bể cá số 1 - Cập nhật  ",
                TopicCode = "  TOPIC002  ",
                CameraUrl = "  http://camera.com/stream2  ",
            };

            _fishTankRepositoryMock
                .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), QueryType.ActiveOnly))
                .ReturnsAsync(existingFishTank);
            _fishTankRepositoryMock.Setup(r => r.Update(It.IsAny<FishTank>())).Verifiable();
            _unitOfWorkMock
                .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            var result = await _sut.UpdateFishTankAsync(fishTankId, updateDto);

            // Assert
            result.IsSuccess.Should().BeTrue();
            existingFishTank.Name.Should().Be("Bể cá số 1 - Cập nhật");
            existingFishTank.TopicCode.Should().Be("TOPIC002");
            existingFishTank.CameraUrl.Should().Be("http://camera.com/stream2");
        }

        [Fact]
        public async Task UpdateFishTankAsync_ShouldReturnNotFound_WhenFishTankNotExists()
        {
            // Arrange
            var fishTankId = Guid.NewGuid();
            var updateDto = new UpdateFishTankDto { Name = "Bể cá số 1 - Cập nhật" };

            _fishTankRepositoryMock
                .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), QueryType.ActiveOnly))
                .ReturnsAsync((FishTank?)null);

            // Act
            var result = await _sut.UpdateFishTankAsync(fishTankId, updateDto);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Type.Should().Be(ResultType.NotFound);
            result.Message.Should().Be("Bể cá không tồn tại.");

            _fishTankRepositoryMock.Verify(
                r => r.GetByIdAsync(It.Is<Guid>(id => id == fishTankId), QueryType.ActiveOnly),
                Times.Once
            );
            _fishTankRepositoryMock.Verify(r => r.Update(It.IsAny<FishTank>()), Times.Never);
            _unitOfWorkMock.Verify(
                u => u.SaveChangesAsync(It.IsAny<CancellationToken>()),
                Times.Never
            );
        }

        [Fact]
        public async Task UpdateFishTankAsync_ShouldReturnNotFound_WhenFishTankIsDeleted()
        {
            // Arrange
            var fishTankId = Guid.NewGuid();
            var deletedFishTank = new FishTank
            {
                Id = fishTankId,
                Name = "Bể cá số 1",
                IsDeleted = true,
                DeletedAt = DateTime.UtcNow.AddDays(-1),
            };
            var updateDto = new UpdateFishTankDto { Name = "Bể cá số 1 - Cập nhật" };

            _fishTankRepositoryMock
                .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), QueryType.ActiveOnly))
                .ReturnsAsync(deletedFishTank);

            // Act
            var result = await _sut.UpdateFishTankAsync(fishTankId, updateDto);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Type.Should().Be(ResultType.NotFound);
            result.Message.Should().Be("Bể cá không tồn tại.");

            _fishTankRepositoryMock.Verify(r => r.Update(It.IsAny<FishTank>()), Times.Never);
            _unitOfWorkMock.Verify(
                u => u.SaveChangesAsync(It.IsAny<CancellationToken>()),
                Times.Never
            );
        }

        [Theory]
        [InlineData(0, "Chiều cao phải lớn hơn 0.")]
        [InlineData(-1, "Chiều cao phải lớn hơn 0.")]
        [InlineData(-2.5f, "Chiều cao phải lớn hơn 0.")]
        public async Task UpdateFishTankAsync_ShouldReturnBadRequest_WhenHeightIsInvalid(
            float height,
            string expectedMessage
        )
        {
            // Arrange
            var fishTankId = Guid.NewGuid();
            var existingFishTank = new FishTank
            {
                Id = fishTankId,
                Name = "Bể cá số 1",
                Height = 2.5f,
                Radius = 3.0f,
                FarmId = Guid.NewGuid(),
                CameraUrl = "http://camera.com/stream1",
                IsDeleted = false,
            };
            var updateDto = new UpdateFishTankDto { Height = height };

            _fishTankRepositoryMock
                .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), QueryType.ActiveOnly))
                .ReturnsAsync(existingFishTank);

            // Act
            var result = await _sut.UpdateFishTankAsync(fishTankId, updateDto);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Type.Should().Be(ResultType.BadRequest);
            result.Message.Should().Be(expectedMessage);

            _fishTankRepositoryMock.Verify(r => r.Update(It.IsAny<FishTank>()), Times.Never);
            _unitOfWorkMock.Verify(
                u => u.SaveChangesAsync(It.IsAny<CancellationToken>()),
                Times.Never
            );
        }

        [Theory]
        [InlineData(0, "Bán kính phải lớn hơn 0.")]
        [InlineData(-1, "Bán kính phải lớn hơn 0.")]
        [InlineData(-3.5f, "Bán kính phải lớn hơn 0.")]
        public async Task UpdateFishTankAsync_ShouldReturnBadRequest_WhenRadiusIsInvalid(
            float radius,
            string expectedMessage
        )
        {
            // Arrange
            var fishTankId = Guid.NewGuid();
            var existingFishTank = new FishTank
            {
                Id = fishTankId,
                Name = "Bể cá số 1",
                Height = 2.5f,
                Radius = 3.0f,
                FarmId = Guid.NewGuid(),
                CameraUrl = "http://camera.com/stream1",
                IsDeleted = false,
            };
            var updateDto = new UpdateFishTankDto { Radius = radius };

            _fishTankRepositoryMock
                .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), QueryType.ActiveOnly))
                .ReturnsAsync(existingFishTank);

            // Act
            var result = await _sut.UpdateFishTankAsync(fishTankId, updateDto);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Type.Should().Be(ResultType.BadRequest);
            result.Message.Should().Be(expectedMessage);

            _fishTankRepositoryMock.Verify(r => r.Update(It.IsAny<FishTank>()), Times.Never);
            _unitOfWorkMock.Verify(
                u => u.SaveChangesAsync(It.IsAny<CancellationToken>()),
                Times.Never
            );
        }

        [Fact]
        public async Task UpdateFishTankAsync_ShouldReturnBadRequest_WhenNewFarmNotExists()
        {
            // Arrange
            var fishTankId = Guid.NewGuid();
            var newFarmId = Guid.NewGuid();
            var existingFishTank = new FishTank
            {
                Id = fishTankId,
                Name = "Bể cá số 1",
                Height = 2.5f,
                Radius = 3.0f,
                FarmId = Guid.NewGuid(),
                CameraUrl = "http://camera.com/stream1",
                IsDeleted = false,
            };
            var updateDto = new UpdateFishTankDto { FarmId = newFarmId };

            _fishTankRepositoryMock
                .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), QueryType.ActiveOnly))
                .ReturnsAsync(existingFishTank);
            _farmRepositoryMock
                .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), QueryType.ActiveOnly))
                .ReturnsAsync((Farm?)null);

            // Act
            var result = await _sut.UpdateFishTankAsync(fishTankId, updateDto);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Type.Should().Be(ResultType.BadRequest);
            result.Message.Should().Be("Trang trại không tồn tại.");

            _fishTankRepositoryMock.Verify(r => r.Update(It.IsAny<FishTank>()), Times.Never);
            _unitOfWorkMock.Verify(
                u => u.SaveChangesAsync(It.IsAny<CancellationToken>()),
                Times.Never
            );
        }

        [Fact]
        public async Task UpdateFishTankAsync_ShouldReturnBadRequest_WhenNewFarmIsDeleted()
        {
            // Arrange
            var fishTankId = Guid.NewGuid();
            var newFarmId = Guid.NewGuid();
            var existingFishTank = new FishTank
            {
                Id = fishTankId,
                Name = "Bể cá số 1",
                Height = 2.5f,
                Radius = 3.0f,
                FarmId = Guid.NewGuid(),
                CameraUrl = "http://camera.com/stream1",
                IsDeleted = false,
            };
            var deletedFarm = new Farm
            {
                Id = newFarmId,
                Name = "Trang trại XYZ",
                IsDeleted = true,
                DeletedAt = DateTime.UtcNow,
            };
            var updateDto = new UpdateFishTankDto { FarmId = newFarmId };

            _fishTankRepositoryMock
                .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), QueryType.ActiveOnly))
                .ReturnsAsync(existingFishTank);
            _farmRepositoryMock
                .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), QueryType.ActiveOnly))
                .ReturnsAsync(deletedFarm);

            // Act
            var result = await _sut.UpdateFishTankAsync(fishTankId, updateDto);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Type.Should().Be(ResultType.BadRequest);
            result.Message.Should().Be("Trang trại không tồn tại.");

            _fishTankRepositoryMock.Verify(r => r.Update(It.IsAny<FishTank>()), Times.Never);
            _unitOfWorkMock.Verify(
                u => u.SaveChangesAsync(It.IsAny<CancellationToken>()),
                Times.Never
            );
        }

        [Fact]
        public async Task UpdateFishTankAsync_ShouldReturnUnexpected_WhenExceptionThrown()
        {
            // Arrange
            var fishTankId = Guid.NewGuid();
            var updateDto = new UpdateFishTankDto { Name = "Bể cá số 1 - Cập nhật" };

            _fishTankRepositoryMock
                .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), QueryType.ActiveOnly))
                .ThrowsAsync(new Exception());

            // Act
            var result = await _sut.UpdateFishTankAsync(fishTankId, updateDto);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Type.Should().Be(ResultType.Unexpected);
            result.Message.Should().Be("Lỗi khi cập nhật bể cá.");

            _fishTankRepositoryMock.Verify(
                r => r.GetByIdAsync(It.Is<Guid>(id => id == fishTankId), QueryType.ActiveOnly),
                Times.Once
            );
        }

        #endregion
    }
}

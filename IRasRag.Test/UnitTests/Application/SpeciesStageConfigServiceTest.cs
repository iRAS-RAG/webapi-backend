using System.Linq.Expressions;
using AutoMapper;
using FluentAssertions;
using IRasRag.Application.Common.Interfaces.Persistence;
using IRasRag.Application.Common.Mappings;
using IRasRag.Application.Common.Models;
using IRasRag.Application.DTOs;
using IRasRag.Application.Services.Implementations;
using IRasRag.Application.Specifications;
using IRasRag.Domain.Entities;
using IRasRag.Domain.Enums;
using IRasRag.Test.UnitTests.Helpers;
using Microsoft.Extensions.Logging;
using Moq;

namespace IRasRag.Test.UnitTests.Application
{
    public class SpeciesStageConfigServiceTest
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<ILogger<SpeciesStageConfigService>> _loggerMock;
        private readonly Mock<IRepository<SpeciesStageConfig>> _configRepoMock;
        private readonly Mock<IRepository<Species>> _speciesRepoMock;
        private readonly Mock<IRepository<GrowthStage>> _growthStageRepoMock;
        private readonly Mock<IRepository<FeedType>> _feedTypeRepoMock;
        private readonly IMapper _mapper;
        private readonly SpeciesStageConfigService _sut;

        public SpeciesStageConfigServiceTest()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _loggerMock = new Mock<ILogger<SpeciesStageConfigService>>();
            _mapper = AutoMapperTestHelper.GetMapper(new SpeciesStageConfigProfile());

            _configRepoMock = new Mock<IRepository<SpeciesStageConfig>>();
            _speciesRepoMock = new Mock<IRepository<Species>>();
            _feedTypeRepoMock = new Mock<IRepository<FeedType>>();
            _growthStageRepoMock = new Mock<IRepository<GrowthStage>>();

            _unitOfWorkMock
                .Setup(u => u.GetRepository<SpeciesStageConfig>())
                .Returns(_configRepoMock.Object);
            _unitOfWorkMock.Setup(u => u.GetRepository<Species>()).Returns(_speciesRepoMock.Object);
            _unitOfWorkMock
                .Setup(u => u.GetRepository<GrowthStage>())
                .Returns(_growthStageRepoMock.Object);
            _unitOfWorkMock
                .Setup(u => u.GetRepository<FeedType>())
                .Returns(_feedTypeRepoMock.Object);

            _sut = new SpeciesStageConfigService(
                _unitOfWorkMock.Object,
                _loggerMock.Object,
                _mapper
            );
        }

        #region CreateSpeciesStageConfig Tests

        [Fact]
        public async Task CreateSpeciesStageConfig_ShouldReturnOk_WhenSuccessful()
        {
            // Arrange
            var dto = new CreateSpeciesStageConfigDto
            {
                SpeciesId = Guid.NewGuid(),
                GrowthStageId = Guid.NewGuid(),
                FeedTypeId = Guid.NewGuid(),
                AmountPer100Fish = 100,
                FrequencyPerDay = 3,
                MaxStockingDensity = 50,
                ExpectedDurationDays = 30,
            };

            _speciesRepoMock
                .Setup(r =>
                    r.AnyAsync(It.IsAny<Expression<Func<Species, bool>>>(), QueryType.ActiveOnly)
                )
                .ReturnsAsync(true);

            _growthStageRepoMock
                .Setup(r =>
                    r.AnyAsync(
                        It.IsAny<Expression<Func<GrowthStage, bool>>>(),
                        QueryType.ActiveOnly
                    )
                )
                .ReturnsAsync(true);

            _feedTypeRepoMock
                .Setup(r =>
                    r.AnyAsync(It.IsAny<Expression<Func<FeedType, bool>>>(), QueryType.ActiveOnly)
                )
                .ReturnsAsync(true);

            _configRepoMock
                .Setup(r =>
                    r.AnyAsync(
                        It.IsAny<Expression<Func<SpeciesStageConfig, bool>>>(),
                        QueryType.ActiveOnly
                    )
                )
                .ReturnsAsync(false);
            _unitOfWorkMock.Setup(x => x.SaveChangesAsync(default)).ReturnsAsync(1);
            // Act
            var result = await _sut.CreateSpeciesStageConfig(dto);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Type.Should().Be(ResultType.Ok);
            result.Message.Should().Be("Tạo cấu hình giai đoạn sinh trưởng của cá thành công.");
            result.Data.Should().NotBeNull();

            _configRepoMock.Verify(r => r.AddAsync(It.IsAny<SpeciesStageConfig>()), Times.Once);

            _unitOfWorkMock.Verify(
                u => u.SaveChangesAsync(It.IsAny<CancellationToken>()),
                Times.Once
            );
        }

        [Fact]
        public async Task CreateSpeciesStageConfig_ShouldReturnBadRequest_WhenSpeciesNotExists()
        {
            // Arrange
            var dto = new CreateSpeciesStageConfigDto
            {
                SpeciesId = Guid.NewGuid(),
                GrowthStageId = Guid.NewGuid(),
                FeedTypeId = Guid.NewGuid(),
                AmountPer100Fish = 100,
                FrequencyPerDay = 3,
                MaxStockingDensity = 50,
                ExpectedDurationDays = 30,
            };

            _speciesRepoMock
                .Setup(r =>
                    r.AnyAsync(It.IsAny<Expression<Func<Species, bool>>>(), QueryType.ActiveOnly)
                )
                .ReturnsAsync(false);
            _unitOfWorkMock.Setup(x => x.SaveChangesAsync(default)).ReturnsAsync(1);
            // Act
            var result = await _sut.CreateSpeciesStageConfig(dto);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Type.Should().Be(ResultType.BadRequest);
            result.Message.Should().Be("Loài cá không tồn tại.");

            _configRepoMock.Verify(r => r.AddAsync(It.IsAny<SpeciesStageConfig>()), Times.Never);
            _unitOfWorkMock.Verify(
                u => u.SaveChangesAsync(It.IsAny<CancellationToken>()),
                Times.Never
            );
        }

        [Fact]
        public async Task CreateSpeciesStageConfig_ShouldReturnBadRequest_WhenGrowthStageNotExists()
        {
            // Arrange
            var dto = new CreateSpeciesStageConfigDto
            {
                SpeciesId = Guid.NewGuid(),
                GrowthStageId = Guid.NewGuid(),
                FeedTypeId = Guid.NewGuid(),
                AmountPer100Fish = 100,
                FrequencyPerDay = 3,
                MaxStockingDensity = 50,
                ExpectedDurationDays = 30,
            };

            _speciesRepoMock
                .Setup(r =>
                    r.AnyAsync(It.IsAny<Expression<Func<Species, bool>>>(), QueryType.ActiveOnly)
                )
                .ReturnsAsync(true);

            _growthStageRepoMock
                .Setup(r =>
                    r.AnyAsync(
                        It.IsAny<Expression<Func<GrowthStage, bool>>>(),
                        QueryType.ActiveOnly
                    )
                )
                .ReturnsAsync(false);
            _unitOfWorkMock.Setup(x => x.SaveChangesAsync(default)).ReturnsAsync(1);
            // Act
            var result = await _sut.CreateSpeciesStageConfig(dto);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Type.Should().Be(ResultType.BadRequest);
            result.Message.Should().Be("Giai đoạn sinh trưởng không tồn tại.");
        }

        [Fact]
        public async Task CreateSpeciesStageConfig_ShouldReturnBadRequest_WhenFeedTypeNotExists()
        {
            // Arrange
            var dto = new CreateSpeciesStageConfigDto
            {
                SpeciesId = Guid.NewGuid(),
                GrowthStageId = Guid.NewGuid(),
                FeedTypeId = Guid.NewGuid(),
                AmountPer100Fish = 100,
                FrequencyPerDay = 3,
                MaxStockingDensity = 50,
                ExpectedDurationDays = 30,
            };

            _speciesRepoMock
                .Setup(r =>
                    r.AnyAsync(It.IsAny<Expression<Func<Species, bool>>>(), QueryType.ActiveOnly)
                )
                .ReturnsAsync(true);

            _growthStageRepoMock
                .Setup(r =>
                    r.AnyAsync(
                        It.IsAny<Expression<Func<GrowthStage, bool>>>(),
                        QueryType.ActiveOnly
                    )
                )
                .ReturnsAsync(true);

            _feedTypeRepoMock
                .Setup(r =>
                    r.AnyAsync(It.IsAny<Expression<Func<FeedType, bool>>>(), QueryType.ActiveOnly)
                )
                .ReturnsAsync(false);
            _unitOfWorkMock.Setup(x => x.SaveChangesAsync(default)).ReturnsAsync(1);
            // Act
            var result = await _sut.CreateSpeciesStageConfig(dto);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Type.Should().Be(ResultType.BadRequest);
            result.Message.Should().Be("Kiểu cho ăn không tồn tại.");

            _configRepoMock.Verify(r => r.AddAsync(It.IsAny<SpeciesStageConfig>()), Times.Never);
        }

        [Fact]
        public async Task CreateSpeciesStageConfig_ShouldReturnConflict_WhenConfigAlreadyExists()
        {
            // Arrange
            var dto = new CreateSpeciesStageConfigDto
            {
                SpeciesId = Guid.NewGuid(),
                GrowthStageId = Guid.NewGuid(),
                FeedTypeId = Guid.NewGuid(),
                AmountPer100Fish = 100,
                FrequencyPerDay = 3,
                MaxStockingDensity = 50,
                ExpectedDurationDays = 30,
            };

            _speciesRepoMock
                .Setup(r =>
                    r.AnyAsync(It.IsAny<Expression<Func<Species, bool>>>(), QueryType.ActiveOnly)
                )
                .ReturnsAsync(true);

            _growthStageRepoMock
                .Setup(r =>
                    r.AnyAsync(
                        It.IsAny<Expression<Func<GrowthStage, bool>>>(),
                        QueryType.ActiveOnly
                    )
                )
                .ReturnsAsync(true);

            _feedTypeRepoMock
                .Setup(r =>
                    r.AnyAsync(It.IsAny<Expression<Func<FeedType, bool>>>(), QueryType.ActiveOnly)
                )
                .ReturnsAsync(true);

            _configRepoMock
                .Setup(r =>
                    r.AnyAsync(
                        It.IsAny<Expression<Func<SpeciesStageConfig, bool>>>(),
                        QueryType.ActiveOnly
                    )
                )
                .ReturnsAsync(true);
            _unitOfWorkMock.Setup(x => x.SaveChangesAsync(default)).ReturnsAsync(1);
            // Act
            var result = await _sut.CreateSpeciesStageConfig(dto);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Type.Should().Be(ResultType.Conflict);
            result
                .Message.Should()
                .Be("Cấu hình giai đoạn sinh trưởng của cá ở giai đoạn này đã tồn tại.");
        }

        [Fact]
        public async Task CreateSpeciesStageConfig_ShouldReturnUnexpected_WhenExceptionThrown()
        {
            // Arrange
            var dto = new CreateSpeciesStageConfigDto
            {
                SpeciesId = Guid.NewGuid(),
                GrowthStageId = Guid.NewGuid(),
                FeedTypeId = Guid.NewGuid(),
                AmountPer100Fish = 100,
                FrequencyPerDay = 3,
                MaxStockingDensity = 50,
                ExpectedDurationDays = 30,
            };

            _speciesRepoMock
                .Setup(r =>
                    r.AnyAsync(It.IsAny<Expression<Func<Species, bool>>>(), QueryType.ActiveOnly)
                )
                .ThrowsAsync(new Exception());
            _unitOfWorkMock.Setup(x => x.SaveChangesAsync(default)).ReturnsAsync(1);
            // Act
            var result = await _sut.CreateSpeciesStageConfig(dto);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Type.Should().Be(ResultType.Unexpected);
            result.Message.Should().Be("Lỗi khi tạo cấu hình giai đoạn sinh trưởng của cá.");
        }

        #endregion

        #region GetSpeciesStageConfigById Tests

        [Fact]
        public async Task GetSpeciesStageConfigById_ShouldReturnOk_WhenExists()
        {
            // Arrange
            var id = Guid.NewGuid();
            var config = new SpeciesStageConfigDto
            {
                Id = id,
                SpeciesName = "Tilapia",
                GrowthStageName = "Juvenile",
                FeedTypeName = "Pellet",
                AmountPer100Fish = 100,
                FrequencyPerDay = 3,
                MaxStockingDensity = 50,
                ExpectedDurationDays = 30,
            };

            _configRepoMock
                .Setup(r =>
                    r.FirstOrDefaultAsync<SpeciesStageConfigDto>(
                        It.IsAny<SpeciesStageConfigByIdSpec>(),
                        QueryType.ActiveOnly
                    )
                )
                .ReturnsAsync(config);

            // Act
            var result = await _sut.GetSpeciesStageConfigById(id);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Type.Should().Be(ResultType.Ok);
            result.Message.Should().Be("Lấy cấu hình giai đoạn sinh trưởng của cá thành công.");
            result.Data.Should().NotBeNull();
            result.Data.Should().BeEquivalentTo(config);
        }

        [Fact]
        public async Task GetSpeciesStageConfigById_ShouldReturnNotFound_WhenNotExists()
        {
            // Arrange
            _configRepoMock
                .Setup(r =>
                    r.FirstOrDefaultAsync<SpeciesStageConfigDto>(
                        It.IsAny<SpeciesStageConfigByIdSpec>(),
                        QueryType.ActiveOnly
                    )
                )
                .ReturnsAsync((SpeciesStageConfigDto?)null);

            // Act
            var result = await _sut.GetSpeciesStageConfigById(Guid.NewGuid());

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Type.Should().Be(ResultType.NotFound);
            result.Message.Should().Be("Cấu hình giai đoạn sinh trưởng của cá không tồn tại.");
        }

        [Fact]
        public async Task GetSpeciesStageConfigById_ShouldReturnUnexpected_WhenExceptionThrown()
        {
            // Arrange
            _configRepoMock
                .Setup(r =>
                    r.FirstOrDefaultAsync<SpeciesStageConfigDto>(
                        It.IsAny<SpeciesStageConfigByIdSpec>(),
                        QueryType.ActiveOnly
                    )
                )
                .ThrowsAsync(new Exception());

            // Act
            var result = await _sut.GetSpeciesStageConfigById(Guid.NewGuid());

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Type.Should().Be(ResultType.Unexpected);
            result.Message.Should().Be("Lỗi khi truy xuất cấu hình giai đoạn sinh trưởng của cá.");
        }

        #endregion

        #region GetAllSpeciesStageConfigsAsync Tests

        [Fact]
        public async Task GetAllSpeciesStageConfigsAsync_ShouldReturnOk_WhenSuccessful()
        {
            // Arrange
            var configs = new List<SpeciesStageConfigDto>
            {
                new SpeciesStageConfigDto { Id = Guid.NewGuid() },
                new SpeciesStageConfigDto { Id = Guid.NewGuid() },
            };

            _configRepoMock
                .Setup(r =>
                    r.ListAsync<SpeciesStageConfigDto>(
                        It.IsAny<SpeciesStageConfigListSpec>(),
                        QueryType.ActiveOnly
                    )
                )
                .ReturnsAsync(configs);

            // Act
            var result = await _sut.GetAllSpeciesStageConfigsAsync();

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Type.Should().Be(ResultType.Ok);
            result
                .Message.Should()
                .Be("Lấy danh sách cấu hình giai đoạn sinh trưởng của cá thành công.");
            result.Data.Should().HaveCount(2);
        }

        [Fact]
        public async Task GetAllSpeciesStageConfigsAsync_ShouldReturnEmptyList_WhenNoConfigsExist()
        {
            // Arrange
            var configs = new List<SpeciesStageConfigDto>();

            _configRepoMock
                .Setup(r =>
                    r.ListAsync<SpeciesStageConfigDto>(
                        It.IsAny<SpeciesStageConfigListSpec>(),
                        QueryType.ActiveOnly
                    )
                )
                .ReturnsAsync(configs);

            // Act
            var result = await _sut.GetAllSpeciesStageConfigsAsync();

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Data.Should().BeEmpty();
            result.Type.Should().Be(ResultType.Ok);
            result
                .Message.Should()
                .Be("Lấy danh sách cấu hình giai đoạn sinh trưởng của cá thành công.");
        }

        [Fact]
        public async Task GetAllSpeciesStageConfigsAsync_ShouldReturnUnexpected_WhenExceptionThrown()
        {
            // Arrange
            _configRepoMock
                .Setup(r =>
                    r.ListAsync(It.IsAny<SpeciesStageConfigListSpec>(), QueryType.ActiveOnly)
                )
                .ThrowsAsync(new Exception());

            // Act
            var result = await _sut.GetAllSpeciesStageConfigsAsync();

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Type.Should().Be(ResultType.Unexpected);
            result
                .Message.Should()
                .Be("Lỗi khi truy xuất danh sách cấu hình giai đoạn sinh trưởng của cá.");
        }

        #endregion

        #region UpdateSpeciesStageConfig Tests

        [Fact]
        public async Task UpdateSpeciesStageConfig_ShouldReturnOk_WhenSuccessful()
        {
            // Arrange
            var config = new SpeciesStageConfig
            {
                Id = Guid.NewGuid(),
                SpeciesId = Guid.NewGuid(),
                GrowthStageId = Guid.NewGuid(),
                FeedTypeId = Guid.NewGuid(),
                AmountPer100Fish = 100,
                FrequencyPerDay = 3,
                MaxStockingDensity = 50,
                ExpectedDurationDays = 30,
            };

            var dto = new UpdateSpeciesStageConfigDto
            {
                FeedTypeId = Guid.NewGuid(),
                AmountPer100Fish = 120,
                FrequencyPerDay = 4,
                MaxStockingDensity = 60,
                ExpectedDurationDays = 35,
            };

            _configRepoMock
                .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), QueryType.ActiveOnly))
                .ReturnsAsync(config);
            _feedTypeRepoMock
                .Setup(r =>
                    r.AnyAsync(It.IsAny<Expression<Func<FeedType, bool>>>(), QueryType.ActiveOnly)
                )
                .ReturnsAsync(true);
            _unitOfWorkMock.Setup(x => x.SaveChangesAsync(default)).ReturnsAsync(1);

            // Act
            var result = await _sut.UpdateSpeciesStageConfig(config.Id, dto);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Type.Should().Be(ResultType.Ok);
            result
                .Message.Should()
                .Be("Cập nhật cấu hình giai đoạn sinh trưởng của cá thành công.");

            _unitOfWorkMock.Verify(
                u => u.SaveChangesAsync(It.IsAny<CancellationToken>()),
                Times.Once
            );
        }

        [Fact]
        public async Task UpdateSpeciesStageConfig_ShouldReturnBadRequest_WhenFeedTypeNotExists()
        {
            // Arrange
            var id = Guid.NewGuid();
            var dto = new UpdateSpeciesStageConfigDto { FeedTypeId = Guid.NewGuid() };

            _configRepoMock
                .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), QueryType.ActiveOnly))
                .ReturnsAsync(new SpeciesStageConfig());

            _feedTypeRepoMock
                .Setup(r =>
                    r.AnyAsync(It.IsAny<Expression<Func<FeedType, bool>>>(), QueryType.ActiveOnly)
                )
                .ReturnsAsync(false);
            _unitOfWorkMock.Setup(x => x.SaveChangesAsync(default)).ReturnsAsync(1);
            // Act
            var result = await _sut.UpdateSpeciesStageConfig(id, dto);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Type.Should().Be(ResultType.BadRequest);
            result.Message.Should().Be("Kiểu cho ăn không tồn tại.");

            _unitOfWorkMock.Verify(
                u => u.SaveChangesAsync(It.IsAny<CancellationToken>()),
                Times.Never
            );
        }

        [Fact]
        public async Task UpdateSpeciesStageConfig_ShouldReturnNotFound_WhenConfigNotExists()
        {
            // Arrange
            var id = Guid.NewGuid();
            var dto = new UpdateSpeciesStageConfigDto();

            _configRepoMock
                .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), QueryType.ActiveOnly))
                .ReturnsAsync((SpeciesStageConfig?)null);
            _unitOfWorkMock.Setup(x => x.SaveChangesAsync(default)).ReturnsAsync(1);
            // Act
            var result = await _sut.UpdateSpeciesStageConfig(id, dto);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Type.Should().Be(ResultType.NotFound);
            result.Message.Should().Be("Cấu hình giai đoạn sinh trưởng của cá không tồn tại.");

            _unitOfWorkMock.Verify(
                u => u.SaveChangesAsync(It.IsAny<CancellationToken>()),
                Times.Never
            );
        }

        [Fact]
        public async Task UpdateSpeciesStageConfig_ShouldReturnUnexpected_WhenExceptionThrown()
        {
            // Arrange
            _configRepoMock
                .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), QueryType.ActiveOnly))
                .ThrowsAsync(new Exception());
            _unitOfWorkMock.Setup(x => x.SaveChangesAsync(default)).ReturnsAsync(1);
            // Act
            var result = await _sut.UpdateSpeciesStageConfig(
                Guid.NewGuid(),
                new UpdateSpeciesStageConfigDto()
            );

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Type.Should().Be(ResultType.Unexpected);
            result.Message.Should().Be("Lỗi khi cập nhật cấu hình giai đoạn sinh trưởng của cá.");
        }

        #endregion

        #region DeleteSpeciesStageConfig Tests

        [Fact]
        public async Task DeleteSpeciesStageConfig_ShouldReturnOk_WhenSuccessful()
        {
            // Arrange
            var id = Guid.NewGuid();
            var config = new SpeciesStageConfig { Id = id };

            _configRepoMock
                .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), QueryType.ActiveOnly))
                .ReturnsAsync(config);
            _unitOfWorkMock.Setup(x => x.SaveChangesAsync(default)).ReturnsAsync(1);
            // Act
            var result = await _sut.DeleteSpeciesStageConfig(id);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Type.Should().Be(ResultType.Ok);
            result.Message.Should().Be("Xóa cấu hình giai đoạn sinh trưởng của cá thành công.");

            _configRepoMock.Verify(r => r.Delete(config), Times.Once);
            _unitOfWorkMock.Verify(
                u => u.SaveChangesAsync(It.IsAny<CancellationToken>()),
                Times.Once
            );
        }

        [Fact]
        public async Task DeleteSpeciesStageConfig_ShouldReturnNotFound_WhenConfigNotExists()
        {
            // Arrange
            _configRepoMock
                .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), QueryType.ActiveOnly))
                .ReturnsAsync((SpeciesStageConfig?)null);
            _unitOfWorkMock.Setup(x => x.SaveChangesAsync(default)).ReturnsAsync(1);
            // Act
            var result = await _sut.DeleteSpeciesStageConfig(Guid.NewGuid());

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Type.Should().Be(ResultType.NotFound);
            result.Message.Should().Be("Cấu hình giai đoạn sinh trưởng của cá không tồn tại.");

            _configRepoMock.Verify(r => r.Delete(It.IsAny<SpeciesStageConfig>()), Times.Never);
        }

        [Fact]
        public async Task DeleteSpeciesStageConfig_ShouldReturnUnexpected_WhenExceptionThrown()
        {
            // Arrange
            _configRepoMock
                .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), QueryType.ActiveOnly))
                .ThrowsAsync(new Exception());
            _unitOfWorkMock.Setup(x => x.SaveChangesAsync(default)).ReturnsAsync(1);
            // Act
            var result = await _sut.DeleteSpeciesStageConfig(Guid.NewGuid());

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Type.Should().Be(ResultType.Unexpected);
            result.Message.Should().Be("Lỗi khi xóa cấu hình giai đoạn sinh trưởng của cá.");
        }

        #endregion
    }
}

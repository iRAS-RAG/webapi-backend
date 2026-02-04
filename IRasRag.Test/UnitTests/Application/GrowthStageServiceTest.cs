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
    public class GrowthStageServiceTest
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<ILogger<GrowthStageService>> _loggerMock;
        private readonly IMapper _mapper;
        private readonly Mock<IRepository<GrowthStage>> _repositoryMock;
        private readonly GrowthStageService _sut;

        public GrowthStageServiceTest()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _loggerMock = new Mock<ILogger<GrowthStageService>>();
            _mapper = AutoMapperTestHelper.GetMapper(new GrowthStageProfile());
            _repositoryMock = new Mock<IRepository<GrowthStage>>();
            _unitOfWorkMock
                .Setup(x => x.GetRepository<GrowthStage>())
                .Returns(_repositoryMock.Object);

            _sut = new GrowthStageService(_unitOfWorkMock.Object, _loggerMock.Object, _mapper);
        }

        #region CreateGrowthStageAsync Tests

        [Fact]
        public async Task CreateGrowthStageAsync_ShouldReturnOk_WhenSuccessful()
        {
            // Arrange
            var createDto = new CreateGrowthStageDto
            {
                Name = "Juvenile",
                Description = "Juvenile stage description",
            };

            _unitOfWorkMock.Setup(x => x.SaveChangesAsync(default)).ReturnsAsync(1);

            // Act
            var result = await _sut.CreateGrowthStageAsync(createDto);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Type.Should().Be(ResultType.Ok);
            result.Message.Should().Be("Tạo giai đoạn sinh trưởng thành công.");
            result.Data.Should().NotBeNull();
            result.Data!.Name.Should().Be(createDto.Name);
            result.Data.Description.Should().Be(createDto.Description);

            _repositoryMock.Verify(
                r =>
                    r.AddAsync(
                        It.Is<GrowthStage>(gs =>
                            gs.Name == createDto.Name && gs.Description == createDto.Description
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
        public async Task CreateGrowthStageAsync_ShouldTrimInputs_WhenCreating()
        {
            // Arrange
            var createDto = new CreateGrowthStageDto
            {
                Name = "  Juvenile  ",
                Description = "  Description  ",
            };

            _unitOfWorkMock.Setup(x => x.SaveChangesAsync(default)).ReturnsAsync(1);
            // Act
            var result = await _sut.CreateGrowthStageAsync(createDto);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Data!.Name.Should().Be("Juvenile");
            result.Data.Description.Should().Be("Description");

            _repositoryMock.Verify(
                r =>
                    r.AddAsync(
                        It.Is<GrowthStage>(gs =>
                            gs.Name == "Juvenile" && gs.Description == "Description"
                        )
                    ),
                Times.Once
            );
        }

        [Theory]
        [InlineData("", "Some description", "Tên giai đoạn sinh trưởng không được để trống.")]
        [InlineData(" ", "Some description", "Tên giai đoạn sinh trưởng không được để trống.")]
        [InlineData("  ", "Some description", "Tên giai đoạn sinh trưởng không được để trống.")]
        [InlineData("\t", "Some description", "Tên giai đoạn sinh trưởng không được để trống.")]
        [InlineData("\n", "Some description", "Tên giai đoạn sinh trưởng không được để trống.")]
        [InlineData("Juvenile", "", "Mô tả giai đoạn sinh trưởng không được để trống.")]
        [InlineData("Juvenile", " ", "Mô tả giai đoạn sinh trưởng không được để trống.")]
        [InlineData("Juvenile", "\t", "Mô tả giai đoạn sinh trưởng không được để trống.")]
        public async Task CreateGrowthStageAsync_ShouldReturnBadRequest_WhenInputIsInvalid(
            string name,
            string description,
            string expectedMessage
        )
        {
            // Arrange
            var createDto = new CreateGrowthStageDto { Name = name, Description = description };
            _unitOfWorkMock.Setup(x => x.SaveChangesAsync(default)).ReturnsAsync(1);
            // Act
            var result = await _sut.CreateGrowthStageAsync(createDto);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Type.Should().Be(ResultType.BadRequest);
            result.Message.Should().Be(expectedMessage);

            _repositoryMock.Verify(r => r.AddAsync(It.IsAny<GrowthStage>()), Times.Never);

            _unitOfWorkMock.Verify(
                u => u.SaveChangesAsync(It.IsAny<CancellationToken>()),
                Times.Never
            );
        }

        [Fact]
        public async Task CreateGrowthStageAsync_ShouldReturnUnexpected_WhenExceptionThrown()
        {
            // Arrange
            var createDto = new CreateGrowthStageDto
            {
                Name = "Juvenile",
                Description = "Juvenile stage description",
            };
            _unitOfWorkMock.Setup(x => x.SaveChangesAsync(default)).ReturnsAsync(1);

            _unitOfWorkMock
                .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception());

            // Act
            var result = await _sut.CreateGrowthStageAsync(createDto);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Type.Should().Be(ResultType.Unexpected);
            result.Message.Should().Be("Lỗi khi tạo giai đoạn sinh trưởng.");

            _repositoryMock.Verify(r => r.AddAsync(It.IsAny<GrowthStage>()), Times.Once);

            _unitOfWorkMock.Verify(
                u => u.SaveChangesAsync(It.IsAny<CancellationToken>()),
                Times.Once
            );
        }

        #endregion

        #region GetGrowthStageByIdAsync Tests

        [Fact]
        public async Task GetGrowthStageByIdAsync_ShouldReturnOk_WhenGrowthStageExists()
        {
            // Arrange
            var growthStageId = Guid.NewGuid();
            var growthStage = new GrowthStage
            {
                Id = growthStageId,
                Name = "Juvenile",
                Description = "Juvenile stage description",
            };

            _repositoryMock
                .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), QueryType.ActiveOnly))
                .ReturnsAsync(growthStage);

            // Act
            var result = await _sut.GetGrowthStageByIdAsync(growthStageId);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Type.Should().Be(ResultType.Ok);
            result.Message.Should().Be("Lấy giai đoạn sinh trưởng thành công.");
            result.Data.Should().NotBeNull();
            result.Data!.Id.Should().Be(growthStageId);
            result.Data.Name.Should().Be(growthStage.Name);
            result.Data.Description.Should().Be(growthStage.Description);

            _repositoryMock.Verify(
                r => r.GetByIdAsync(It.Is<Guid>(id => id == growthStageId), QueryType.ActiveOnly),
                Times.Once
            );
        }

        [Fact]
        public async Task GetGrowthStageByIdAsync_ShouldReturnNotFound_WhenGrowthStageNotExists()
        {
            // Arrange
            var growthStageId = Guid.NewGuid();
            _repositoryMock
                .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), QueryType.ActiveOnly))
                .ReturnsAsync((GrowthStage?)null);

            // Act
            var result = await _sut.GetGrowthStageByIdAsync(growthStageId);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Type.Should().Be(ResultType.NotFound);
            result.Message.Should().Be("Giai đoạn sinh trưởng không tồn tại.");
            result.Data.Should().BeNull();

            _repositoryMock.Verify(
                r => r.GetByIdAsync(It.Is<Guid>(id => id == growthStageId), QueryType.ActiveOnly),
                Times.Once
            );
        }

        [Fact]
        public async Task GetGrowthStageByIdAsync_ShouldReturnUnexpected_WhenThrownException()
        {
            // Arrage
            var growthStageId = Guid.NewGuid();
            _repositoryMock
                .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), QueryType.ActiveOnly))
                .ThrowsAsync(new Exception());

            // Act
            var result = await _sut.GetGrowthStageByIdAsync(growthStageId);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Type.Should().Be(ResultType.Unexpected);
            result.Message.Should().Be("Lỗi khi truy xuất giai đoạn sinh trưởng.");

            _repositoryMock.Verify(
                r => r.GetByIdAsync(It.Is<Guid>(id => id == growthStageId), QueryType.ActiveOnly),
                Times.Once
            );
        }

        #endregion

        #region GetAllGrowthStagesAsync Tests

        [Fact]
        public async Task GetAllGrowthStagesAsync_ShouldReturnOk_WhenSuccessful()
        {
            // Arrange
            var growthStages = new List<GrowthStage>
            {
                new GrowthStage
                {
                    Id = Guid.NewGuid(),
                    Name = "Juvenile",
                    Description = "Juvenile stage description",
                },
                new GrowthStage
                {
                    Id = Guid.NewGuid(),
                    Name = "Adult",
                    Description = "Adult stage description",
                },
            };

            _repositoryMock
                .Setup(r => r.GetAllAsync(QueryType.ActiveOnly))
                .ReturnsAsync(growthStages);

            // Act
            var result = await _sut.GetAllGrowthStagesAsync();

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Type.Should().Be(ResultType.Ok);
            result.Message.Should().Be("Lấy danh sách giai đoạn sinh trưởng thành công.");
            result.Data.Should().NotBeNull();
            result.Data!.Count().Should().Be(growthStages.Count);
            _repositoryMock.Verify(r => r.GetAllAsync(QueryType.ActiveOnly), Times.Once);
        }

        [Fact]
        public async Task GetAllGrowthStagesAsync_ShouldReturnEmptyList_WhenNoRecordsExist()
        {
            // Arrange
            _repositoryMock
                .Setup(r => r.GetAllAsync(QueryType.ActiveOnly))
                .ReturnsAsync(new List<GrowthStage>());

            // Act
            var result = await _sut.GetAllGrowthStagesAsync();

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Type.Should().Be(ResultType.Ok);
            result.Message.Should().Be("Lấy danh sách giai đoạn sinh trưởng thành công.");
            result.Data.Should().BeEmpty();
            _repositoryMock.Verify(r => r.GetAllAsync(QueryType.ActiveOnly), Times.Once);
        }

        [Fact]
        public async Task GetAllGrowthStagesAsync_ShouldReturnUnexpected_WhenThrownException()
        {
            // Arrange
            _repositoryMock
                .Setup(r => r.GetAllAsync(QueryType.ActiveOnly))
                .ThrowsAsync(new Exception());
            // Act
            var result = await _sut.GetAllGrowthStagesAsync();
            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Type.Should().Be(ResultType.Unexpected);
            result.Message.Should().Be("Lỗi khi truy xuất danh sách giai đoạn sinh trưởng.");
            _repositoryMock.Verify(r => r.GetAllAsync(QueryType.ActiveOnly), Times.Once);
        }

        #endregion

        #region DeleteGrowthStageAsync Tests
        [Fact]
        public async Task DeleteGrowthStageAsync_ShouldReturnOk_WhenSuccessful()
        {
            // Arrange
            var growthStageId = Guid.NewGuid();
            var growthStage = new GrowthStage
            {
                Id = growthStageId,
                Name = "Juvenile",
                Description = "Juvenile stage description",
            };
            _repositoryMock
                .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), QueryType.ActiveOnly))
                .ReturnsAsync(growthStage);
            _unitOfWorkMock.Setup(x => x.SaveChangesAsync(default)).ReturnsAsync(1);

            // Act
            var result = await _sut.DeleteGrowthStageAsync(growthStageId);
            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Type.Should().Be(ResultType.Ok);
            result.Message.Should().Be("Xóa giai đoạn sinh trưởng thành công.");
            _repositoryMock.Verify(
                r => r.GetByIdAsync(It.Is<Guid>(id => id == growthStageId), QueryType.ActiveOnly),
                Times.Once
            );
            _repositoryMock.Verify(
                r => r.Delete(It.Is<GrowthStage>(gs => gs.Id == growthStageId)),
                Times.Once
            );
            _unitOfWorkMock.Verify(
                u => u.SaveChangesAsync(It.IsAny<CancellationToken>()),
                Times.Once
            );
        }

        [Fact]
        public async Task DeleteGrowthStageAsync_ShouldReturnNotFound_WhenGrowthStageNotExists()
        {
            // Arrange
            var growthStageId = Guid.NewGuid();
            _repositoryMock
                .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), QueryType.ActiveOnly))
                .ReturnsAsync((GrowthStage?)null);
            _unitOfWorkMock.Setup(x => x.SaveChangesAsync(default)).ReturnsAsync(1);
            // Act
            var result = await _sut.DeleteGrowthStageAsync(growthStageId);
            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Type.Should().Be(ResultType.NotFound);
            result.Message.Should().Be("Giai đoạn sinh trưởng không tồn tại.");
            _repositoryMock.Verify(
                r => r.GetByIdAsync(It.Is<Guid>(id => id == growthStageId), QueryType.ActiveOnly),
                Times.Once
            );
            _repositoryMock.Verify(r => r.Delete(It.IsAny<GrowthStage>()), Times.Never);
            _unitOfWorkMock.Verify(
                u => u.SaveChangesAsync(It.IsAny<CancellationToken>()),
                Times.Never
            );
        }

        [Fact]
        public async Task DeleteGrowthStageAsync_ShouldReturnUnexpected_WhenThrownException()
        {
            // Arrange
            var growthStageId = Guid.NewGuid();
            _repositoryMock
                .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), QueryType.ActiveOnly))
                .ThrowsAsync(new Exception());
            _unitOfWorkMock.Setup(x => x.SaveChangesAsync(default)).ReturnsAsync(1);
            // Act
            var result = await _sut.DeleteGrowthStageAsync(growthStageId);
            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Type.Should().Be(ResultType.Unexpected);
            result.Message.Should().Be("Lỗi khi xóa giai đoạn sinh trưởng.");
            _repositoryMock.Verify(
                r => r.GetByIdAsync(It.Is<Guid>(id => id == growthStageId), QueryType.ActiveOnly),
                Times.Once
            );
            _repositoryMock.Verify(r => r.Delete(It.IsAny<GrowthStage>()), Times.Never);
            _unitOfWorkMock.Verify(
                u => u.SaveChangesAsync(It.IsAny<CancellationToken>()),
                Times.Never
            );
        }

        #endregion

        #region UpdateGrowthStageAsync Tests

        [Fact]
        public async Task UpdateGrowthStageAsync_ShouldReturnOk_WhenBothFieldsUpdated()
        {
            // Arrange
            var growthStageId = Guid.NewGuid();
            var existingGrowthStage = new GrowthStage
            {
                Id = growthStageId,
                Name = "Juvenile",
                Description = "Juvenile stage description",
            };
            var updateDto = new UpdateGrowthStageDto
            {
                Name = "Updated Juvenile",
                Description = "Updated description",
            };
            _repositoryMock
                .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), QueryType.ActiveOnly))
                .ReturnsAsync(existingGrowthStage);
            _unitOfWorkMock.Setup(x => x.SaveChangesAsync(default)).ReturnsAsync(1);

            // Act
            var result = await _sut.UpdateGrowthStageAsync(growthStageId, updateDto);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Type.Should().Be(ResultType.Ok);
            result.Message.Should().Be("Cập nhật giai đoạn sinh trưởng thành công.");

            existingGrowthStage.Name.Should().Be("Updated Juvenile");
            existingGrowthStage.Description.Should().Be("Updated description");

            _repositoryMock.Verify(
                r => r.GetByIdAsync(It.Is<Guid>(id => id == growthStageId), QueryType.ActiveOnly),
                Times.Once
            );
            _unitOfWorkMock.Verify(
                u => u.SaveChangesAsync(It.IsAny<CancellationToken>()),
                Times.Once
            );
        }

        [Theory]
        [InlineData("Updated Juvenile", null, "Updated Juvenile", "Juvenile stage description")]
        [InlineData(null, "Updated description", "Juvenile", "Updated description")]
        [InlineData("", "Updated description", "Juvenile", "Updated description")]
        [InlineData(" ", "Updated description", "Juvenile", "Updated description")]
        [InlineData("Updated Juvenile", "", "Updated Juvenile", "Juvenile stage description")]
        [InlineData("Updated Juvenile", " ", "Updated Juvenile", "Juvenile stage description")]
        [InlineData(null, null, "Juvenile", "Juvenile stage description")]
        [InlineData("", "", "Juvenile", "Juvenile stage description")]
        [InlineData(" ", " ", "Juvenile", "Juvenile stage description")]
        public async Task UpdateGrowthStageAsync_ShouldUpdateOnlyProvidedFields(
            string nameInput,
            string descriptionInput,
            string expectedName,
            string expectedDescription
        )
        {
            // Arrange
            var growthStageId = Guid.NewGuid();
            var existingGrowthStage = new GrowthStage
            {
                Id = growthStageId,
                Name = "Juvenile",
                Description = "Juvenile stage description",
            };
            var updateDto = new UpdateGrowthStageDto
            {
                Name = nameInput,
                Description = descriptionInput,
            };

            _repositoryMock
                .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), QueryType.ActiveOnly))
                .ReturnsAsync(existingGrowthStage);
            _unitOfWorkMock.Setup(x => x.SaveChangesAsync(default)).ReturnsAsync(1);

            // Act
            var result = await _sut.UpdateGrowthStageAsync(growthStageId, updateDto);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Type.Should().Be(ResultType.Ok);
            result.Message.Should().Be("Cập nhật giai đoạn sinh trưởng thành công.");
            existingGrowthStage.Name.Should().Be(expectedName);
            existingGrowthStage.Description.Should().Be(expectedDescription);

            _repositoryMock.Verify(
                r => r.GetByIdAsync(It.Is<Guid>(id => id == growthStageId), QueryType.ActiveOnly),
                Times.Once
            );
            _unitOfWorkMock.Verify(
                u => u.SaveChangesAsync(It.IsAny<CancellationToken>()),
                Times.Once
            );
        }

        [Fact]
        public async Task UpdateGrowthStageAsync_ShouldTrimInputs_WhenUpdating()
        {
            // Arrange
            var growthStageId = Guid.NewGuid();
            var existingGrowthStage = new GrowthStage
            {
                Id = growthStageId,
                Name = "Juvenile",
                Description = "Juvenile stage description",
            };
            var updateDto = new UpdateGrowthStageDto
            {
                Name = "  Updated Juvenile  ",
                Description = "  Updated description  ",
            };

            _repositoryMock
                .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), QueryType.ActiveOnly))
                .ReturnsAsync(existingGrowthStage);
            _unitOfWorkMock.Setup(x => x.SaveChangesAsync(default)).ReturnsAsync(1);

            // Act
            var result = await _sut.UpdateGrowthStageAsync(growthStageId, updateDto);

            // Assert
            result.IsSuccess.Should().BeTrue();
            existingGrowthStage.Name.Should().Be("Updated Juvenile");
            existingGrowthStage.Description.Should().Be("Updated description");
        }

        [Fact]
        public async Task UpdateGrowthStageAsync_ShouldReturnNotFound_WhenGrowthStageNotExists()
        {
            // Arrange
            var growthStageId = Guid.NewGuid();
            var updateDto = new UpdateGrowthStageDto
            {
                Name = "Updated Juvenile",
                Description = "Updated description",
            };

            _repositoryMock
                .Setup(r =>
                    r.GetByIdAsync(It.Is<Guid>(id => id == growthStageId), QueryType.ActiveOnly)
                )
                .ReturnsAsync((GrowthStage?)null);
            _unitOfWorkMock.Setup(x => x.SaveChangesAsync(default)).ReturnsAsync(1);

            // Act
            var result = await _sut.UpdateGrowthStageAsync(growthStageId, updateDto);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Type.Should().Be(ResultType.NotFound);
            result.Message.Should().Be("Giai đoạn sinh trưởng không tồn tại.");

            _repositoryMock.Verify(
                r => r.GetByIdAsync(It.Is<Guid>(id => id == growthStageId), QueryType.ActiveOnly),
                Times.Once
            );
            _unitOfWorkMock.Verify(
                u => u.SaveChangesAsync(It.IsAny<CancellationToken>()),
                Times.Never
            );
        }

        [Fact]
        public async Task UpdateGrowthStageAsync_ShouldReturnUnexpected_WhenExceptionThrown()
        {
            // Arrange
            var growthStageId = Guid.NewGuid();
            var updateDto = new UpdateGrowthStageDto
            {
                Name = "Updated Juvenile",
                Description = "Updated description",
            };

            _repositoryMock
                .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), QueryType.ActiveOnly))
                .ThrowsAsync(new Exception());
            _unitOfWorkMock.Setup(x => x.SaveChangesAsync(default)).ReturnsAsync(1);

            // Act
            var result = await _sut.UpdateGrowthStageAsync(growthStageId, updateDto);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Type.Should().Be(ResultType.Unexpected);
            result.Message.Should().Be("Lỗi khi cập nhật giai đoạn sinh trưởng.");

            _repositoryMock.Verify(
                r => r.GetByIdAsync(It.Is<Guid>(id => id == growthStageId), QueryType.ActiveOnly),
                Times.Once
            );
            _unitOfWorkMock.Verify(
                u => u.SaveChangesAsync(It.IsAny<CancellationToken>()),
                Times.Never
            );
        }

        #endregion
    }
}

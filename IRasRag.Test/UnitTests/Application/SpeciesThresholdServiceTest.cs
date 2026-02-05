using System.Linq.Expressions;
using AutoMapper;
using FluentAssertions;
using IRasRag.Application.Common.Interfaces.Persistence;
using IRasRag.Application.Common.Mappings;
using IRasRag.Application.Common.Models;
using IRasRag.Application.Common.Models.Pagination;
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
    public class SpeciesThresholdServiceTest
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IRepository<SpeciesThreshold>> _thresholdRepoMock;
        private readonly Mock<IRepository<Species>> _speciesRepoMock;
        private readonly Mock<IRepository<GrowthStage>> _growthStageRepoMock;
        private readonly Mock<IRepository<SensorType>> _sensorTypeRepoMock;
        private readonly Mock<ILogger<SpeciesThresholdService>> _loggerMock;
        private readonly IMapper _mapper;
        private readonly SpeciesThresholdService _sut;

        public SpeciesThresholdServiceTest()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _thresholdRepoMock = new Mock<IRepository<SpeciesThreshold>>();
            _speciesRepoMock = new Mock<IRepository<Species>>();
            _growthStageRepoMock = new Mock<IRepository<GrowthStage>>();
            _sensorTypeRepoMock = new Mock<IRepository<SensorType>>();
            _loggerMock = new Mock<ILogger<SpeciesThresholdService>>();
            _mapper = AutoMapperTestHelper.GetMapper(new SpeciesThresholdProfile());

            _unitOfWorkMock
                .Setup(u => u.GetRepository<SpeciesThreshold>())
                .Returns(_thresholdRepoMock.Object);

            _unitOfWorkMock.Setup(u => u.GetRepository<Species>()).Returns(_speciesRepoMock.Object);

            _unitOfWorkMock
                .Setup(u => u.GetRepository<GrowthStage>())
                .Returns(_growthStageRepoMock.Object);
            _unitOfWorkMock
                .Setup(u => u.GetRepository<SensorType>())
                .Returns(_sensorTypeRepoMock.Object);

            _sut = new SpeciesThresholdService(_unitOfWorkMock.Object, _loggerMock.Object, _mapper);
        }

        #region CreateSpeciesThreshold

        [Fact]
        public async Task CreateSpeciesThreshold_ShouldReturnBadRequest_WhenMinGreaterOrEqualMax()
        {
            var dto = new CreateSpeciesThresholdDto { MinValue = 10, MaxValue = 5 };
            _unitOfWorkMock.Setup(x => x.SaveChangesAsync(default)).ReturnsAsync(1);

            var result = await _sut.CreateSpeciesThreshold(dto);

            result.IsSuccess.Should().BeFalse();
            result.Type.Should().Be(ResultType.BadRequest);
            result.Message.Should().Be("Giá trị Min phải nhỏ hơn Max.");
        }

        [Fact]
        public async Task CreateSpeciesThreshold_ShouldReturnConflict_WhenExists()
        {
            var dto = new CreateSpeciesThresholdDto
            {
                MinValue = 1,
                MaxValue = 10,
                SpeciesId = Guid.NewGuid(),
                GrowthStageId = Guid.NewGuid(),
                SensorTypeId = Guid.NewGuid(),
            };

            _unitOfWorkMock
                .Setup(u =>
                    u.GetRepository<Species>()
                        .AnyAsync(It.IsAny<Expression<Func<Species, bool>>>(), QueryType.ActiveOnly)
                )
                .ReturnsAsync(true);

            _unitOfWorkMock
                .Setup(u =>
                    u.GetRepository<GrowthStage>()
                        .AnyAsync(
                            It.IsAny<Expression<Func<GrowthStage, bool>>>(),
                            QueryType.ActiveOnly
                        )
                )
                .ReturnsAsync(true);

            _unitOfWorkMock
                .Setup(u =>
                    u.GetRepository<SensorType>()
                        .AnyAsync(
                            It.IsAny<Expression<Func<SensorType, bool>>>(),
                            QueryType.ActiveOnly
                        )
                )
                .ReturnsAsync(true);

            _thresholdRepoMock
                .Setup(r =>
                    r.AnyAsync(
                        It.IsAny<Expression<Func<SpeciesThreshold, bool>>>(),
                        QueryType.ActiveOnly
                    )
                )
                .ReturnsAsync(true);
            _unitOfWorkMock.Setup(x => x.SaveChangesAsync(default)).ReturnsAsync(1);

            var result = await _sut.CreateSpeciesThreshold(dto);

            result.IsSuccess.Should().BeFalse();
            result.Type.Should().Be(ResultType.Conflict);
            result.Message.Should().Be("Ngưỡng sinh trưởng này đã tồn tại.");
        }

        [Fact]
        public async Task CreateSpeciesThreshold_ShouldReturnBadRequest_WhenSpeciesNotExists()
        {
            var dto = new CreateSpeciesThresholdDto
            {
                MinValue = 1,
                MaxValue = 10,
                SpeciesId = Guid.NewGuid(),
                GrowthStageId = Guid.NewGuid(),
                SensorTypeId = Guid.NewGuid(),
            };

            _speciesRepoMock
                .Setup(r =>
                    r.AnyAsync(It.IsAny<Expression<Func<Species, bool>>>(), QueryType.ActiveOnly)
                )
                .ReturnsAsync(false);
            _unitOfWorkMock.Setup(x => x.SaveChangesAsync(default)).ReturnsAsync(1);

            var result = await _sut.CreateSpeciesThreshold(dto);

            result.IsSuccess.Should().BeFalse();
            result.Type.Should().Be(ResultType.BadRequest);
            result.Message.Should().Be("Loài cá không tồn tại.");
        }

        [Fact]
        public async Task CreateSpeciesThreshold_ShouldReturnBadRequest_WhenGrowthStageNotExists()
        {
            var dto = new CreateSpeciesThresholdDto
            {
                MinValue = 1,
                MaxValue = 10,
                SpeciesId = Guid.NewGuid(),
                GrowthStageId = Guid.NewGuid(),
                SensorTypeId = Guid.NewGuid(),
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

            var result = await _sut.CreateSpeciesThreshold(dto);

            result.IsSuccess.Should().BeFalse();
            result.Type.Should().Be(ResultType.BadRequest);
            result.Message.Should().Be("Giai đoạn sinh trưởng không tồn tại.");
        }

        [Fact]
        public async Task CreateSpeciesThreshold_ShouldReturnBadRequest_WhenSensorTypeNotExists()
        {
            var dto = new CreateSpeciesThresholdDto
            {
                MinValue = 1,
                MaxValue = 10,
                SpeciesId = Guid.NewGuid(),
                GrowthStageId = Guid.NewGuid(),
                SensorTypeId = Guid.NewGuid(),
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

            _sensorTypeRepoMock
                .Setup(r =>
                    r.AnyAsync(It.IsAny<Expression<Func<SensorType, bool>>>(), QueryType.ActiveOnly)
                )
                .ReturnsAsync(false);
            _unitOfWorkMock.Setup(x => x.SaveChangesAsync(default)).ReturnsAsync(1);

            var result = await _sut.CreateSpeciesThreshold(dto);

            result.IsSuccess.Should().BeFalse();
            result.Type.Should().Be(ResultType.BadRequest);
            result.Message.Should().Be("Loại cảm biến không tồn tại.");
        }

        [Fact]
        public async Task CreateSpeciesThreshold_ShouldReturnOk_WhenSuccessful()
        {
            var dto = new CreateSpeciesThresholdDto
            {
                MinValue = 1,
                MaxValue = 10,
                SpeciesId = Guid.NewGuid(),
                GrowthStageId = Guid.NewGuid(),
                SensorTypeId = Guid.NewGuid(),
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

            _sensorTypeRepoMock
                .Setup(r =>
                    r.AnyAsync(It.IsAny<Expression<Func<SensorType, bool>>>(), QueryType.ActiveOnly)
                )
                .ReturnsAsync(true);

            _thresholdRepoMock
                .Setup(r =>
                    r.AnyAsync(
                        It.IsAny<Expression<Func<SpeciesThreshold, bool>>>(),
                        QueryType.ActiveOnly
                    )
                )
                .ReturnsAsync(false);
            _unitOfWorkMock.Setup(x => x.SaveChangesAsync(default)).ReturnsAsync(1);
            var result = await _sut.CreateSpeciesThreshold(dto);

            result.IsSuccess.Should().BeTrue();
            result.Type.Should().Be(ResultType.Ok);
            result.Message.Should().Be("Tạo ngưỡng sinh trưởng thành công.");
            result.Data.Should().NotBeNull();

            _thresholdRepoMock.Verify(r => r.AddAsync(It.IsAny<SpeciesThreshold>()), Times.Once);

            _unitOfWorkMock.Verify(
                u => u.SaveChangesAsync(It.IsAny<CancellationToken>()),
                Times.Once
            );
        }

        [Fact]
        public async Task CreateSpeciesThreshold_ShouldReturnUnexpected_WhenExceptionThrown()
        {
            var dto = new CreateSpeciesThresholdDto
            {
                MinValue = 1,
                MaxValue = 10,
                SpeciesId = Guid.NewGuid(),
                GrowthStageId = Guid.NewGuid(),
                SensorTypeId = Guid.NewGuid(),
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

            _sensorTypeRepoMock
                .Setup(r =>
                    r.AnyAsync(It.IsAny<Expression<Func<SensorType, bool>>>(), QueryType.ActiveOnly)
                )
                .ReturnsAsync(true);

            _thresholdRepoMock
                .Setup(r =>
                    r.AnyAsync(
                        It.IsAny<Expression<Func<SpeciesThreshold, bool>>>(),
                        QueryType.ActiveOnly
                    )
                )
                .ReturnsAsync(false);
            _unitOfWorkMock.Setup(x => x.SaveChangesAsync(default)).ReturnsAsync(1);
            _thresholdRepoMock
                .Setup(r => r.AddAsync(It.IsAny<SpeciesThreshold>()))
                .ThrowsAsync(new Exception());

            var result = await _sut.CreateSpeciesThreshold(dto);

            result.IsSuccess.Should().BeFalse();
            result.Type.Should().Be(ResultType.Unexpected);
            result.Message.Should().Be("Lỗi khi tạo ngưỡng sinh trưởng.");
        }

        #endregion

        #region GetSpeciesThresholdById

        [Fact]
        public async Task GetSpeciesThresholdById_ShouldReturnOk_WhenExists()
        {
            var dto = new SpeciesThresholdDto { Id = Guid.NewGuid() };

            _thresholdRepoMock
                .Setup(r =>
                    r.FirstOrDefaultAsync(
                        It.IsAny<SpeciesThresholdByIdSpec>(),
                        QueryType.ActiveOnly
                    )
                )
                .ReturnsAsync(dto);

            var result = await _sut.GetSpeciesThresholdById(dto.Id);

            result.IsSuccess.Should().BeTrue();
            result.Type.Should().Be(ResultType.Ok);
            result.Message.Should().Be("Lấy ngưỡng sinh trưởng thành công.");
            result.Data.Should().BeEquivalentTo(dto);
        }

        [Fact]
        public async Task GetSpeciesThresholdById_ShouldReturnNotFound_WhenMissing()
        {
            _thresholdRepoMock
                .Setup(r =>
                    r.FirstOrDefaultAsync(
                        It.IsAny<SpeciesThresholdByIdSpec>(),
                        QueryType.ActiveOnly
                    )
                )
                .ReturnsAsync((SpeciesThresholdDto?)null);

            var result = await _sut.GetSpeciesThresholdById(Guid.NewGuid());

            result.IsSuccess.Should().BeFalse();
            result.Type.Should().Be(ResultType.NotFound);
            result.Message.Should().Be("Ngưỡng sinh trưởng không tồn tại.");
        }

        [Fact]
        public async Task GetSpeciesThresholdById_ShouldReturnUnexpected_WhenExceptionThrown()
        {
            _thresholdRepoMock
                .Setup(r =>
                    r.FirstOrDefaultAsync(
                        It.IsAny<SpeciesThresholdByIdSpec>(),
                        QueryType.ActiveOnly
                    )
                )
                .ThrowsAsync(new Exception());

            var result = await _sut.GetSpeciesThresholdById(Guid.NewGuid());

            result.IsSuccess.Should().BeFalse();
            result.Type.Should().Be(ResultType.Unexpected);
            result.Message.Should().Be("Lỗi khi truy xuất ngưỡng sinh trưởng.");
        }

        #endregion

        #region GetAllSpeciesThresholdsAsync

        [Fact]
        public async Task GetAllSpeciesThresholdsAsync_ShouldReturnOk_WhenSuccessful()
        {
            // Arrange
            var page = 1;
            var pageSize = 10;

            var list = new List<SpeciesThresholdDto>
            {
                new SpeciesThresholdDto(),
                new SpeciesThresholdDto(),
            };

            _thresholdRepoMock
                .Setup(r =>
                    r.GetPagedAsync<SpeciesThresholdDto>(
                        It.IsAny<SpeciesThresholdListSpec>(),
                        page,
                        pageSize,
                        QueryType.ActiveOnly
                    )
                )
                .ReturnsAsync(
                    new PagedResult<SpeciesThresholdDto> { Items = list, TotalItems = list.Count }
                );

            // Act
            var result = await _sut.GetAllSpeciesThresholdsAsync(page, pageSize);

            // Assert
            result.Should().NotBeNull();
            result.Message.Should().Be("Lấy danh sách ngưỡng sinh trưởng thành công");
            result.Data.Should().NotBeNull();
            result.Data!.Count.Should().Be(2);

            result.Meta.Should().NotBeNull();
            result.Meta!.Page.Should().Be(page);
            result.Meta.PageSize.Should().Be(pageSize);
            result.Meta.TotalItems.Should().Be(2);

            result.Links.Should().NotBeNull();
        }

        [Fact]
        public async Task GetAllSpeciesThresholdsAsync_ShouldReturnErrorMessage_WhenExceptionThrown()
        {
            // Arrange
            var page = 1;
            var pageSize = 10;

            _thresholdRepoMock
                .Setup(r =>
                    r.GetPagedAsync<SpeciesThresholdDto>(
                        It.IsAny<SpeciesThresholdListSpec>(),
                        page,
                        pageSize,
                        QueryType.ActiveOnly
                    )
                )
                .ThrowsAsync(new Exception());

            // Act
            var result = await _sut.GetAllSpeciesThresholdsAsync(page, pageSize);

            // Assert
            result.Should().NotBeNull();
            result.Message.Should().Be("Lỗi khi truy xuất danh sách ngưỡng sinh trưởng");

            result.Data.Should().NotBeNull();
            result.Data!.Should().BeEmpty();

            result.Meta.Should().BeNull();
            result.Links.Should().BeNull();
        }

        [Fact]
        public async Task GetAllSpeciesThresholdsAsync_ShouldReturnEmptyList_WhenNoThresholdsExist()
        {
            // Arrange
            var page = 1;
            var pageSize = 10;

            _thresholdRepoMock
                .Setup(r =>
                    r.GetPagedAsync<SpeciesThresholdDto>(
                        It.IsAny<SpeciesThresholdListSpec>(),
                        page,
                        pageSize,
                        QueryType.ActiveOnly
                    )
                )
                .ReturnsAsync(
                    new PagedResult<SpeciesThresholdDto>
                    {
                        Items = Array.Empty<SpeciesThresholdDto>(),
                        TotalItems = 0,
                    }
                );

            // Act
            var result = await _sut.GetAllSpeciesThresholdsAsync(page, pageSize);

            // Assert
            result.Should().NotBeNull();
            result.Message.Should().Be("Không có ngưỡng sinh trưởng nào");

            result.Data.Should().NotBeNull();
            result.Data!.Should().BeEmpty();

            result.Meta.Should().NotBeNull();
            result.Meta!.TotalItems.Should().Be(0);

            result.Links.Should().NotBeNull();
        }

        #endregion

        #region UpdateSpeciesThreshold

        [Fact]
        public async Task UpdateSpeciesThreshold_ShouldReturnOk_WhenSuccessful()
        {
            var entity = new SpeciesThreshold
            {
                Id = Guid.NewGuid(),
                SpeciesId = Guid.NewGuid(),
                GrowthStageId = Guid.NewGuid(),
                SensorTypeId = Guid.NewGuid(),
                MinValue = 0,
                MaxValue = 5,
            };

            _thresholdRepoMock
                .Setup(r => r.GetByIdAsync(entity.Id, QueryType.ActiveOnly))
                .ReturnsAsync(entity);
            _unitOfWorkMock.Setup(x => x.SaveChangesAsync(default)).ReturnsAsync(1);
            var dto = new UpdateSpeciesThresholdDto { MinValue = 1, MaxValue = 10 };

            var result = await _sut.UpdateSpeciesThreshold(entity.Id, dto);

            result.IsSuccess.Should().BeTrue();
            result.Type.Should().Be(ResultType.Ok);
            result.Message.Should().Be("Cập nhật ngưỡng sinh trưởng thành công.");

            _unitOfWorkMock.Verify(
                u => u.SaveChangesAsync(It.IsAny<CancellationToken>()),
                Times.Once
            );
        }

        [Fact]
        public async Task UpdateSpeciesThreshold_ShouldReturnNotFound_WhenMissing()
        {
            _thresholdRepoMock
                .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), QueryType.ActiveOnly))
                .ReturnsAsync((SpeciesThreshold?)null);
            _unitOfWorkMock.Setup(x => x.SaveChangesAsync(default)).ReturnsAsync(1);
            var result = await _sut.UpdateSpeciesThreshold(
                Guid.NewGuid(),
                new UpdateSpeciesThresholdDto()
            );

            result.IsSuccess.Should().BeFalse();
            result.Type.Should().Be(ResultType.NotFound);
            result.Message.Should().Be("Ngưỡng sinh trưởng không tồn tại.");
        }

        [Fact]
        public async Task UpdateSpeciesThreshold_ShouldReturnBadRequest_WhenMinGreaterOrEqualMax()
        {
            var entity = new SpeciesThreshold
            {
                Id = Guid.NewGuid(),
                SpeciesId = Guid.NewGuid(),
                GrowthStageId = Guid.NewGuid(),
                SensorTypeId = Guid.NewGuid(),
                MinValue = 0,
                MaxValue = 5,
            };
            var dto = new UpdateSpeciesThresholdDto { MinValue = 10, MaxValue = 5 };
            _unitOfWorkMock.Setup(x => x.SaveChangesAsync(default)).ReturnsAsync(1);
            var result = await _sut.UpdateSpeciesThreshold(entity.Id, dto);

            result.IsSuccess.Should().BeFalse();
            result.Type.Should().Be(ResultType.BadRequest);
            result.Message.Should().Be("Giá trị Min phải nhỏ hơn Max.");
            _unitOfWorkMock.Verify(
                u => u.SaveChangesAsync(It.IsAny<CancellationToken>()),
                Times.Never
            );
        }

        [Fact]
        public async Task UpdateSpeciesThreshold_ShouldReturnUnexpected_WhenExceptionThrown()
        {
            var entity = new SpeciesThreshold
            {
                Id = Guid.NewGuid(),
                SpeciesId = Guid.NewGuid(),
                GrowthStageId = Guid.NewGuid(),
                SensorTypeId = Guid.NewGuid(),
                MinValue = 0,
                MaxValue = 5,
            };
            _thresholdRepoMock
                .Setup(r => r.GetByIdAsync(entity.Id, QueryType.ActiveOnly))
                .ReturnsAsync(entity);
            _unitOfWorkMock
                .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception());
            var dto = new UpdateSpeciesThresholdDto { MinValue = 1, MaxValue = 10 };

            var result = await _sut.UpdateSpeciesThreshold(entity.Id, dto);

            result.IsSuccess.Should().BeFalse();
            result.Type.Should().Be(ResultType.Unexpected);
            result.Message.Should().Be("Lỗi khi cập nhật ngưỡng sinh trưởng.");
        }

        #endregion

        #region DeleteSpeciesThreshold

        [Fact]
        public async Task DeleteSpeciesThreshold_ShouldReturnOk_WhenSuccessful()
        {
            var entity = new SpeciesThreshold
            {
                Id = Guid.NewGuid(),
                SpeciesId = Guid.NewGuid(),
                GrowthStageId = Guid.NewGuid(),
                SensorTypeId = Guid.NewGuid(),
                MinValue = 0,
                MaxValue = 5,
            };

            _thresholdRepoMock
                .Setup(r => r.GetByIdAsync(entity.Id, QueryType.ActiveOnly))
                .ReturnsAsync(entity);
            _unitOfWorkMock.Setup(x => x.SaveChangesAsync(default)).ReturnsAsync(1);
            var result = await _sut.DeleteSpeciesThreshold(entity.Id);

            result.IsSuccess.Should().BeTrue();
            result.Type.Should().Be(ResultType.Ok);
            result.Message.Should().Be("Xóa ngưỡng sinh trưởng thành công.");
        }

        [Fact]
        public async Task DeleteSpeciesThreshold_ShouldReturnNotFound_WhenMissing()
        {
            _thresholdRepoMock
                .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), QueryType.ActiveOnly))
                .ReturnsAsync((SpeciesThreshold?)null);
            _unitOfWorkMock.Setup(x => x.SaveChangesAsync(default)).ReturnsAsync(1);
            var result = await _sut.DeleteSpeciesThreshold(Guid.NewGuid());

            result.IsSuccess.Should().BeFalse();
            result.Type.Should().Be(ResultType.NotFound);
            result.Message.Should().Be("Ngưỡng sinh trưởng không tồn tại.");
        }

        [Fact]
        public async Task DeleteSpeciesThreshold_ShouldReturnUnexpected_WhenExceptionThrown()
        {
            var entity = new SpeciesThreshold
            {
                Id = Guid.NewGuid(),
                SpeciesId = Guid.NewGuid(),
                GrowthStageId = Guid.NewGuid(),
                SensorTypeId = Guid.NewGuid(),
                MinValue = 0,
                MaxValue = 5,
            };
            _thresholdRepoMock
                .Setup(r => r.GetByIdAsync(entity.Id, QueryType.ActiveOnly))
                .ReturnsAsync(entity);
            _unitOfWorkMock.Setup(x => x.SaveChangesAsync(default)).ThrowsAsync(new Exception());
            var result = await _sut.DeleteSpeciesThreshold(entity.Id);

            result.IsSuccess.Should().BeFalse();
            result.Type.Should().Be(ResultType.Unexpected);
            result.Message.Should().Be("Lỗi khi xóa ngưỡng sinh trưởng.");
        }

        #endregion
    }
}

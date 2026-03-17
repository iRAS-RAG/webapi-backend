using System.Linq.Expressions;
using Ardalis.Specification;
using AutoMapper;
using FluentAssertions;
using IRasRag.Application.Common.Interfaces.Persistence;
using IRasRag.Application.Common.Interfaces.Persistence.Repositories;
using IRasRag.Application.Common.Mappings;
using IRasRag.Application.Common.Models;
using IRasRag.Application.Common.Models.Pagination;
using IRasRag.Application.DTOs;
using IRasRag.Application.Services.Implementations;
using IRasRag.Application.Specifications;
using IRasRag.Application.Specifications.SpeciesThresholdSpecifications;
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

            var persistedId = Guid.NewGuid();
            SpeciesThreshold? persistedThreshold = null;

            _thresholdRepoMock
                .Setup(r => r.AddAsync(It.IsAny<SpeciesThreshold>()))
                .Callback<SpeciesThreshold>(entity =>
                {
                    entity.Id = persistedId;
                    entity.Species = new Species { Id = dto.SpeciesId, Name = "Species" };
                    entity.GrowthStage = new GrowthStage
                    {
                        Id = dto.GrowthStageId,
                        Name = "Stage",
                    };
                    entity.SensorType = new SensorType
                    {
                        Id = dto.SensorTypeId,
                        Name = "Sensor",
                        UnitOfMeasure = "Unit",
                    };
                    persistedThreshold = entity;
                });

            _thresholdRepoMock
                .Setup(r =>
                    r.FirstOrDefaultAsync(
                        It.IsAny<ISpecification<SpeciesThreshold, SpeciesThresholdDto>>(),
                        It.IsAny<QueryType>()
                    )
                )
                .ReturnsAsync(
                    (
                        ISpecification<SpeciesThreshold, SpeciesThresholdDto> spec,
                        QueryType _
                    ) =>
                    {
                        if (persistedThreshold == null || spec.Selector == null)
                        {
                            return null;
                        }

                        IQueryable<SpeciesThreshold> query = new List<SpeciesThreshold>
                        {
                            persistedThreshold,
                        }.AsQueryable();

                        foreach (var whereExpression in spec.WhereExpressions)
                        {
                            query = query.Where(whereExpression.Filter);
                        }

                        return query.Select(spec.Selector).FirstOrDefault();
                    }
                );

            _unitOfWorkMock.Setup(x => x.SaveChangesAsync(default)).ReturnsAsync(1);
            var result = await _sut.CreateSpeciesThreshold(dto);

            result.IsSuccess.Should().BeTrue();
            result.Type.Should().Be(ResultType.Ok);
            result.Message.Should().Be("Tạo ngưỡng sinh trưởng thành công.");
            result.Data.Should().NotBeNull();
            result.Data!.Id.Should().Be(persistedId);
            result.Data.SpeciesId.Should().Be(dto.SpeciesId);
            result.Data.GrowthStageId.Should().Be(dto.GrowthStageId);
            result.Data.SensorTypeId.Should().Be(dto.SensorTypeId);
            result.Data.MinValue.Should().Be(dto.MinValue);
            result.Data.MaxValue.Should().Be(dto.MaxValue);
            result.Data.UnitOfMeasure.Should().Be("Unit");

            _thresholdRepoMock.Verify(r => r.AddAsync(It.IsAny<SpeciesThreshold>()), Times.Once);
            _thresholdRepoMock.Verify(
                r =>
                    r.FirstOrDefaultAsync(
                        It.Is<ISpecification<SpeciesThreshold, SpeciesThresholdDto>>(s =>
                            s is SpeciesThresholdDtoByIdSpec
                        ),
                        It.IsAny<QueryType>()
                    ),
                Times.Once
            );

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
                        It.IsAny<SpeciesThresholdDtoByIdSpec>(),
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
                        It.IsAny<SpeciesThresholdDtoByIdSpec>(),
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
                        It.IsAny<SpeciesThresholdDtoByIdSpec>(),
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
        public async Task GetAllSpeciesThresholdsAsync_ShouldApplySearchAndSort_FromSpecification()
        {
            // Arrange
            var request = new SpeciesThresholdListRequest
            {
                Page = 1,
                PageSize = 10,
                SearchTerm = "tilapia",
                SortBy = "sensortypename",
                SortDir = "desc",
            };

            var list = new List<SpeciesThreshold>
            {
                new SpeciesThreshold
                {
                    Id = Guid.NewGuid(),
                    Species = new Species { Id = Guid.NewGuid(), Name = "Tilapia" },
                    GrowthStage = new GrowthStage
                    {
                        Id = Guid.NewGuid(),
                        Name = "Juvenile",
                        Description = "desc",
                    },
                    SensorType = new SensorType { Id = Guid.NewGuid(), Name = "PH" },
                    MinValue = 1,
                    MaxValue = 2,
                },
                new SpeciesThreshold
                {
                    Id = Guid.NewGuid(),
                    Species = new Species { Id = Guid.NewGuid(), Name = "Tilapia" },
                    GrowthStage = new GrowthStage
                    {
                        Id = Guid.NewGuid(),
                        Name = "Adult",
                        Description = "desc",
                    },
                    SensorType = new SensorType { Id = Guid.NewGuid(), Name = "Temperature" },
                    MinValue = 1,
                    MaxValue = 2,
                },
                new SpeciesThreshold
                {
                    Id = Guid.NewGuid(),
                    Species = new Species { Id = Guid.NewGuid(), Name = "Catfish" },
                    GrowthStage = new GrowthStage
                    {
                        Id = Guid.NewGuid(),
                        Name = "Adult",
                        Description = "desc",
                    },
                    SensorType = new SensorType { Id = Guid.NewGuid(), Name = "Dissolved Oxygen" },
                    MinValue = 1,
                    MaxValue = 2,
                },
            };

            ISpecification<SpeciesThreshold, SpeciesThresholdDto>? capturedSpec = null;

            _thresholdRepoMock
                .Setup(r =>
                    r.GetPagedAsync<SpeciesThresholdDto>(
                        It.IsAny<ISpecification<SpeciesThreshold, SpeciesThresholdDto>>(),
                        request.Page,
                        request.PageSize,
                        It.IsAny<QueryType>()
                    )
                )
                .Callback(
                    (
                        ISpecification<SpeciesThreshold, SpeciesThresholdDto> spec,
                        int _,
                        int _,
                        QueryType _
                    ) => capturedSpec = spec
                )
                .ReturnsAsync(
                    (
                        ISpecification<SpeciesThreshold, SpeciesThresholdDto> spec,
                        int page,
                        int pageSize,
                        QueryType _
                    ) =>
                        SpecificationTestHelper.ApplySpecificationWithPaging(
                            list,
                            spec,
                            page,
                            pageSize
                        )
                );

            // Act
            var result = await _sut.GetAllSpeciesThresholdsAsync(request);

            // Assert
            result.Should().NotBeNull();
            result.Message.Should().Be("Lấy danh sách ngưỡng sinh trưởng thành công");
            result.Data.Should().NotBeNull();
            result.Data!.Count.Should().Be(2);
            result.Data.Select(x => x.SensorTypeName).Should().ContainInOrder("Temperature", "PH");

            result.Meta.Should().NotBeNull();
            result.Meta!.Page.Should().Be(request.Page);
            result.Meta.PageSize.Should().Be(request.PageSize);
            result.Meta.TotalItems.Should().Be(2);

            result.Links.Should().NotBeNull();
            capturedSpec.Should().NotBeNull();
            capturedSpec.Should().BeOfType<SpeciesThresholdListSpec>();

            _thresholdRepoMock.Verify(
                r =>
                    r.GetPagedAsync<SpeciesThresholdDto>(
                        It.Is<ISpecification<SpeciesThreshold, SpeciesThresholdDto>>(s =>
                            s is SpeciesThresholdListSpec
                        ),
                        request.Page,
                        request.PageSize,
                        It.IsAny<QueryType>()
                    ),
                Times.Once
            );
        }

        [Fact]
        public async Task GetAllSpeciesThresholdsAsync_ShouldApplyDefaultSortBySpeciesName_WhenSortByIsNull()
        {
            // Arrange
            var request = new SpeciesThresholdListRequest { Page = 1, PageSize = 10 };

            var list = new List<SpeciesThreshold>
            {
                new SpeciesThreshold
                {
                    Id = Guid.NewGuid(),
                    Species = new Species { Id = Guid.NewGuid(), Name = "Zulu" },
                    GrowthStage = new GrowthStage
                    {
                        Id = Guid.NewGuid(),
                        Name = "Juvenile",
                        Description = "desc",
                    },
                    SensorType = new SensorType { Id = Guid.NewGuid(), Name = "PH" },
                    MinValue = 1,
                    MaxValue = 2,
                },
                new SpeciesThreshold
                {
                    Id = Guid.NewGuid(),
                    Species = new Species { Id = Guid.NewGuid(), Name = "Alpha" },
                    GrowthStage = new GrowthStage
                    {
                        Id = Guid.NewGuid(),
                        Name = "Adult",
                        Description = "desc",
                    },
                    SensorType = new SensorType { Id = Guid.NewGuid(), Name = "Temperature" },
                    MinValue = 1,
                    MaxValue = 2,
                },
            };

            _thresholdRepoMock
                .Setup(r =>
                    r.GetPagedAsync<SpeciesThresholdDto>(
                        It.IsAny<ISpecification<SpeciesThreshold, SpeciesThresholdDto>>(),
                        request.Page,
                        request.PageSize,
                        It.IsAny<QueryType>()
                    )
                )
                .ReturnsAsync(
                    (
                        ISpecification<SpeciesThreshold, SpeciesThresholdDto> spec,
                        int page,
                        int pageSize,
                        QueryType _
                    ) =>
                        SpecificationTestHelper.ApplySpecificationWithPaging(
                            list,
                            spec,
                            page,
                            pageSize
                        )
                );

            // Act
            var result = await _sut.GetAllSpeciesThresholdsAsync(request);

            // Assert
            result.Data.Should().NotBeNull();
            result.Data!.Select(x => x.SpeciesName).Should().ContainInOrder("Alpha", "Zulu");

            _thresholdRepoMock.Verify(
                r =>
                    r.GetPagedAsync<SpeciesThresholdDto>(
                        It.Is<ISpecification<SpeciesThreshold, SpeciesThresholdDto>>(s =>
                            s is SpeciesThresholdListSpec
                        ),
                        request.Page,
                        request.PageSize,
                        It.IsAny<QueryType>()
                    ),
                Times.Once
            );
        }

        [Fact]
        public async Task GetAllSpeciesThresholdsAsync_ShouldReturnErrorMessage_WhenExceptionThrown()
        {
            // Arrange
            var request = new SpeciesThresholdListRequest { Page = 1, PageSize = 10 };

            _thresholdRepoMock
                .Setup(r =>
                    r.GetPagedAsync<SpeciesThresholdDto>(
                        It.IsAny<ISpecification<SpeciesThreshold, SpeciesThresholdDto>>(),
                        request.Page,
                        request.PageSize,
                        It.IsAny<QueryType>()
                    )
                )
                .ThrowsAsync(new Exception());

            // Act
            var result = await _sut.GetAllSpeciesThresholdsAsync(request);

            // Assert
            result.Should().NotBeNull();
            result.Message.Should().Be("Lỗi khi truy xuất danh sách ngưỡng sinh trưởng");

            result.Data.Should().NotBeNull();
            result.Data!.Should().BeEmpty();

            result.Meta.Should().BeNull();
            result.Links.Should().BeNull();

            _thresholdRepoMock.Verify(
                r =>
                    r.GetPagedAsync<SpeciesThresholdDto>(
                        It.Is<ISpecification<SpeciesThreshold, SpeciesThresholdDto>>(s =>
                            s != null
                        ),
                        request.Page,
                        request.PageSize,
                        It.IsAny<QueryType>()
                    ),
                Times.Once
            );
        }

        [Fact]
        public async Task GetAllSpeciesThresholdsAsync_ShouldReturnEmptyList_WhenNoThresholdsExist()
        {
            // Arrange
            var request = new SpeciesThresholdListRequest { Page = 1, PageSize = 10 };

            _thresholdRepoMock
                .Setup(r =>
                    r.GetPagedAsync<SpeciesThresholdDto>(
                        It.IsAny<ISpecification<SpeciesThreshold, SpeciesThresholdDto>>(),
                        request.Page,
                        request.PageSize,
                        It.IsAny<QueryType>()
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
            var result = await _sut.GetAllSpeciesThresholdsAsync(request);

            // Assert
            result.Should().NotBeNull();
            result.Message.Should().Be("Không có ngưỡng sinh trưởng nào");

            result.Data.Should().NotBeNull();
            result.Data!.Should().BeEmpty();

            result.Meta.Should().NotBeNull();
            result.Meta!.TotalItems.Should().Be(0);

            result.Links.Should().NotBeNull();

            _thresholdRepoMock.Verify(
                r =>
                    r.GetPagedAsync<SpeciesThresholdDto>(
                        It.Is<ISpecification<SpeciesThreshold, SpeciesThresholdDto>>(s =>
                            s != null
                        ),
                        request.Page,
                        request.PageSize,
                        It.IsAny<QueryType>()
                    ),
                Times.Once
            );
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

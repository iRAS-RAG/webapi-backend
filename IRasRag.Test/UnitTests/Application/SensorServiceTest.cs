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
using IRasRag.Application.Specifications.SensorSpecifications;
using IRasRag.Domain.Entities;
using IRasRag.Domain.Enums;
using IRasRag.Test.UnitTests.Helpers;
using Microsoft.Extensions.Logging;
using Moq;

namespace IRasRag.Test.UnitTests.Application
{
    public class SensorServiceTest
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<ILogger<SensorService>> _loggerMock;
        private readonly IMapper _mapper;

        private readonly Mock<IRepository<Sensor>> _sensorRepoMock;
        private readonly Mock<IRepository<SensorType>> _sensorTypeRepoMock;
        private readonly Mock<IRepository<MasterBoard>> _masterBoardRepoMock;
        private readonly Mock<IRepository<SensorLog>> _logRepoMock;

        private readonly SensorService _sut;

        public SensorServiceTest()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _loggerMock = new Mock<ILogger<SensorService>>();
            _mapper = AutoMapperTestHelper.GetMapper(new SensorProfile());

            _sensorRepoMock = new Mock<IRepository<Sensor>>();
            _sensorTypeRepoMock = new Mock<IRepository<SensorType>>();
            _masterBoardRepoMock = new Mock<IRepository<MasterBoard>>();
            _logRepoMock = new Mock<IRepository<SensorLog>>();

            _unitOfWorkMock.Setup(x => x.GetRepository<Sensor>()).Returns(_sensorRepoMock.Object);
            _unitOfWorkMock
                .Setup(x => x.GetRepository<SensorType>())
                .Returns(_sensorTypeRepoMock.Object);
            _unitOfWorkMock
                .Setup(x => x.GetRepository<MasterBoard>())
                .Returns(_masterBoardRepoMock.Object);
            _unitOfWorkMock.Setup(x => x.GetRepository<SensorLog>()).Returns(_logRepoMock.Object);

            _sut = new SensorService(_unitOfWorkMock.Object, _loggerMock.Object, _mapper);
        }

        #region GetSensorByIdAsync Tests

        [Fact]
        public async Task GetSensorByIdAsync_ShouldReturnSuccess_WhenSensorExists()
        {
            // Arrange
            var sensorId = Guid.NewGuid();
            var sensor = new Sensor
            {
                Id = sensorId,
                Name = "Cảm biến nhiệt độ",
                PinCode = 1,
                SensorTypeId = Guid.NewGuid(),
                MasterBoardId = Guid.NewGuid(),
                SensorType = new SensorType { Name = "Nhiệt độ" },
                MasterBoard = new MasterBoard { Name = "Board A" },
            };

            _sensorRepoMock
                .Setup(r => r.GetByIdAsync(sensorId, QueryType.ActiveOnly))
                .ReturnsAsync(sensor);

            // Act
            var result = await _sut.GetSensorByIdAsync(sensorId);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Data.Should().NotBeNull();
            result.Data!.Id.Should().Be(sensorId);
            result.Data.Name.Should().Be("Cảm biến nhiệt độ");
        }

        [Fact]
        public async Task GetSensorByIdAsync_ShouldReturnNotFound_WhenSensorDoesNotExist()
        {
            // Arrange
            var sensorId = Guid.NewGuid();
            _sensorRepoMock
                .Setup(r => r.GetByIdAsync(sensorId, QueryType.ActiveOnly))
                .ReturnsAsync((Sensor?)null);

            // Act
            var result = await _sut.GetSensorByIdAsync(sensorId);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Type.Should().Be(ResultType.NotFound);
        }

        [Fact]
        public async Task GetSensorByIdAsync_ShouldReturnUnexpected_WhenExceptionThrown()
        {
            // Arrange
            var sensorId = Guid.NewGuid();
            _sensorRepoMock
                .Setup(r => r.GetByIdAsync(sensorId, QueryType.ActiveOnly))
                .ThrowsAsync(new Exception());

            // Act
            var result = await _sut.GetSensorByIdAsync(sensorId);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Type.Should().Be(ResultType.Unexpected);
        }

        #endregion

        #region GetAllSensorsAsync Tests

        [Fact]
        public async Task GetAllSensorsAsync_ShouldReturnPagedResult_WhenSensorsExist()
        {
            // Arrange
            var request = new SensorListRequest { Page = 1, PageSize = 10 };
            var sensors = new List<Sensor>
            {
                new Sensor
                {
                    Id = Guid.NewGuid(),
                    Name = "Sensor B",
                    PinCode = 2,
                    SensorType = new SensorType { Name = "pH" },
                    MasterBoard = new MasterBoard { Name = "Board 1" },
                },
                new Sensor
                {
                    Id = Guid.NewGuid(),
                    Name = "Sensor A",
                    PinCode = 1,
                    SensorType = new SensorType { Name = "Nhiệt độ" },
                    MasterBoard = new MasterBoard { Name = "Board 1" },
                },
            };

            _sensorRepoMock
                .Setup(r =>
                    r.GetPagedAsync(
                        It.IsAny<ISpecification<Sensor, SensorDto>>(),
                        request.Page,
                        request.PageSize,
                        It.IsAny<QueryType>()
                    )
                )
                .ReturnsAsync(
                    (ISpecification<Sensor, SensorDto> spec, int page, int pageSize, QueryType _) =>
                        SpecificationTestHelper.ApplySpecificationWithPaging(
                            sensors,
                            spec,
                            page,
                            pageSize
                        )
                );

            // Act
            var result = await _sut.GetAllSensorsAsync(request);

            // Assert
            result.Should().NotBeNull();
            result.Data.Should().HaveCount(2);
            result.Data.Select(x => x.Name).Should().ContainInOrder("Sensor A", "Sensor B");
            result.Meta.Should().NotBeNull();
            result.Meta!.TotalItems.Should().Be(2);

            _sensorRepoMock.Verify(
                r =>
                    r.GetPagedAsync(
                        It.Is<ISpecification<Sensor, SensorDto>>(s => s is SensorDtoListSpec),
                        request.Page,
                        request.PageSize,
                        It.IsAny<QueryType>()
                    ),
                Times.Once
            );
        }

        [Fact]
        public async Task GetAllSensorsAsync_ShouldReturnEmptyList_WhenNoSensorsExist()
        {
            // Arrange
            var request = new SensorListRequest { Page = 1, PageSize = 10 };

            _sensorRepoMock
                .Setup(r =>
                    r.GetPagedAsync(
                        It.IsAny<ISpecification<Sensor, SensorDto>>(),
                        request.Page,
                        request.PageSize,
                        It.IsAny<QueryType>()
                    )
                )
                .ReturnsAsync(
                    new PagedResult<SensorDto> { Items = new List<SensorDto>(), TotalItems = 0 }
                );

            // Act
            var result = await _sut.GetAllSensorsAsync(request);

            // Assert
            result.Data.Should().BeEmpty();
            result.Meta!.TotalItems.Should().Be(0);
            result.Message.Should().Be("Không có cảm biến nào");
        }

        [Fact]
        public async Task GetAllSensorsAsync_ShouldReturnError_WhenExceptionThrown()
        {
            // Arrange
            var request = new SensorListRequest { Page = 1, PageSize = 10 };
            _sensorRepoMock
                .Setup(r =>
                    r.GetPagedAsync(
                        It.IsAny<ISpecification<Sensor, SensorDto>>(),
                        It.IsAny<int>(),
                        It.IsAny<int>(),
                        It.IsAny<QueryType>()
                    )
                )
                .ThrowsAsync(new Exception());

            // Act
            var result = await _sut.GetAllSensorsAsync(request);

            // Assert
            result.Data.Should().BeEmpty();
            result.Meta.Should().BeNull();
        }

        #endregion

        #region CreateSensorAsync Tests

        [Fact]
        public async Task CreateSensorAsync_ShouldReturnSuccess_WhenValidInput()
        {
            // Arrange
            var sensorTypeId = Guid.NewGuid();
            var masterBoardId = Guid.NewGuid();
            var dto = new CreateSensorDto
            {
                Name = "Sensor mới",
                PinCode = 5,
                SensorTypeId = sensorTypeId,
                MasterBoardId = masterBoardId,
            };
            var sensorType = new SensorType { Id = sensorTypeId, Name = "Độ mặn" };
            var masterBoard = new MasterBoard { Id = masterBoardId, Name = "Board X" };

            _sensorTypeRepoMock
                .Setup(r => r.GetByIdAsync(sensorTypeId, QueryType.ActiveOnly))
                .ReturnsAsync(sensorType);
            _masterBoardRepoMock
                .Setup(r => r.GetByIdAsync(masterBoardId, QueryType.ActiveOnly))
                .ReturnsAsync(masterBoard);
            _sensorRepoMock
                .Setup(r =>
                    r.AnyAsync(It.IsAny<Expression<Func<Sensor, bool>>>(), QueryType.ActiveOnly)
                )
                .ReturnsAsync(false);

            _sensorRepoMock
                .Setup(r => r.FirstOrDefaultAsync(It.IsAny<ISpecification<Sensor, SensorDto>>(), QueryType.ActiveOnly))
                .ReturnsAsync(new SensorDto
                {
                    Id = Guid.NewGuid(),
                    Name = "Sensor mới",
                    PinCode = 5,
                    SensorTypeId = sensorTypeId,
                    SensorTypeName = "Độ mặn",
                    MasterBoardId = masterBoardId,
                    MasterBoardName = "Board X",
                });

            // Act
            var result = await _sut.CreateSensorAsync(dto);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Data.Should().NotBeNull();
            result.Data!.Name.Should().Be("Sensor mới");
            result.Data.SensorTypeName.Should().Be("Độ mặn");
            result.Data.MasterBoardName.Should().Be("Board X");
            _sensorRepoMock.Verify(r => r.AddAsync(It.IsAny<Sensor>()), Times.Once);
            _unitOfWorkMock.Verify(
                u => u.SaveChangesAsync(It.IsAny<CancellationToken>()),
                Times.Once
            );
        }

        [Fact]
        public async Task CreateSensorAsync_ShouldReturnNotFound_WhenSensorTypeDoesNotExist()
        {
            // Arrange
            var dto = new CreateSensorDto
            {
                Name = "Sensor",
                PinCode = 1,
                SensorTypeId = Guid.NewGuid(),
                MasterBoardId = Guid.NewGuid(),
            };
            _sensorTypeRepoMock
                .Setup(r => r.GetByIdAsync(dto.SensorTypeId, QueryType.ActiveOnly))
                .ReturnsAsync((SensorType?)null);

            // Act
            var result = await _sut.CreateSensorAsync(dto);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Type.Should().Be(ResultType.NotFound);
            _sensorRepoMock.Verify(r => r.AddAsync(It.IsAny<Sensor>()), Times.Never);
        }

        [Fact]
        public async Task CreateSensorAsync_ShouldReturnNotFound_WhenMasterBoardDoesNotExist()
        {
            // Arrange
            var sensorTypeId = Guid.NewGuid();
            var dto = new CreateSensorDto
            {
                Name = "Sensor",
                PinCode = 1,
                SensorTypeId = sensorTypeId,
                MasterBoardId = Guid.NewGuid(),
            };
            _sensorTypeRepoMock
                .Setup(r => r.GetByIdAsync(sensorTypeId, QueryType.ActiveOnly))
                .ReturnsAsync(new SensorType { Id = sensorTypeId, Name = "Type" });
            _masterBoardRepoMock
                .Setup(r => r.GetByIdAsync(dto.MasterBoardId, QueryType.ActiveOnly))
                .ReturnsAsync((MasterBoard?)null);

            // Act
            var result = await _sut.CreateSensorAsync(dto);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Type.Should().Be(ResultType.NotFound);
            _sensorRepoMock.Verify(r => r.AddAsync(It.IsAny<Sensor>()), Times.Never);
        }

        [Fact]
        public async Task CreateSensorAsync_ShouldReturnConflict_WhenDuplicatePinCodeOnSameBoard()
        {
            // Arrange
            var sensorTypeId = Guid.NewGuid();
            var masterBoardId = Guid.NewGuid();
            var dto = new CreateSensorDto
            {
                Name = "Sensor",
                PinCode = 3,
                SensorTypeId = sensorTypeId,
                MasterBoardId = masterBoardId,
            };
            _sensorTypeRepoMock
                .Setup(r => r.GetByIdAsync(sensorTypeId, QueryType.ActiveOnly))
                .ReturnsAsync(new SensorType { Id = sensorTypeId, Name = "Type" });
            _masterBoardRepoMock
                .Setup(r => r.GetByIdAsync(masterBoardId, QueryType.ActiveOnly))
                .ReturnsAsync(new MasterBoard { Id = masterBoardId, Name = "Board" });
            _sensorRepoMock
                .Setup(r =>
                    r.AnyAsync(It.IsAny<Expression<Func<Sensor, bool>>>(), QueryType.ActiveOnly)
                )
                .ReturnsAsync(true);

            // Act
            var result = await _sut.CreateSensorAsync(dto);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Type.Should().Be(ResultType.Conflict);
            _sensorRepoMock.Verify(r => r.AddAsync(It.IsAny<Sensor>()), Times.Never);
        }

        [Fact]
        public async Task CreateSensorAsync_ShouldReturnUnexpected_WhenExceptionThrown()
        {
            // Arrange
            var dto = new CreateSensorDto
            {
                Name = "Sensor",
                PinCode = 1,
                SensorTypeId = Guid.NewGuid(),
                MasterBoardId = Guid.NewGuid(),
            };
            _sensorTypeRepoMock
                .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), QueryType.ActiveOnly))
                .ThrowsAsync(new Exception());

            // Act
            var result = await _sut.CreateSensorAsync(dto);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Type.Should().Be(ResultType.Unexpected);
        }

        #endregion

        #region DeleteSensorAsync Tests

        [Fact]
        public async Task DeleteSensorAsync_ShouldReturnSuccess_WhenSensorExistsAndHasNoRelations()
        {
            // Arrange
            var sensorId = Guid.NewGuid();
            var sensor = new Sensor { Id = sensorId, Name = "Sensor A" };

            _sensorRepoMock
                .Setup(r => r.GetByIdAsync(sensorId, QueryType.ActiveOnly))
                .ReturnsAsync(sensor);
            _sensorRepoMock
                .SetupSequence(r =>
                    r.AnyAsync(It.IsAny<Expression<Func<Sensor, bool>>>(), QueryType.ActiveOnly)
                )
                .ReturnsAsync(false) // hasSensorLogs
                .ReturnsAsync(false); // hasJobs

            // Act
            var result = await _sut.DeleteSensorAsync(sensorId);

            // Assert
            result.IsSuccess.Should().BeTrue();
            _sensorRepoMock.Verify(r => r.Delete(sensor), Times.Once);
            _unitOfWorkMock.Verify(
                u => u.SaveChangesAsync(It.IsAny<CancellationToken>()),
                Times.Once
            );
        }

        [Fact]
        public async Task DeleteSensorAsync_ShouldReturnNotFound_WhenSensorDoesNotExist()
        {
            // Arrange
            var sensorId = Guid.NewGuid();
            _sensorRepoMock
                .Setup(r => r.GetByIdAsync(sensorId, QueryType.ActiveOnly))
                .ReturnsAsync((Sensor?)null);

            // Act
            var result = await _sut.DeleteSensorAsync(sensorId);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Type.Should().Be(ResultType.NotFound);
            _sensorRepoMock.Verify(r => r.Delete(It.IsAny<Sensor>()), Times.Never);
        }

        [Fact]
        public async Task DeleteSensorAsync_ShouldReturnConflict_WhenSensorHasSensorLogs()
        {
            // Arrange
            var sensorId = Guid.NewGuid();
            var sensor = new Sensor { Id = sensorId, Name = "Sensor A" };

            _sensorRepoMock
                .Setup(r => r.GetByIdAsync(sensorId, QueryType.ActiveOnly))
                .ReturnsAsync(sensor);
            _sensorRepoMock
                .Setup(r =>
                    r.AnyAsync(It.IsAny<Expression<Func<Sensor, bool>>>(), QueryType.ActiveOnly)
                )
                .ReturnsAsync(true); // hasSensorLogs = true

            // Act
            var result = await _sut.DeleteSensorAsync(sensorId);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Type.Should().Be(ResultType.Conflict);
            _sensorRepoMock.Verify(r => r.Delete(It.IsAny<Sensor>()), Times.Never);
        }

        [Fact]
        public async Task DeleteSensorAsync_ShouldReturnConflict_WhenSensorHasJobs()
        {
            // Arrange
            var sensorId = Guid.NewGuid();
            var sensor = new Sensor { Id = sensorId, Name = "Sensor A" };

            _sensorRepoMock
                .Setup(r => r.GetByIdAsync(sensorId, QueryType.ActiveOnly))
                .ReturnsAsync(sensor);
            _sensorRepoMock
                .SetupSequence(r =>
                    r.AnyAsync(It.IsAny<Expression<Func<Sensor, bool>>>(), QueryType.ActiveOnly)
                )
                .ReturnsAsync(false) // hasSensorLogs = false
                .ReturnsAsync(true); // hasJobs = true

            // Act
            var result = await _sut.DeleteSensorAsync(sensorId);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Type.Should().Be(ResultType.Conflict);
            _sensorRepoMock.Verify(r => r.Delete(It.IsAny<Sensor>()), Times.Never);
        }

        [Fact]
        public async Task DeleteSensorAsync_ShouldReturnUnexpected_WhenExceptionThrown()
        {
            // Arrange
            var sensorId = Guid.NewGuid();
            _sensorRepoMock
                .Setup(r => r.GetByIdAsync(sensorId, QueryType.ActiveOnly))
                .ThrowsAsync(new Exception());

            // Act
            var result = await _sut.DeleteSensorAsync(sensorId);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Type.Should().Be(ResultType.Unexpected);
        }

        #endregion

        #region CreateSensorLogAsync Tests

        [Fact]
        public async Task CreateSensorLogAsync_ShouldReturnSuccess_WhenSensorExists()
        {
            // Arrange
            var sensorId = Guid.NewGuid();
            var sensor = new Sensor { Id = sensorId, Name = "Sensor" };
            var dto = new CreateSensorLogDto { Data = 25.5 };

            _sensorRepoMock
                .Setup(r => r.GetByIdAsync(sensorId, QueryType.ActiveOnly))
                .ReturnsAsync(sensor);
            _logRepoMock
                .Setup(r => r.AddAsync(It.IsAny<SensorLog>()))
                .Callback<SensorLog>(log => log.Id = Guid.NewGuid()); // simulate DB-generated Id

            _logRepoMock
                .Setup(r => r.FirstOrDefaultAsync(It.IsAny<ISpecification<SensorLog, SensorLogDto>>(), QueryType.ActiveOnly))
                .ReturnsAsync(new SensorLogDto
                {
                    Id = Guid.NewGuid(),
                    SensorId = sensorId,
                    Data = 25.5,
                    IsWarning = false,
                    DataJson = "{}",
                    CreatedAt = DateTime.UtcNow,
                });

            // Act
            var result = await _sut.CreateSensorLogAsync(sensorId, dto);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Data.Should().NotBeNull();
            result.Data!.Data.Should().Be(25.5);
            result.Data.SensorId.Should().Be(sensorId);
            _logRepoMock.Verify(r => r.AddAsync(It.IsAny<SensorLog>()), Times.Once);
            _unitOfWorkMock.Verify(
                u => u.SaveChangesAsync(It.IsAny<CancellationToken>()),
                Times.Once
            );
            // Without timestamp: Update should NOT be called
            _logRepoMock.Verify(r => r.Update(It.IsAny<SensorLog>()), Times.Never);
        }

        [Fact]
        public async Task CreateSensorLogAsync_ShouldOverrideTimestamp_WhenTimestampProvided()
        {
            // Arrange
            var sensorId = Guid.NewGuid();
            var sensor = new Sensor { Id = sensorId, Name = "Sensor" };
            var customTime = new DateTime(2025, 6, 1, 12, 0, 0, DateTimeKind.Utc);
            var dto = new CreateSensorLogDto { Data = 30.0, Timestamp = customTime };

            _sensorRepoMock
                .Setup(r => r.GetByIdAsync(sensorId, QueryType.ActiveOnly))
                .ReturnsAsync(sensor);

            SensorLog? capturedLog = null;
            _logRepoMock
                .Setup(r => r.AddAsync(It.IsAny<SensorLog>()))
                .Callback<SensorLog>(log => capturedLog = log);

            // Act
            var result = await _sut.CreateSensorLogAsync(sensorId, dto);

            // Assert
            result.IsSuccess.Should().BeTrue();
            capturedLog.Should().NotBeNull();
            capturedLog!.CreatedAt.Should().Be(customTime);
            // Timestamp is set before AddAsync — no second save or Update needed
            _logRepoMock.Verify(r => r.Update(It.IsAny<SensorLog>()), Times.Never);
            _unitOfWorkMock.Verify(
                u => u.SaveChangesAsync(It.IsAny<CancellationToken>()),
                Times.Once
            );
        }

        [Fact]
        public async Task CreateSensorLogAsync_ShouldReturnNotFound_WhenSensorDoesNotExist()
        {
            // Arrange
            var sensorId = Guid.NewGuid();
            _sensorRepoMock
                .Setup(r => r.GetByIdAsync(sensorId, QueryType.ActiveOnly))
                .ReturnsAsync((Sensor?)null);

            // Act
            var result = await _sut.CreateSensorLogAsync(
                sensorId,
                new CreateSensorLogDto { Data = 1.0 }
            );

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Type.Should().Be(ResultType.NotFound);
            _logRepoMock.Verify(r => r.AddAsync(It.IsAny<SensorLog>()), Times.Never);
        }

        [Fact]
        public async Task CreateSensorLogAsync_ShouldReturnUnexpected_WhenExceptionThrown()
        {
            // Arrange
            var sensorId = Guid.NewGuid();
            _sensorRepoMock
                .Setup(r => r.GetByIdAsync(sensorId, QueryType.ActiveOnly))
                .ThrowsAsync(new Exception());

            // Act
            var result = await _sut.CreateSensorLogAsync(
                sensorId,
                new CreateSensorLogDto { Data = 1.0 }
            );

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Type.Should().Be(ResultType.Unexpected);
        }

        #endregion

        #region GetSensorLogsAsync Tests

        [Fact]
        public async Task GetSensorLogsAsync_ShouldReturnRawLogs_WhenIntervalIsNull()
        {
            // Arrange
            var sensorId = Guid.NewGuid();
            var sensor = new Sensor { Id = sensorId, Name = "Sensor" };
            var request = new SensorLogListRequest
            {
                From = null,
                To = null,
                Interval = null,
            };
            var logs = new List<SensorLogDto>
            {
                new SensorLogDto
                {
                    Id = Guid.NewGuid(),
                    SensorId = sensorId,
                    Data = 10.0,
                    CreatedAt = DateTime.UtcNow.AddMinutes(-10),
                },
                new SensorLogDto
                {
                    Id = Guid.NewGuid(),
                    SensorId = sensorId,
                    Data = 20.0,
                    CreatedAt = DateTime.UtcNow.AddMinutes(-5),
                },
                new SensorLogDto
                {
                    Id = Guid.NewGuid(),
                    SensorId = sensorId,
                    Data = 30.0,
                    CreatedAt = DateTime.UtcNow,
                },
            };

            _sensorRepoMock
                .Setup(r => r.GetByIdAsync(sensorId, QueryType.ActiveOnly))
                .ReturnsAsync(sensor);
            _logRepoMock
                .Setup(r =>
                    r.GetPagedAsync(
                        It.IsAny<ISpecification<SensorLog, SensorLogDto>>(),
                        It.IsAny<int>(),
                        It.IsAny<int>(),
                        QueryType.ActiveOnly
                    )
                )
                .ReturnsAsync(
                    new PagedResult<SensorLogDto> { Items = logs, TotalItems = logs.Count }
                );

            // Act
            var result = await _sut.GetSensorLogsAsync(sensorId, request);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Data!.Data.Should().HaveCount(3);
            result
                .Data.Data!.Select(l => l.Data)
                .Should()
                .BeEquivalentTo(new[] { 10.0, 20.0, 30.0 });
            result.Data.Meta!.TotalItems.Should().Be(3);
        }

        [Fact]
        public async Task GetSensorLogsAsync_ShouldAggregateByInterval_WhenIntervalIsProvided()
        {
            // Arrange
            var sensorId = Guid.NewGuid();
            var sensor = new Sensor { Id = sensorId, Name = "Sensor" };
            var request = new SensorLogListRequest
            {
                Interval = 60, // 60-minute buckets
                From = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                To = new DateTime(2025, 1, 1, 2, 0, 0, DateTimeKind.Utc),
            };

            var baseTime = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            var logs = new List<SensorLogDto>
            {
                // Bucket 1: 00:00 UTC — values 10, 20 → avg = 15
                new SensorLogDto
                {
                    Id = Guid.NewGuid(),
                    SensorId = sensorId,
                    Data = 10.0,
                    IsWarning = false,
                    DataJson = "{}",
                    CreatedAt = baseTime.AddMinutes(10),
                },
                new SensorLogDto
                {
                    Id = Guid.NewGuid(),
                    SensorId = sensorId,
                    Data = 20.0,
                    IsWarning = false,
                    DataJson = "{}",
                    CreatedAt = baseTime.AddMinutes(40),
                },
                // Bucket 2: 01:00 UTC — values 30, 40 → avg = 35
                new SensorLogDto
                {
                    Id = Guid.NewGuid(),
                    SensorId = sensorId,
                    Data = 30.0,
                    IsWarning = false,
                    DataJson = "{}",
                    CreatedAt = baseTime.AddMinutes(70),
                },
                new SensorLogDto
                {
                    Id = Guid.NewGuid(),
                    SensorId = sensorId,
                    Data = 40.0,
                    IsWarning = true,
                    DataJson = "{}",
                    CreatedAt = baseTime.AddMinutes(110),
                },
            };

            _sensorRepoMock
                .Setup(r => r.GetByIdAsync(sensorId, QueryType.ActiveOnly))
                .ReturnsAsync(sensor);

            var mockSensorLogRepository = new Mock<ISensorLogRepository>();
            var aggregatedLogs = new List<SensorLogDto>
            {
                new SensorLogDto
                {
                    Id = Guid.NewGuid(),
                    SensorId = sensorId,
                    Data = 15.0,
                    IsWarning = false,
                    DataJson = "{}",
                    CreatedAt = baseTime,
                },
                new SensorLogDto
                {
                    Id = Guid.NewGuid(),
                    SensorId = sensorId,
                    Data = 35.0,
                    IsWarning = true,
                    DataJson = "{}",
                    CreatedAt = baseTime.AddHours(1),
                },
            };
            mockSensorLogRepository
                .Setup(r => r.GetAggregatedLogsAsync(
                    sensorId,
                    request.From!.Value,
                    request.To!.Value,
                    request.Interval!.Value,
                    request.Page,
                    request.PageSize
                ))
                .ReturnsAsync((items: aggregatedLogs.Cast<SensorLogDto>().ToList() as IReadOnlyList<SensorLogDto>, totalCount: aggregatedLogs.Count));
            _unitOfWorkMock.Setup(u => u.SensorLogs).Returns(mockSensorLogRepository.Object);

            // Act
            var result = await _sut.GetSensorLogsAsync(sensorId, request);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Data!.Data.Should().HaveCount(2);
            result.Data.Meta!.TotalItems.Should().Be(2);

            var bucket1 = result.Data.Data![0];
            bucket1.Data.Should().Be(15.0); // avg(10, 20)
            bucket1.IsWarning.Should().BeFalse(); // no warning in bucket 1

            var bucket2 = result.Data.Data[1];
            bucket2.Data.Should().Be(35.0); // avg(30, 40)
            bucket2.IsWarning.Should().BeTrue(); // one warning in bucket 2

            // Bucket keys should align to the hour boundary
            result.Data.Data[0].CreatedAt.Should().Be(baseTime); // 00:00
            result.Data.Data[1].CreatedAt.Should().Be(baseTime.AddHours(1)); // 01:00

            mockSensorLogRepository.Verify(
                r => r.GetAggregatedLogsAsync(
                    sensorId,
                    request.From!.Value,
                    request.To!.Value,
                    request.Interval!.Value,
                    request.Page,
                    request.PageSize
                ),
                Times.Once
            );

            _logRepoMock.Verify(
                r =>
                    r.GetPagedAsync(
                        It.IsAny<ISpecification<SensorLog, SensorLogDto>>(),
                        It.IsAny<int>(),
                        It.IsAny<int>(),
                        It.IsAny<QueryType>()
                    ),
                Times.Never
            );
        }

        [Fact]
        public async Task GetSensorLogsAsync_ShouldReturnEmptyList_WhenNoLogsExist()
        {
            // Arrange
            var sensorId = Guid.NewGuid();
            var sensor = new Sensor { Id = sensorId, Name = "Sensor" };
            var request = new SensorLogListRequest();

            _sensorRepoMock
                .Setup(r => r.GetByIdAsync(sensorId, QueryType.ActiveOnly))
                .ReturnsAsync(sensor);
            _logRepoMock
                .Setup(r =>
                    r.GetPagedAsync(
                        It.IsAny<ISpecification<SensorLog, SensorLogDto>>(),
                        It.IsAny<int>(),
                        It.IsAny<int>(),
                        QueryType.ActiveOnly
                    )
                )
                .ReturnsAsync(
                    new PagedResult<SensorLogDto>
                    {
                        Items = new List<SensorLogDto>(),
                        TotalItems = 0,
                    }
                );

            // Act
            var result = await _sut.GetSensorLogsAsync(sensorId, request);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Data!.Data.Should().BeEmpty();
            result.Data.Message.Should().Be("Không có dữ liệu lịch sử");
        }

        [Fact]
        public async Task GetSensorLogsAsync_ShouldReturnNotFound_WhenSensorDoesNotExist()
        {
            // Arrange
            var sensorId = Guid.NewGuid();
            _sensorRepoMock
                .Setup(r => r.GetByIdAsync(sensorId, QueryType.ActiveOnly))
                .ReturnsAsync((Sensor?)null);

            // Act
            var result = await _sut.GetSensorLogsAsync(sensorId, new SensorLogListRequest());

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Type.Should().Be(ResultType.NotFound);
            _logRepoMock.Verify(
                r =>
                    r.GetPagedAsync(
                        It.IsAny<ISpecification<SensorLog, SensorLogDto>>(),
                        It.IsAny<int>(),
                        It.IsAny<int>(),
                        It.IsAny<QueryType>()
                    ),
                Times.Never
            );
        }

        [Fact]
        public async Task GetSensorLogsAsync_ShouldReturnUnexpected_WhenExceptionThrown()
        {
            // Arrange
            var sensorId = Guid.NewGuid();
            _sensorRepoMock
                .Setup(r => r.GetByIdAsync(sensorId, QueryType.ActiveOnly))
                .ThrowsAsync(new Exception());

            // Act
            var result = await _sut.GetSensorLogsAsync(sensorId, new SensorLogListRequest());

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Type.Should().Be(ResultType.Unexpected);
        }

        #endregion
    }
}

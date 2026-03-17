using System.Linq.Expressions;
using Ardalis.Specification;
using FluentAssertions;
using IRasRag.Application.Common.Interfaces.Persistence;
using IRasRag.Application.Common.Interfaces.Persistence.Repositories;
using IRasRag.Application.Common.Models;
using IRasRag.Application.DTOs;
using IRasRag.Application.Services.Implementations;
using IRasRag.Domain.Entities;
using IRasRag.Domain.Enums;
using Microsoft.Extensions.Logging;
using Moq;

namespace IRasRag.Test.UnitTests.Application
{
    public class AnalyticsServiceTest
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<ILogger<AnalyticsService>> _loggerMock;

        // Repositories
        private readonly Mock<IRepository<FarmingBatch>> _batchRepoMock;
        private readonly Mock<IRepository<FishTank>> _fishTankRepoMock;
        private readonly Mock<IRepository<Alert>> _alertRepoMock;
        private readonly Mock<IRepository<MortalityLog>> _mortalityLogRepoMock;
        private readonly Mock<IRepository<FeedingLog>> _feedingLogRepoMock;
        private readonly Mock<IRepository<MasterBoard>> _masterBoardRepoMock;
        private readonly Mock<IRepository<Sensor>> _sensorRepoMock;
        private readonly Mock<IRepository<SensorLog>> _sensorLogRepoMock;
        private readonly Mock<IRepository<SensorType>> _sensorTypeRepoMock;
        private readonly Mock<IRepository<Farm>> _farmRepoMock;
        private readonly Mock<IRepository<UserFarm>> _userFarmRepoMock;

        private readonly AnalyticsService _sut;

        public AnalyticsServiceTest()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _loggerMock = new Mock<ILogger<AnalyticsService>>();

            _batchRepoMock = new Mock<IRepository<FarmingBatch>>();
            _fishTankRepoMock = new Mock<IRepository<FishTank>>();
            _alertRepoMock = new Mock<IRepository<Alert>>();
            _mortalityLogRepoMock = new Mock<IRepository<MortalityLog>>();
            _feedingLogRepoMock = new Mock<IRepository<FeedingLog>>();
            _masterBoardRepoMock = new Mock<IRepository<MasterBoard>>();
            _sensorRepoMock = new Mock<IRepository<Sensor>>();
            _sensorLogRepoMock = new Mock<IRepository<SensorLog>>();
            _sensorTypeRepoMock = new Mock<IRepository<SensorType>>();
            _farmRepoMock = new Mock<IRepository<Farm>>();
            _userFarmRepoMock = new Mock<IRepository<UserFarm>>();

            _unitOfWorkMock
                .Setup(u => u.GetRepository<FarmingBatch>())
                .Returns(_batchRepoMock.Object);
            _unitOfWorkMock
                .Setup(u => u.GetRepository<FishTank>())
                .Returns(_fishTankRepoMock.Object);
            _unitOfWorkMock.Setup(u => u.GetRepository<Alert>()).Returns(_alertRepoMock.Object);
            _unitOfWorkMock
                .Setup(u => u.GetRepository<MortalityLog>())
                .Returns(_mortalityLogRepoMock.Object);
            _unitOfWorkMock
                .Setup(u => u.GetRepository<FeedingLog>())
                .Returns(_feedingLogRepoMock.Object);
            _unitOfWorkMock
                .Setup(u => u.GetRepository<MasterBoard>())
                .Returns(_masterBoardRepoMock.Object);
            _unitOfWorkMock.Setup(u => u.GetRepository<Sensor>()).Returns(_sensorRepoMock.Object);
            _unitOfWorkMock
                .Setup(u => u.GetRepository<SensorLog>())
                .Returns(_sensorLogRepoMock.Object);
            _unitOfWorkMock
                .Setup(u => u.GetRepository<SensorType>())
                .Returns(_sensorTypeRepoMock.Object);
            _unitOfWorkMock.Setup(u => u.GetRepository<Farm>()).Returns(_farmRepoMock.Object);
            _unitOfWorkMock
                .Setup(u => u.GetRepository<UserFarm>())
                .Returns(_userFarmRepoMock.Object);

            // ── Default setups so GetUserTankIdsAsync never throws ────────────
            // UserFarm returns one record so farmIds is non-empty and FishTank query is reached.
            _userFarmRepoMock
                .Setup(r =>
                    r.FindAllAsync(
                        It.IsAny<Expression<Func<UserFarm, bool>>>(),
                        It.IsAny<QueryType>()
                    )
                )
                .ReturnsAsync(
                    (IReadOnlyList<UserFarm>)new List<UserFarm>
                    {
                        new() { UserId = Guid.Empty, FarmId = Guid.NewGuid() },
                    }
                );

            // FishTank returns an empty list by default; individual tests override this as needed.
            // When overridden, the returned tank IDs become userTankIds via GetUserTankIdsAsync.
            _fishTankRepoMock
                .Setup(r =>
                    r.FindAllAsync(
                        It.IsAny<Expression<Func<FishTank, bool>>>(),
                        It.IsAny<QueryType>()
                    )
                )
                .ReturnsAsync((IReadOnlyList<FishTank>)new List<FishTank>());

            _sut = new AnalyticsService(_unitOfWorkMock.Object, _loggerMock.Object);
        }

        // ─── Helpers ──────────────────────────────────────────────────────────

        private void SetupDefaultSensorTypes(IEnumerable<SensorType>? types = null)
        {
            _sensorTypeRepoMock
                .Setup(r => r.GetAllAsync(It.IsAny<QueryType>()))
                .ReturnsAsync((IReadOnlyList<SensorType>)(types?.ToList() ?? new List<SensorType>()));
        }

        private void SetupDefaultAlertRepo()
        {
            _alertRepoMock
                .Setup(r =>
                    r.FindAllAsync(It.IsAny<Expression<Func<Alert, bool>>>(), It.IsAny<QueryType>())
                )
                .ReturnsAsync((IReadOnlyList<Alert>)new List<Alert>());
        }

        private void SetupDefaultMasterBoardRepo()
        {
            _masterBoardRepoMock
                .Setup(r =>
                    r.FindAllAsync(
                        It.IsAny<Expression<Func<MasterBoard, bool>>>(),
                        It.IsAny<QueryType>()
                    )
                )
                .ReturnsAsync((IReadOnlyList<MasterBoard>)new List<MasterBoard>());
        }

        private FarmingBatch MakeBatch(
            Guid batchId,
            Guid fishTankId,
            int initial = 1000,
            int current = 900,
            DateTime? startDate = null
        ) =>
            new()
            {
                Id = batchId,
                Name = $"Batch-{batchId}",
                FishTankId = fishTankId,
                CurrentStageConfigId = Guid.NewGuid(),
                Status = FarmingBatchStatus.ACTIVE,
                StartDate = startDate ?? DateTime.UtcNow.AddMonths(-1),
                InitialQuantity = initial,
                CurrentQuantity = current,
                UnitOfMeasure = "fish",
            };

        private FishTank MakeFishTank(Guid tankId) =>
            new()
            {
                Id = tankId,
                Name = $"Tank-{tankId}",
                FarmId = Guid.NewGuid(),
                Height = 2,
                Radius = 1,
            };

        // ─────────────────────────────────────────────────────────────────────
        // CompareBatchesAsync
        // ─────────────────────────────────────────────────────────────────────

        #region CompareBatchesAsync – Input Validation

        [Fact]
        public async Task CompareBatchesAsync_ShouldReturnBadRequest_WhenBatchIdsIsNull()
        {
            var request = new BatchCompareRequest { BatchIds = null! };

            var result = await _sut.CompareBatchesAsync(request);

            result.Type.Should().Be(ResultType.BadRequest);
            result.Message.Should().Be("Vui lòng cung cấp ít nhất hai lô nuôi để so sánh.");
        }

        [Fact]
        public async Task CompareBatchesAsync_ShouldReturnBadRequest_WhenBatchIdsIsEmpty()
        {
            var request = new BatchCompareRequest { BatchIds = new List<Guid>() };

            var result = await _sut.CompareBatchesAsync(request);

            result.Type.Should().Be(ResultType.BadRequest);
            result.Message.Should().Be("Vui lòng cung cấp ít nhất hai lô nuôi để so sánh.");
        }

        [Fact]
        public async Task CompareBatchesAsync_ShouldReturnBadRequest_WhenOnlyOneBatchIdProvided()
        {
            var request = new BatchCompareRequest { BatchIds = new List<Guid> { Guid.NewGuid() } };

            var result = await _sut.CompareBatchesAsync(request);

            result.Type.Should().Be(ResultType.BadRequest);
            result.Message.Should().Be("Cần ít nhất hai lô nuôi khác nhau để thực hiện so sánh.");
        }

        [Fact]
        public async Task CompareBatchesAsync_ShouldReturnBadRequest_WhenBatchIdsAreAllDuplicates()
        {
            var sameId = Guid.NewGuid();
            var request = new BatchCompareRequest
            {
                BatchIds = new List<Guid> { sameId, sameId, sameId },
            };

            var result = await _sut.CompareBatchesAsync(request);

            result.Type.Should().Be(ResultType.BadRequest);
            result.Message.Should().Be("Cần ít nhất hai lô nuôi khác nhau để thực hiện so sánh.");
        }

        [Fact]
        public async Task CompareBatchesAsync_ShouldReturnBadRequest_WhenMoreThan10BatchIdsProvided()
        {
            var request = new BatchCompareRequest
            {
                BatchIds = Enumerable.Range(0, 11).Select(_ => Guid.NewGuid()).ToList(),
            };

            var result = await _sut.CompareBatchesAsync(request);

            result.Type.Should().Be(ResultType.BadRequest);
            result.Message.Should().Be("Tối đa 10 lô nuôi có thể được so sánh trong một lần.");
        }

        #endregion

        #region CompareBatchesAsync – Data Retrieval

        [Fact]
        public async Task CompareBatchesAsync_ShouldReturnNotFound_WhenNoBatchesExistInDatabase()
        {
            var request = new BatchCompareRequest
            {
                BatchIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() },
            };

            SetupDefaultSensorTypes();
            _batchRepoMock
                .Setup(r =>
                    r.FindAllAsync(
                        It.IsAny<Expression<Func<FarmingBatch, bool>>>(),
                        It.IsAny<QueryType>()
                    )
                )
                .ReturnsAsync(new List<FarmingBatch>());
            _fishTankRepoMock
                .Setup(r =>
                    r.FindAllAsync(
                        It.IsAny<Expression<Func<FishTank, bool>>>(),
                        It.IsAny<QueryType>()
                    )
                )
                .ReturnsAsync(new List<FishTank>());

            var result = await _sut.CompareBatchesAsync(request);

            result.Type.Should().Be(ResultType.NotFound);
            result
                .Message.Should()
                .Be("Không tìm thấy lô nuôi nào hợp lệ trong danh sách đã cung cấp.");
        }

        [Fact]
        public async Task CompareBatchesAsync_ShouldSkipMissing_AndReturnNotFound_WhenAllBatchesMissing()
        {
            var id1 = Guid.NewGuid();
            var id2 = Guid.NewGuid();
            var request = new BatchCompareRequest
            {
                BatchIds = new List<Guid> { id1, id2 },
                Metrics = new List<string> { "survival_rate" },
            };

            SetupDefaultSensorTypes();
            // Return only one batch → the other is skipped; but since only 1 valid batch is found,
            // FindAllAsync still returns it. But wait – if FindAllAsync returns only 1 batch,
            // the foreach loop will only process id1 (or id2 depending on which was returned),
            // so only 1 entry is added to batchResults. The result should still be Ok.
            // Here we return NONE to trigger NotFound.
            _batchRepoMock
                .Setup(r =>
                    r.FindAllAsync(
                        It.IsAny<Expression<Func<FarmingBatch, bool>>>(),
                        It.IsAny<QueryType>()
                    )
                )
                .ReturnsAsync(new List<FarmingBatch>());
            _fishTankRepoMock
                .Setup(r =>
                    r.FindAllAsync(
                        It.IsAny<Expression<Func<FishTank, bool>>>(),
                        It.IsAny<QueryType>()
                    )
                )
                .ReturnsAsync(new List<FishTank>());

            var result = await _sut.CompareBatchesAsync(request);

            result.Type.Should().Be(ResultType.NotFound);
        }

        #endregion

        #region CompareBatchesAsync – Metric Computation

        [Fact]
        public async Task CompareBatchesAsync_ShouldReturnOk_WithAllMetrics()
        {
            var batchId1 = Guid.NewGuid();
            var batchId2 = Guid.NewGuid();
            var tankId1 = Guid.NewGuid();
            var tankId2 = Guid.NewGuid();

            var request = new BatchCompareRequest
            {
                BatchIds = new List<Guid> { batchId1, batchId2 },
                Metrics = new List<string>(), // request all metrics
            };

            var batches = new List<FarmingBatch>
            {
                MakeBatch(batchId1, tankId1, initial: 1000, current: 850),
                MakeBatch(batchId2, tankId2, initial: 500, current: 400),
            };

            var fishTanks = new List<FishTank> { MakeFishTank(tankId1), MakeFishTank(tankId2) };

            SetupDefaultSensorTypes();

            _batchRepoMock
                .Setup(r =>
                    r.FindAllAsync(
                        It.IsAny<Expression<Func<FarmingBatch, bool>>>(),
                        It.IsAny<QueryType>()
                    )
                )
                .ReturnsAsync(batches);
            _fishTankRepoMock
                .Setup(r =>
                    r.FindAllAsync(
                        It.IsAny<Expression<Func<FishTank, bool>>>(),
                        It.IsAny<QueryType>()
                    )
                )
                .ReturnsAsync(fishTanks);
            _mortalityLogRepoMock
                .Setup(r =>
                    r.FindAllAsync(
                        It.IsAny<Expression<Func<MortalityLog, bool>>>(),
                        It.IsAny<QueryType>()
                    )
                )
                .ReturnsAsync(new List<MortalityLog>());
            _feedingLogRepoMock
                .Setup(r =>
                    r.FindAllAsync(
                        It.IsAny<Expression<Func<FeedingLog, bool>>>(),
                        It.IsAny<QueryType>()
                    )
                )
                .ReturnsAsync(new List<FeedingLog>());
            SetupDefaultAlertRepo();
            SetupDefaultMasterBoardRepo();

            var result = await _sut.CompareBatchesAsync(request);

            result.Type.Should().Be(ResultType.Ok);
            result.IsSuccess.Should().BeTrue();
            result.Data.Should().NotBeNull();
            result.Data!.Batches.Should().HaveCount(2);
            result
                .Data.EvaluatedMetrics.Should()
                .Contain(new[] { "survival_rate", "mortality", "feeding", "alerts" });
        }

        [Fact]
        public async Task CompareBatchesAsync_ShouldComputeCorrectSurvivalRate()
        {
            var batchId1 = Guid.NewGuid();
            var batchId2 = Guid.NewGuid();
            var tankId1 = Guid.NewGuid();
            var tankId2 = Guid.NewGuid();

            var request = new BatchCompareRequest
            {
                BatchIds = new List<Guid> { batchId1, batchId2 },
                Metrics = new List<string> { "survival_rate" },
            };

            var batches = new List<FarmingBatch>
            {
                MakeBatch(batchId1, tankId1, initial: 1000, current: 800),
                MakeBatch(batchId2, tankId2, initial: 200, current: 100),
            };

            SetupDefaultSensorTypes();
            _batchRepoMock
                .Setup(r =>
                    r.FindAllAsync(
                        It.IsAny<Expression<Func<FarmingBatch, bool>>>(),
                        It.IsAny<QueryType>()
                    )
                )
                .ReturnsAsync(batches);
            _fishTankRepoMock
                .Setup(r =>
                    r.FindAllAsync(
                        It.IsAny<Expression<Func<FishTank, bool>>>(),
                        It.IsAny<QueryType>()
                    )
                )
                .ReturnsAsync(new List<FishTank> { MakeFishTank(tankId1), MakeFishTank(tankId2) });

            var result = await _sut.CompareBatchesAsync(request);

            result.Type.Should().Be(ResultType.Ok);
            var b1 = result.Data!.Batches.First(b => b.BatchId == batchId1);
            var b2 = result.Data!.Batches.First(b => b.BatchId == batchId2);
            b1.MetricValues.SurvivalRate.Should().Be(80.0); // 800/1000 * 100
            b2.MetricValues.SurvivalRate.Should().Be(50.0); // 100/200 * 100
        }

        [Fact]
        public async Task CompareBatchesAsync_ShouldReturnZeroSurvivalRate_WhenInitialQuantityIsZero()
        {
            var batchId1 = Guid.NewGuid();
            var batchId2 = Guid.NewGuid();
            var tankId = Guid.NewGuid();

            var request = new BatchCompareRequest
            {
                BatchIds = new List<Guid> { batchId1, batchId2 },
                Metrics = new List<string> { "survival_rate" },
            };

            var batches = new List<FarmingBatch>
            {
                MakeBatch(batchId1, tankId, initial: 0, current: 0),
                MakeBatch(batchId2, tankId, initial: 100, current: 60),
            };

            SetupDefaultSensorTypes();
            _batchRepoMock
                .Setup(r =>
                    r.FindAllAsync(
                        It.IsAny<Expression<Func<FarmingBatch, bool>>>(),
                        It.IsAny<QueryType>()
                    )
                )
                .ReturnsAsync(batches);
            _fishTankRepoMock
                .Setup(r =>
                    r.FindAllAsync(
                        It.IsAny<Expression<Func<FishTank, bool>>>(),
                        It.IsAny<QueryType>()
                    )
                )
                .ReturnsAsync(new List<FishTank> { MakeFishTank(tankId) });

            var result = await _sut.CompareBatchesAsync(request);

            result.Type.Should().Be(ResultType.Ok);
            result
                .Data!.Batches.First(b => b.BatchId == batchId1)
                .MetricValues.SurvivalRate.Should()
                .Be(0d);
        }

        [Fact]
        public async Task CompareBatchesAsync_ShouldComputeTotalMortality_FromMortalityLogs()
        {
            var batchId1 = Guid.NewGuid();
            var batchId2 = Guid.NewGuid();
            var tankId1 = Guid.NewGuid();
            var tankId2 = Guid.NewGuid();

            var request = new BatchCompareRequest
            {
                BatchIds = new List<Guid> { batchId1, batchId2 },
                Metrics = new List<string> { "mortality" },
            };

            var batches = new List<FarmingBatch>
            {
                MakeBatch(batchId1, tankId1),
                MakeBatch(batchId2, tankId2),
            };

            SetupDefaultSensorTypes();
            _batchRepoMock
                .Setup(r =>
                    r.FindAllAsync(
                        It.IsAny<Expression<Func<FarmingBatch, bool>>>(),
                        It.IsAny<QueryType>()
                    )
                )
                .ReturnsAsync(batches);
            _fishTankRepoMock
                .Setup(r =>
                    r.FindAllAsync(
                        It.IsAny<Expression<Func<FishTank, bool>>>(),
                        It.IsAny<QueryType>()
                    )
                )
                .ReturnsAsync(new List<FishTank> { MakeFishTank(tankId1), MakeFishTank(tankId2) });
            _mortalityLogRepoMock
                .Setup(r =>
                    r.FindAllAsync(
                        It.IsAny<Expression<Func<MortalityLog, bool>>>(),
                        It.IsAny<QueryType>()
                    )
                )
                .ReturnsAsync(
                    new List<MortalityLog>
                    {
                        new()
                        {
                            BatchId = batchId1,
                            Quantity = 30,
                            Date = DateTime.UtcNow,
                        },
                        new()
                        {
                            BatchId = batchId1,
                            Quantity = 20,
                            Date = DateTime.UtcNow,
                        },
                    }
                );

            var result = await _sut.CompareBatchesAsync(request);

            result.Type.Should().Be(ResultType.Ok);
            // Both batches get the same mortality logs list (mock returns same list for any predicate)
            result
                .Data!.Batches.Should()
                .AllSatisfy(b => b.MetricValues.TotalMortality.Should().Be(50d));
        }

        [Fact]
        public async Task CompareBatchesAsync_ShouldComputeTotalFeeding_FromFeedingLogs()
        {
            var batchId1 = Guid.NewGuid();
            var batchId2 = Guid.NewGuid();
            var tankId = Guid.NewGuid();

            var request = new BatchCompareRequest
            {
                BatchIds = new List<Guid> { batchId1, batchId2 },
                Metrics = new List<string> { "feeding" },
            };

            var batches = new List<FarmingBatch>
            {
                MakeBatch(batchId1, tankId),
                MakeBatch(batchId2, tankId),
            };

            SetupDefaultSensorTypes();
            _batchRepoMock
                .Setup(r =>
                    r.FindAllAsync(
                        It.IsAny<Expression<Func<FarmingBatch, bool>>>(),
                        It.IsAny<QueryType>()
                    )
                )
                .ReturnsAsync(batches);
            _fishTankRepoMock
                .Setup(r =>
                    r.FindAllAsync(
                        It.IsAny<Expression<Func<FishTank, bool>>>(),
                        It.IsAny<QueryType>()
                    )
                )
                .ReturnsAsync(new List<FishTank> { MakeFishTank(tankId) });
            _feedingLogRepoMock
                .Setup(r =>
                    r.FindAllAsync(
                        It.IsAny<Expression<Func<FeedingLog, bool>>>(),
                        It.IsAny<QueryType>()
                    )
                )
                .ReturnsAsync(
                    new List<FeedingLog>
                    {
                        new() { FarmingBatchId = batchId1, Amount = 150.5f },
                        new() { FarmingBatchId = batchId1, Amount = 49.5f },
                    }
                );

            var result = await _sut.CompareBatchesAsync(request);

            result.Type.Should().Be(ResultType.Ok);
            result
                .Data!.Batches.Should()
                .AllSatisfy(b => b.MetricValues.TotalFeeding.Should().Be(200.0));
        }

        [Fact]
        public async Task CompareBatchesAsync_ShouldComputeAlertsByType_WhenAlertsExist()
        {
            var batchId1 = Guid.NewGuid();
            var batchId2 = Guid.NewGuid();
            var tankId = Guid.NewGuid();
            var sensorTypeId = Guid.NewGuid();
            var startDate = DateTime.UtcNow.AddDays(-10);

            var request = new BatchCompareRequest
            {
                BatchIds = new List<Guid> { batchId1, batchId2 },
                Metrics = new List<string> { "alerts" },
            };

            var sensorType = new SensorType
            {
                Id = sensorTypeId,
                Name = "DO",
                MeasureType = "do",
                UnitOfMeasure = "mg/L",
            };

            var batches = new List<FarmingBatch>
            {
                MakeBatch(batchId1, tankId, startDate: startDate),
                MakeBatch(batchId2, tankId, startDate: startDate),
            };

            var alerts = new List<Alert>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    FishTankId = tankId,
                    SensorTypeId = sensorTypeId,
                    RaisedAt = DateTime.UtcNow.AddDays(-3),
                    Value = 4.5f,
                    SensorLogId = Guid.NewGuid(),
                    SpeciesThresholdId = Guid.NewGuid(),
                    Status = AlertStatus.OPEN,
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    FishTankId = tankId,
                    SensorTypeId = sensorTypeId,
                    RaisedAt = DateTime.UtcNow.AddDays(-2),
                    Value = 4.0f,
                    SensorLogId = Guid.NewGuid(),
                    SpeciesThresholdId = Guid.NewGuid(),
                    Status = AlertStatus.RESOLVED,
                },
            };

            SetupDefaultSensorTypes(new List<SensorType> { sensorType });
            _batchRepoMock
                .Setup(r =>
                    r.FindAllAsync(
                        It.IsAny<Expression<Func<FarmingBatch, bool>>>(),
                        It.IsAny<QueryType>()
                    )
                )
                .ReturnsAsync(batches);
            _fishTankRepoMock
                .Setup(r =>
                    r.FindAllAsync(
                        It.IsAny<Expression<Func<FishTank, bool>>>(),
                        It.IsAny<QueryType>()
                    )
                )
                .ReturnsAsync(new List<FishTank> { MakeFishTank(tankId) });
            _alertRepoMock
                .Setup(r =>
                    r.FindAllAsync(It.IsAny<Expression<Func<Alert, bool>>>(), It.IsAny<QueryType>())
                )
                .ReturnsAsync(alerts);

            var result = await _sut.CompareBatchesAsync(request);

            result.Type.Should().Be(ResultType.Ok);
            var firstBatch = result.Data!.Batches.First();
            firstBatch.MetricValues.TotalAlerts.Should().Be(2);
            firstBatch.MetricValues.AlertsByType.Should().ContainKey("DO");
            firstBatch.MetricValues.AlertsByType["DO"].Should().Be(2);
        }

        [Fact]
        public async Task CompareBatchesAsync_ShouldComputeSensorAverages_WhenSensorLogsExist()
        {
            var batchId1 = Guid.NewGuid();
            var batchId2 = Guid.NewGuid();
            var tankId = Guid.NewGuid();
            var sensorTypeId = Guid.NewGuid();
            var masterBoardId = Guid.NewGuid();
            var sensorId = Guid.NewGuid();
            var startDate = DateTime.UtcNow.AddDays(-10);

            var request = new BatchCompareRequest
            {
                BatchIds = new List<Guid> { batchId1, batchId2 },
                Metrics = new List<string> { "DO" },
            };

            var sensorTypes = new List<SensorType>
            {
                new()
                {
                    Id = sensorTypeId,
                    Name = "DO",
                    MeasureType = "do",
                    UnitOfMeasure = "mg/L",
                },
            };

            var batches = new List<FarmingBatch>
            {
                MakeBatch(batchId1, tankId, startDate: startDate),
                MakeBatch(batchId2, tankId, startDate: startDate),
            };

            SetupDefaultSensorTypes(sensorTypes);
            _batchRepoMock
                .Setup(r =>
                    r.FindAllAsync(
                        It.IsAny<Expression<Func<FarmingBatch, bool>>>(),
                        It.IsAny<QueryType>()
                    )
                )
                .ReturnsAsync(batches);
            _fishTankRepoMock
                .Setup(r =>
                    r.FindAllAsync(
                        It.IsAny<Expression<Func<FishTank, bool>>>(),
                        It.IsAny<QueryType>()
                    )
                )
                .ReturnsAsync(new List<FishTank> { MakeFishTank(tankId) });
            _masterBoardRepoMock
                .Setup(r =>
                    r.FindAllAsync(
                        It.IsAny<Expression<Func<MasterBoard, bool>>>(),
                        It.IsAny<QueryType>()
                    )
                )
                .ReturnsAsync(
                    new List<MasterBoard>
                    {
                        new()
                        {
                            Id = masterBoardId,
                            FishTankId = tankId,
                            Name = "MB1",
                            MacAddress = "AA:BB:CC:DD:EE:FF",
                        },
                    }
                );
            _sensorRepoMock
                .Setup(r =>
                    r.FindAllAsync(
                        It.IsAny<Expression<Func<Sensor, bool>>>(),
                        It.IsAny<QueryType>()
                    )
                )
                .ReturnsAsync(
                    new List<Sensor>
                    {
                        new()
                        {
                            Id = sensorId,
                            Name = "DO Sensor",
                            PinCode = 1,
                            MasterBoardId = masterBoardId,
                            SensorTypeId = sensorTypeId,
                        },
                    }
                );
            _sensorLogRepoMock
                .Setup(r =>
                    r.FindAllAsync(
                        It.IsAny<Expression<Func<SensorLog, bool>>>(),
                        It.IsAny<QueryType>()
                    )
                )
                .ReturnsAsync(
                    new List<SensorLog>
                    {
                        new()
                        {
                            SensorId = sensorId,
                            Data = 7.0,
                            IsWarning = false,
                            CreatedAt = startDate.AddDays(1),
                        },
                        new()
                        {
                            SensorId = sensorId,
                            Data = 5.0,
                            IsWarning = false,
                            CreatedAt = startDate.AddDays(2),
                        },
                    }
                );

            var result = await _sut.CompareBatchesAsync(request);

            result.Type.Should().Be(ResultType.Ok);
            result.Data!.EvaluatedMetrics.Should().Contain("DO");

            var firstBatch = result.Data.Batches.First();
            firstBatch.MetricValues.SensorAverages.Should().ContainKey("DO");
            firstBatch.MetricValues.SensorAverages["DO"].Should().Be(6.0); // (7.0 + 5.0) / 2
        }

        [Fact]
        public async Task CompareBatchesAsync_ShouldReturnOk_WithOnlyOneBatchFound_WhenOtherIsMissing()
        {
            var existingBatchId = Guid.NewGuid();
            var missingBatchId = Guid.NewGuid();
            var tankId = Guid.NewGuid();

            var request = new BatchCompareRequest
            {
                BatchIds = new List<Guid> { existingBatchId, missingBatchId },
                Metrics = new List<string> { "survival_rate" },
            };

            SetupDefaultSensorTypes();
            _batchRepoMock
                .Setup(r =>
                    r.FindAllAsync(
                        It.IsAny<Expression<Func<FarmingBatch, bool>>>(),
                        It.IsAny<QueryType>()
                    )
                )
                .ReturnsAsync(new List<FarmingBatch> { MakeBatch(existingBatchId, tankId) });
            _fishTankRepoMock
                .Setup(r =>
                    r.FindAllAsync(
                        It.IsAny<Expression<Func<FishTank, bool>>>(),
                        It.IsAny<QueryType>()
                    )
                )
                .ReturnsAsync(new List<FishTank> { MakeFishTank(tankId) });

            var result = await _sut.CompareBatchesAsync(request);

            // Only one batch was found, so batchResults has 1 item → still Ok
            result.Type.Should().Be(ResultType.Ok);
            result.Data!.Batches.Should().HaveCount(1);
            result.Data.Batches.Single().BatchId.Should().Be(existingBatchId);
        }

        [Fact]
        public async Task CompareBatchesAsync_ShouldReturnUnexpected_WhenExceptionIsThrown()
        {
            var request = new BatchCompareRequest
            {
                BatchIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() },
            };

            _sensorTypeRepoMock
                .Setup(r => r.GetAllAsync(It.IsAny<QueryType>()))
                .ThrowsAsync(new Exception("DB failure"));

            var result = await _sut.CompareBatchesAsync(request);

            result.Type.Should().Be(ResultType.Unexpected);
            result.IsSuccess.Should().BeFalse();
            result.Message.Should().Be("Có lỗi xảy ra khi so sánh lô nuôi, vui lòng thử lại sau.");
        }

        #endregion

        // ─────────────────────────────────────────────────────────────────────
        // GetAlertFrequencyAsync
        // ─────────────────────────────────────────────────────────────────────

        #region GetAlertFrequencyAsync – Input Validation

        [Fact]
        public async Task GetAlertFrequencyAsync_ShouldReturnBadRequest_WhenFromIsAfterTo()
        {
            var request = new AlertFrequencyRequest
            {
                From = DateTime.UtcNow,
                To = DateTime.UtcNow.AddDays(-1),
            };

            var result = await _sut.GetAlertFrequencyAsync(request);

            result.Type.Should().Be(ResultType.BadRequest);
            result.Message.Should().Be("Thời điểm bắt đầu phải trước thời điểm kết thúc.");
        }

        [Fact]
        public async Task GetAlertFrequencyAsync_ShouldReturnBadRequest_WhenFromEqualsTo()
        {
            var now = DateTime.UtcNow;
            var request = new AlertFrequencyRequest { From = now, To = now };

            var result = await _sut.GetAlertFrequencyAsync(request);

            result.Type.Should().Be(ResultType.BadRequest);
            result.Message.Should().Be("Thời điểm bắt đầu phải trước thời điểm kết thúc.");
        }

        [Fact]
        public async Task GetAlertFrequencyAsync_ShouldReturnBadRequest_WhenRangeExceeds365Days()
        {
            var request = new AlertFrequencyRequest
            {
                From = DateTime.UtcNow.AddDays(-400),
                To = DateTime.UtcNow,
            };

            var result = await _sut.GetAlertFrequencyAsync(request);

            result.Type.Should().Be(ResultType.BadRequest);
            result.Message.Should().Be("Khoảng thời gian thống kê tối đa là 365 ngày.");
        }

        #endregion

        #region GetAlertFrequencyAsync – Filter Validation

        [Fact]
        public async Task GetAlertFrequencyAsync_ShouldReturnUnauthorized_WhenFishTankIdNotOwnedByUser()
        {
            var request = new AlertFrequencyRequest
            {
                From = DateTime.UtcNow.AddDays(-7),
                To = DateTime.UtcNow,
                FishTankId = Guid.NewGuid(), // not in userTankIds (FishTank default returns empty)
            };

            var result = await _sut.GetAlertFrequencyAsync(request);

            result.Type.Should().Be(ResultType.Unauthorized);
            result.Message.Should().Be("Bạn không có quyền truy cập bể nuôi với ID đã cung cấp.");
        }

        [Fact]
        public async Task GetAlertFrequencyAsync_ShouldReturnUnauthorized_WhenFarmIdNotOwnedByUser()
        {
            var request = new AlertFrequencyRequest
            {
                From = DateTime.UtcNow.AddDays(-7),
                To = DateTime.UtcNow,
                FarmId = Guid.NewGuid(),
            };

            // Override AnyAsync so the FarmId ownership check returns false
            _userFarmRepoMock
                .Setup(r =>
                    r.AnyAsync(It.IsAny<Expression<Func<UserFarm, bool>>>(), It.IsAny<QueryType>())
                )
                .ReturnsAsync(false);

            var result = await _sut.GetAlertFrequencyAsync(request);

            result.Type.Should().Be(ResultType.Unauthorized);
            result
                .Message.Should()
                .Be("Bạn không có quyền truy cập trang trại với ID đã cung cấp.");
        }

        #endregion

        #region GetAlertFrequencyAsync – Response Building

        [Fact]
        public async Task GetAlertFrequencyAsync_ShouldReturnOk_WithNoAlerts()
        {
            var request = new AlertFrequencyRequest
            {
                From = DateTime.UtcNow.AddDays(-7),
                To = DateTime.UtcNow,
                TopN = 10,
            };

            _alertRepoMock
                .Setup(r =>
                    r.ListAsync(
                        It.IsAny<ISpecification<Alert, AlertFrequencyProjection>>(),
                        It.IsAny<QueryType>()
                    )
                )
                .ReturnsAsync((IReadOnlyList<AlertFrequencyProjection>)new List<AlertFrequencyProjection>());
            SetupDefaultSensorTypes();

            var result = await _sut.GetAlertFrequencyAsync(request);

            result.Type.Should().Be(ResultType.Ok);
            result.IsSuccess.Should().BeTrue();
            result.Data.Should().NotBeNull();
            result.Data!.TotalAlerts.Should().Be(0);
            result.Data.ByAlertType.Should().BeEmpty();
            result.Data.DailyTrend.Should().NotBeEmpty(); // filled with zeros for every day in range
        }

        [Fact]
        public async Task GetAlertFrequencyAsync_ShouldReturnOk_WithDefaultDateRange_WhenNoDatesProvided()
        {
            var request = new AlertFrequencyRequest { TopN = 5 };

            _alertRepoMock
                .Setup(r =>
                    r.ListAsync(
                        It.IsAny<ISpecification<Alert, AlertFrequencyProjection>>(),
                        It.IsAny<QueryType>()
                    )
                )
                .ReturnsAsync((IReadOnlyList<AlertFrequencyProjection>)new List<AlertFrequencyProjection>());
            SetupDefaultSensorTypes();

            var result = await _sut.GetAlertFrequencyAsync(request);

            result.Type.Should().Be(ResultType.Ok);
            result.Data!.TotalAlerts.Should().Be(0);
            // Default range is 30 days → daily trend should have ~31 entries
            result.Data.DailyTrend.Should().HaveCountGreaterThanOrEqualTo(30);
        }

        [Fact]
        public async Task GetAlertFrequencyAsync_ShouldBuildCorrectAlertStats()
        {
            var sensorTypeId = Guid.NewGuid();
            var fishTankId = Guid.NewGuid();
            var raisedAt = DateTime.UtcNow.AddDays(-3);
            var resolvedAt = raisedAt.AddMinutes(60);

            var request = new AlertFrequencyRequest
            {
                From = DateTime.UtcNow.AddDays(-7),
                To = DateTime.UtcNow,
                TopN = 10,
            };

            var projections = new List<AlertFrequencyProjection>
            {
                new()
                {
                    SensorTypeId = sensorTypeId,
                    FishTankId = fishTankId,
                    FishTankName = "Tank A",
                    Status = AlertStatus.OPEN,
                    RaisedAt = raisedAt,
                    ResolvedAt = null,
                },
                new()
                {
                    SensorTypeId = sensorTypeId,
                    FishTankId = fishTankId,
                    FishTankName = "Tank A",
                    Status = AlertStatus.ACKNOWLEDGED,
                    RaisedAt = raisedAt,
                    ResolvedAt = null,
                },
                new()
                {
                    SensorTypeId = sensorTypeId,
                    FishTankId = fishTankId,
                    FishTankName = "Tank A",
                    Status = AlertStatus.RESOLVED,
                    RaisedAt = raisedAt,
                    ResolvedAt = resolvedAt,
                },
                new()
                {
                    SensorTypeId = sensorTypeId,
                    FishTankId = fishTankId,
                    FishTankName = "Tank A",
                    Status = AlertStatus.DISMISSED,
                    RaisedAt = raisedAt,
                    ResolvedAt = null,
                },
            };

            var sensorTypes = new List<SensorType>
            {
                new()
                {
                    Id = sensorTypeId,
                    Name = "Temperature",
                    MeasureType = "temperature",
                    UnitOfMeasure = "°C",
                },
            };

            _alertRepoMock
                .Setup(r =>
                    r.ListAsync(
                        It.IsAny<ISpecification<Alert, AlertFrequencyProjection>>(),
                        It.IsAny<QueryType>()
                    )
                )
                .ReturnsAsync((IReadOnlyList<AlertFrequencyProjection>)projections);
            SetupDefaultSensorTypes(sensorTypes);

            var result = await _sut.GetAlertFrequencyAsync(request);

            result.Type.Should().Be(ResultType.Ok);
            result.Data!.TotalAlerts.Should().Be(4);
            result.Data.ByAlertType.Should().HaveCount(1);

            var item = result.Data.ByAlertType.Single();
            item.SensorTypeName.Should().Be("Temperature");
            item.MeasureType.Should().Be("temperature");
            item.UnitOfMeasure.Should().Be("°C");
            item.TotalCount.Should().Be(4);
            item.Percentage.Should().Be(100.0);
            item.OpenCount.Should().Be(1);
            item.AcknowledgedCount.Should().Be(1);
            item.ResolvedCount.Should().Be(1);
            item.DismissedCount.Should().Be(1);
            item.AverageResolutionMinutes.Should().Be(60.0);
        }

        [Fact]
        public async Task GetAlertFrequencyAsync_ShouldCalculateCorrectPercentages_WithMultipleSensorTypes()
        {
            var stId1 = Guid.NewGuid();
            var stId2 = Guid.NewGuid();
            var tankId = Guid.NewGuid();
            var raisedAt = DateTime.UtcNow.AddDays(-2);

            var request = new AlertFrequencyRequest
            {
                From = DateTime.UtcNow.AddDays(-7),
                To = DateTime.UtcNow,
                TopN = 10,
            };

            var projections = new List<AlertFrequencyProjection>
            {
                new()
                {
                    SensorTypeId = stId1,
                    FishTankId = tankId,
                    FishTankName = "T",
                    Status = AlertStatus.OPEN,
                    RaisedAt = raisedAt,
                },
                new()
                {
                    SensorTypeId = stId1,
                    FishTankId = tankId,
                    FishTankName = "T",
                    Status = AlertStatus.OPEN,
                    RaisedAt = raisedAt,
                },
                new()
                {
                    SensorTypeId = stId1,
                    FishTankId = tankId,
                    FishTankName = "T",
                    Status = AlertStatus.OPEN,
                    RaisedAt = raisedAt,
                },
                new()
                {
                    SensorTypeId = stId2,
                    FishTankId = tankId,
                    FishTankName = "T",
                    Status = AlertStatus.OPEN,
                    RaisedAt = raisedAt,
                },
            };

            var sensorTypes = new List<SensorType>
            {
                new()
                {
                    Id = stId1,
                    Name = "DO",
                    MeasureType = "do",
                    UnitOfMeasure = "mg/L",
                },
                new()
                {
                    Id = stId2,
                    Name = "Temperature",
                    MeasureType = "temp",
                    UnitOfMeasure = "°C",
                },
            };

            _alertRepoMock
                .Setup(r =>
                    r.ListAsync(
                        It.IsAny<ISpecification<Alert, AlertFrequencyProjection>>(),
                        It.IsAny<QueryType>()
                    )
                )
                .ReturnsAsync((IReadOnlyList<AlertFrequencyProjection>)projections);
            SetupDefaultSensorTypes(sensorTypes);

            var result = await _sut.GetAlertFrequencyAsync(request);

            result.Type.Should().Be(ResultType.Ok);
            result.Data!.TotalAlerts.Should().Be(4);

            var doItem = result.Data.ByAlertType.First(x => x.SensorTypeName == "DO");
            doItem.TotalCount.Should().Be(3);
            doItem.Percentage.Should().Be(75.0);

            var tempItem = result.Data.ByAlertType.First(x => x.SensorTypeName == "Temperature");
            tempItem.TotalCount.Should().Be(1);
            tempItem.Percentage.Should().Be(25.0);
        }

        [Fact]
        public async Task GetAlertFrequencyAsync_ShouldLimitResults_ByTopN()
        {
            var raisedAt = DateTime.UtcNow.AddDays(-2);
            var fishTankId = Guid.NewGuid();

            // 5 different sensor types each with 1 alert
            var projections = Enumerable
                .Range(0, 5)
                .Select(_ => new AlertFrequencyProjection
                {
                    SensorTypeId = Guid.NewGuid(),
                    FishTankId = fishTankId,
                    FishTankName = "Tank X",
                    Status = AlertStatus.OPEN,
                    RaisedAt = raisedAt,
                })
                .ToList();

            var sensorTypes = projections
                .Select(p => new SensorType
                {
                    Id = p.SensorTypeId,
                    Name = $"Sensor-{p.SensorTypeId}",
                    MeasureType = "type",
                    UnitOfMeasure = "unit",
                })
                .ToList();

            var request = new AlertFrequencyRequest
            {
                From = DateTime.UtcNow.AddDays(-7),
                To = DateTime.UtcNow,
                TopN = 3,
            };

            _alertRepoMock
                .Setup(r =>
                    r.ListAsync(
                        It.IsAny<ISpecification<Alert, AlertFrequencyProjection>>(),
                        It.IsAny<QueryType>()
                    )
                )
                .ReturnsAsync((IReadOnlyList<AlertFrequencyProjection>)projections);
            SetupDefaultSensorTypes(sensorTypes);

            var result = await _sut.GetAlertFrequencyAsync(request);

            result.Type.Should().Be(ResultType.Ok);
            result.Data!.ByAlertType.Should().HaveCount(3);
        }

        [Fact]
        public async Task GetAlertFrequencyAsync_ShouldReturnNullAvgResolutionMinutes_WhenNoResolvedAlerts()
        {
            var stId = Guid.NewGuid();
            var tankId = Guid.NewGuid();
            var raisedAt = DateTime.UtcNow.AddDays(-2);

            var projections = new List<AlertFrequencyProjection>
            {
                new()
                {
                    SensorTypeId = stId,
                    FishTankId = tankId,
                    FishTankName = "T",
                    Status = AlertStatus.OPEN,
                    RaisedAt = raisedAt,
                },
            };

            var request = new AlertFrequencyRequest
            {
                From = DateTime.UtcNow.AddDays(-7),
                To = DateTime.UtcNow,
            };

            _alertRepoMock
                .Setup(r =>
                    r.ListAsync(
                        It.IsAny<ISpecification<Alert, AlertFrequencyProjection>>(),
                        It.IsAny<QueryType>()
                    )
                )
                .ReturnsAsync((IReadOnlyList<AlertFrequencyProjection>)projections);
            SetupDefaultSensorTypes(
                new List<SensorType>
                {
                    new()
                    {
                        Id = stId,
                        Name = "DO",
                        MeasureType = "do",
                        UnitOfMeasure = "mg/L",
                    },
                }
            );

            var result = await _sut.GetAlertFrequencyAsync(request);

            result.Type.Should().Be(ResultType.Ok);
            result.Data!.ByAlertType.Single().AverageResolutionMinutes.Should().BeNull();
        }

        [Fact]
        public async Task GetAlertFrequencyAsync_ShouldPopulateDailyTrend_WithContinuousDates()
        {
            var from = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            var to = new DateTime(2026, 1, 5, 0, 0, 0, DateTimeKind.Utc);

            var request = new AlertFrequencyRequest
            {
                From = from,
                To = to,
                TopN = 10,
            };

            var projections = new List<AlertFrequencyProjection>
            {
                new()
                {
                    SensorTypeId = Guid.NewGuid(),
                    FishTankId = Guid.NewGuid(),
                    FishTankName = "T",
                    Status = AlertStatus.OPEN,
                    RaisedAt = from.AddDays(1),
                },
            };

            _alertRepoMock
                .Setup(r =>
                    r.ListAsync(
                        It.IsAny<ISpecification<Alert, AlertFrequencyProjection>>(),
                        It.IsAny<QueryType>()
                    )
                )
                .ReturnsAsync((IReadOnlyList<AlertFrequencyProjection>)projections);
            SetupDefaultSensorTypes();

            var result = await _sut.GetAlertFrequencyAsync(request);

            result.Type.Should().Be(ResultType.Ok);
            // Jan 1–5 inclusive = 5 days
            result.Data!.DailyTrend.Should().HaveCount(5);
            result.Data.DailyTrend.Select(d => d.Date).Should().BeInAscendingOrder();
            // Day 2 (Jan 2) should have count = 1
            result.Data.DailyTrend.First(d => d.Date == from.AddDays(1).Date).Count.Should().Be(1);
            // Other days should have count = 0
            result
                .Data.DailyTrend.Where(d => d.Date != from.AddDays(1).Date)
                .Should()
                .AllSatisfy(d => d.Count.Should().Be(0));
        }

        [Fact]
        public async Task GetAlertFrequencyAsync_ShouldBuildTopTanks_GroupedByFishTank()
        {
            var stId = Guid.NewGuid();
            var tankId1 = Guid.NewGuid();
            var tankId2 = Guid.NewGuid();
            var raisedAt = DateTime.UtcNow.AddDays(-2);

            var projections = new List<AlertFrequencyProjection>
            {
                new()
                {
                    SensorTypeId = stId,
                    FishTankId = tankId1,
                    FishTankName = "Tank A",
                    Status = AlertStatus.OPEN,
                    RaisedAt = raisedAt,
                },
                new()
                {
                    SensorTypeId = stId,
                    FishTankId = tankId1,
                    FishTankName = "Tank A",
                    Status = AlertStatus.OPEN,
                    RaisedAt = raisedAt,
                },
                new()
                {
                    SensorTypeId = stId,
                    FishTankId = tankId1,
                    FishTankName = "Tank A",
                    Status = AlertStatus.OPEN,
                    RaisedAt = raisedAt,
                },
                new()
                {
                    SensorTypeId = stId,
                    FishTankId = tankId2,
                    FishTankName = "Tank B",
                    Status = AlertStatus.OPEN,
                    RaisedAt = raisedAt,
                },
            };

            var request = new AlertFrequencyRequest
            {
                From = DateTime.UtcNow.AddDays(-7),
                To = DateTime.UtcNow,
                TopN = 10,
            };

            _alertRepoMock
                .Setup(r =>
                    r.ListAsync(
                        It.IsAny<ISpecification<Alert, AlertFrequencyProjection>>(),
                        It.IsAny<QueryType>()
                    )
                )
                .ReturnsAsync((IReadOnlyList<AlertFrequencyProjection>)projections);
            SetupDefaultSensorTypes(
                new List<SensorType>
                {
                    new()
                    {
                        Id = stId,
                        Name = "DO",
                        MeasureType = "do",
                        UnitOfMeasure = "mg/L",
                    },
                }
            );

            var result = await _sut.GetAlertFrequencyAsync(request);

            result.Type.Should().Be(ResultType.Ok);
            var item = result.Data!.ByAlertType.Single();
            item.TopTanks.Should().HaveCount(2);
            item.TopTanks.First().FishTankName.Should().Be("Tank A");
            item.TopTanks.First().Count.Should().Be(3);
            item.TopTanks.Last().FishTankName.Should().Be("Tank B");
            item.TopTanks.Last().Count.Should().Be(1);
        }

        [Fact]
        public async Task GetAlertFrequencyAsync_ShouldReturnUnexpected_WhenExceptionIsThrown()
        {
            var request = new AlertFrequencyRequest
            {
                From = DateTime.UtcNow.AddDays(-7),
                To = DateTime.UtcNow,
            };

            _alertRepoMock
                .Setup(r =>
                    r.ListAsync(
                        It.IsAny<ISpecification<Alert, AlertFrequencyProjection>>(),
                        It.IsAny<QueryType>()
                    )
                )
                .ThrowsAsync(new Exception("Database error"));

            var result = await _sut.GetAlertFrequencyAsync(request);

            result.Type.Should().Be(ResultType.Unexpected);
            result.IsSuccess.Should().BeFalse();
            result
                .Message.Should()
                .Be("Có lỗi xảy ra khi thống kê tần suất cảnh báo, vui lòng thử lại sau.");
        }

        #endregion
    }
}

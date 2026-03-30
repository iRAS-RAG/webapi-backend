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
    public class ReportServiceTest
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<ILogger<ReportService>> _loggerMock;

        // Repositories
        private readonly Mock<IRepository<Alert>> _alertRepoMock;
        private readonly Mock<IRepository<FarmingBatch>> _batchRepoMock;
        private readonly Mock<IRepository<MortalityLog>> _mortalityRepoMock;
        private readonly Mock<IRepository<CorrectiveAction>> _correctiveActionRepoMock;
        private readonly Mock<IRepository<Recommendation>> _recommendationRepoMock;
        private readonly Mock<IRepository<UserFarm>> _userFarmRepoMock;
        private readonly Mock<IRepository<FishTank>> _fishTankRepoMock;

        private readonly ReportService _sut;

        public ReportServiceTest()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _loggerMock = new Mock<ILogger<ReportService>>();

            _alertRepoMock = new Mock<IRepository<Alert>>();
            _batchRepoMock = new Mock<IRepository<FarmingBatch>>();
            _mortalityRepoMock = new Mock<IRepository<MortalityLog>>();
            _correctiveActionRepoMock = new Mock<IRepository<CorrectiveAction>>();
            _recommendationRepoMock = new Mock<IRepository<Recommendation>>();
            _userFarmRepoMock = new Mock<IRepository<UserFarm>>();
            _fishTankRepoMock = new Mock<IRepository<FishTank>>();

            _unitOfWorkMock.Setup(u => u.GetRepository<Alert>()).Returns(_alertRepoMock.Object);
            _unitOfWorkMock
                .Setup(u => u.GetRepository<FarmingBatch>())
                .Returns(_batchRepoMock.Object);
            _unitOfWorkMock
                .Setup(u => u.GetRepository<MortalityLog>())
                .Returns(_mortalityRepoMock.Object);
            _unitOfWorkMock
                .Setup(u => u.GetRepository<CorrectiveAction>())
                .Returns(_correctiveActionRepoMock.Object);
            _unitOfWorkMock
                .Setup(u => u.GetRepository<Recommendation>())
                .Returns(_recommendationRepoMock.Object);
            _unitOfWorkMock
                .Setup(u => u.GetRepository<UserFarm>())
                .Returns(_userFarmRepoMock.Object);
            _unitOfWorkMock
                .Setup(u => u.GetRepository<FishTank>())
                .Returns(_fishTankRepoMock.Object);

            _sut = new ReportService(_unitOfWorkMock.Object, _loggerMock.Object);
        }

        // ─── Factory helpers ────────────────────────────────────────────────────

        private static MortalityLog MakeMortalityLog(DateTime date, int quantity) =>
            new()
            {
                Id = Guid.NewGuid(),
                BatchId = Guid.NewGuid(),
                Quantity = quantity,
                Date = date,
            };

        private static WeeklyCorrectiveActionItem MakeCAItem(DateTime timestamp) =>
            new()
            {
                Id = Guid.NewGuid(),
                AlertId = Guid.NewGuid(),
                ActionTaken = "action taken",
                Notes = "notes",
                PerformedBy = "John Doe",
                Timestamp = timestamp,
            };

        private static WeeklyRecommendationItem MakeRecItem() =>
            new()
            {
                Id = Guid.NewGuid(),
                AlertId = Guid.NewGuid(),
                SuggestionText = "suggestion text",
                DocumentTitle = "document title",
            };

        // ─── Default mock setups ─────────────────────────────────────────────────

        /// <summary>
        /// Configures all repositories used by GetDashboardSummaryAsync.
        /// The 5 alert CountAsync calls are sequenced in this order:
        ///   totalAlerts → openAlerts → acknowledgedAlerts → resolvedAlerts → dismissedAlerts.
        /// The 3 batch CountAsync calls are sequenced:
        ///   activeBatches → harvestedBatches → totalBatches.
        /// </summary>
        private void SetupDashboardDefaults(
            int totalAlerts = 0,
            int openAlerts = 0,
            int acknowledgedAlerts = 0,
            int resolvedAlerts = 0,
            int dismissedAlerts = 0,
            int activeBatches = 0,
            int harvestedBatches = 0,
            int totalBatches = 0,
            IEnumerable<BatchSurvivalProjection>? survivalData = null,
            IEnumerable<MortalityLog>? mortalityLogs = null
        )
        {
            // ── Scope: return one dummy UserFarm → one dummy FishTank so the service
            //    proceeds past the early-exit guard (tankIds.Count == 0).
            var dummyFarmId = Guid.NewGuid();
            var dummyTankId = Guid.NewGuid();

            _userFarmRepoMock
                .Setup(r =>
                    r.FindAllAsync(
                        It.IsAny<Expression<Func<UserFarm, bool>>>(),
                        It.IsAny<QueryType>()
                    )
                )
                .ReturnsAsync(
                    (IReadOnlyList<UserFarm>)new List<UserFarm> { new() { FarmId = dummyFarmId } }
                );

            _fishTankRepoMock
                .Setup(r =>
                    r.FindAllAsync(
                        It.IsAny<Expression<Func<FishTank, bool>>>(),
                        It.IsAny<QueryType>()
                    )
                )
                .ReturnsAsync(
                    (IReadOnlyList<FishTank>)
                        new List<FishTank>
                        {
                            new() { Id = dummyTankId, FarmId = dummyFarmId },
                        }
                );

            // batchRepo.FindAllAsync is used to build dashboardBatchIds before querying mortality.
            _batchRepoMock
                .Setup(r =>
                    r.FindAllAsync(
                        It.IsAny<Expression<Func<FarmingBatch, bool>>>(),
                        It.IsAny<QueryType>()
                    )
                )
                .ReturnsAsync((IReadOnlyList<FarmingBatch>)new List<FarmingBatch>());

            var alertList = new List<AlertStatus>();
            alertList.AddRange(Enumerable.Repeat(AlertStatus.OPEN, openAlerts));
            alertList.AddRange(Enumerable.Repeat(AlertStatus.ACKNOWLEDGED, acknowledgedAlerts));
            alertList.AddRange(Enumerable.Repeat(AlertStatus.RESOLVED, resolvedAlerts));
            alertList.AddRange(Enumerable.Repeat(AlertStatus.DISMISSED, dismissedAlerts));
            int remaining = totalAlerts - alertList.Count;
            if (remaining > 0)
                alertList.AddRange(Enumerable.Repeat((AlertStatus)999, remaining));

            _alertRepoMock
                .Setup(r =>
                    r.ListAsync(
                        It.IsAny<ISpecification<Alert, AlertStatus>>(),
                        It.IsAny<QueryType>()
                    )
                )
                .ReturnsAsync((IReadOnlyList<AlertStatus>)alertList);

            _batchRepoMock
                .SetupSequence(r =>
                    r.CountAsync(
                        It.IsAny<Expression<Func<FarmingBatch, bool>>>(),
                        It.IsAny<QueryType>()
                    )
                )
                .ReturnsAsync(activeBatches)
                .ReturnsAsync(harvestedBatches)
                .ReturnsAsync(totalBatches);

            _batchRepoMock
                .Setup(r =>
                    r.ListAsync(
                        It.IsAny<ISpecification<FarmingBatch, BatchSurvivalProjection>>(),
                        It.IsAny<QueryType>()
                    )
                )
                .ReturnsAsync(
                    (IReadOnlyList<BatchSurvivalProjection>)(
                        survivalData?.ToList() ?? new List<BatchSurvivalProjection>()
                    )
                );

            _mortalityRepoMock
                .Setup(r =>
                    r.FindAllAsync(
                        It.IsAny<Expression<Func<MortalityLog, bool>>>(),
                        It.IsAny<QueryType>()
                    )
                )
                .ReturnsAsync(
                    (IReadOnlyList<MortalityLog>)(
                        mortalityLogs?.ToList() ?? new List<MortalityLog>()
                    )
                );
        }

        /// <summary>
        /// Configures all repositories used by GetWeeklyReportAsync.
        /// The 5 alert CountAsync calls are sequenced:
        ///   totalAlerts → openAlerts → acknowledgedAlerts → resolvedAlerts → dismissedAlerts.
        /// Batch CountAsync is a single call (activeBatches).
        /// </summary>
        private void SetupWeeklyDefaults(
            int totalAlerts = 0,
            int openAlerts = 0,
            int acknowledgedAlerts = 0,
            int resolvedAlerts = 0,
            int dismissedAlerts = 0,
            IEnumerable<string>? sensorTypeNames = null,
            IEnumerable<WeeklyCorrectiveActionItem>? correctiveActions = null,
            IEnumerable<WeeklyRecommendationItem>? recommendations = null,
            IEnumerable<MortalityLog>? mortalityLogs = null,
            int activeBatches = 0,
            IEnumerable<BatchSurvivalProjection>? survivalData = null
        )
        {
            // ── Scope: return one dummy UserFarm → one dummy FishTank so the service
            //    proceeds past the early-exit guard (tankIds.Count == 0).
            var dummyFarmId = Guid.NewGuid();
            var dummyTankId = Guid.NewGuid();

            _userFarmRepoMock
                .Setup(r =>
                    r.FindAllAsync(
                        It.IsAny<Expression<Func<UserFarm, bool>>>(),
                        It.IsAny<QueryType>()
                    )
                )
                .ReturnsAsync(
                    (IReadOnlyList<UserFarm>)new List<UserFarm> { new() { FarmId = dummyFarmId } }
                );

            _fishTankRepoMock
                .Setup(r =>
                    r.FindAllAsync(
                        It.IsAny<Expression<Func<FishTank, bool>>>(),
                        It.IsAny<QueryType>()
                    )
                )
                .ReturnsAsync(
                    (IReadOnlyList<FishTank>)
                        new List<FishTank>
                        {
                            new() { Id = dummyTankId, FarmId = dummyFarmId },
                        }
                );

            // batchRepo.FindAllAsync is used to build weeklyBatchIds before querying mortality.
            _batchRepoMock
                .Setup(r =>
                    r.FindAllAsync(
                        It.IsAny<Expression<Func<FarmingBatch, bool>>>(),
                        It.IsAny<QueryType>()
                    )
                )
                .ReturnsAsync((IReadOnlyList<FarmingBatch>)new List<FarmingBatch>());

            var alertList = new List<AlertStatus>();
            alertList.AddRange(Enumerable.Repeat(AlertStatus.OPEN, openAlerts));
            alertList.AddRange(Enumerable.Repeat(AlertStatus.ACKNOWLEDGED, acknowledgedAlerts));
            alertList.AddRange(Enumerable.Repeat(AlertStatus.RESOLVED, resolvedAlerts));
            alertList.AddRange(Enumerable.Repeat(AlertStatus.DISMISSED, dismissedAlerts));
            int remaining = totalAlerts - alertList.Count;
            if (remaining > 0)
                alertList.AddRange(Enumerable.Repeat((AlertStatus)999, remaining));

            _alertRepoMock
                .Setup(r =>
                    r.ListAsync(
                        It.IsAny<ISpecification<Alert, AlertStatus>>(),
                        It.IsAny<QueryType>()
                    )
                )
                .ReturnsAsync((IReadOnlyList<AlertStatus>)alertList);

            _alertRepoMock
                .Setup(r =>
                    r.ListAsync(It.IsAny<ISpecification<Alert, string>>(), It.IsAny<QueryType>())
                )
                .ReturnsAsync(
                    (IReadOnlyList<string>)(sensorTypeNames?.ToList() ?? new List<string>())
                );

            _correctiveActionRepoMock
                .Setup(r =>
                    r.ListAsync(
                        It.IsAny<ISpecification<CorrectiveAction, WeeklyCorrectiveActionItem>>(),
                        It.IsAny<QueryType>()
                    )
                )
                .ReturnsAsync(
                    (IReadOnlyList<WeeklyCorrectiveActionItem>)(
                        correctiveActions?.ToList() ?? new List<WeeklyCorrectiveActionItem>()
                    )
                );

            _recommendationRepoMock
                .Setup(r =>
                    r.ListAsync(
                        It.IsAny<ISpecification<Recommendation, WeeklyRecommendationItem>>(),
                        It.IsAny<QueryType>()
                    )
                )
                .ReturnsAsync(
                    (IReadOnlyList<WeeklyRecommendationItem>)(
                        recommendations?.ToList() ?? new List<WeeklyRecommendationItem>()
                    )
                );

            _mortalityRepoMock
                .Setup(r =>
                    r.FindAllAsync(
                        It.IsAny<Expression<Func<MortalityLog, bool>>>(),
                        It.IsAny<QueryType>()
                    )
                )
                .ReturnsAsync(
                    (IReadOnlyList<MortalityLog>)(
                        mortalityLogs?.ToList() ?? new List<MortalityLog>()
                    )
                );

            _batchRepoMock
                .Setup(r =>
                    r.CountAsync(
                        It.IsAny<Expression<Func<FarmingBatch, bool>>>(),
                        It.IsAny<QueryType>()
                    )
                )
                .ReturnsAsync(activeBatches);

            _batchRepoMock
                .Setup(r =>
                    r.ListAsync(
                        It.IsAny<ISpecification<FarmingBatch, BatchSurvivalProjection>>(),
                        It.IsAny<QueryType>()
                    )
                )
                .ReturnsAsync(
                    (IReadOnlyList<BatchSurvivalProjection>)(
                        survivalData?.ToList() ?? new List<BatchSurvivalProjection>()
                    )
                );
        }

        // ═══════════════════════════════════════════════════════════════════════
        // GetDashboardSummaryAsync – Period Resolution
        // ═══════════════════════════════════════════════════════════════════════

        #region GetDashboardSummaryAsync – Period Resolution

        [Fact]
        public async Task GetDashboardSummaryAsync_ShouldSetMonthBoundaries_WhenPeriodIsMonth()
        {
            var now = DateTime.UtcNow;
            SetupDashboardDefaults();

            var result = await _sut.GetDashboardSummaryAsync(
                new DashboardQueryRequest { Period = ReportPeriod.MONTH }
            );

            result.Type.Should().Be(ResultType.Ok);
            result.Data!.PeriodLabel.Should().Be($"Tháng {now.Month}/{now.Year}");
            result
                .Data.PeriodFrom.Should()
                .Be(new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc));
            result
                .Data.PeriodTo.Should()
                .Be(new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc).AddMonths(1));
        }

        [Fact]
        public async Task GetDashboardSummaryAsync_ShouldSetTodayBoundaries_WhenPeriodIsToday()
        {
            var now = DateTime.UtcNow;
            SetupDashboardDefaults();

            var result = await _sut.GetDashboardSummaryAsync(
                new DashboardQueryRequest { Period = ReportPeriod.TODAY }
            );

            result.Type.Should().Be(ResultType.Ok);
            result.Data!.PeriodLabel.Should().Be("Hôm nay");
            result
                .Data.PeriodFrom.Should()
                .Be(new DateTime(now.Year, now.Month, now.Day, 0, 0, 0, DateTimeKind.Utc));
            result
                .Data.PeriodTo.Should()
                .Be(
                    new DateTime(now.Year, now.Month, now.Day, 0, 0, 0, DateTimeKind.Utc).AddDays(1)
                );
        }

        [Fact]
        public async Task GetDashboardSummaryAsync_ShouldSetWeekBoundaries_WhenPeriodIsWeek()
        {
            var now = DateTime.UtcNow;
            SetupDashboardDefaults();

            var result = await _sut.GetDashboardSummaryAsync(
                new DashboardQueryRequest { Period = ReportPeriod.WEEK }
            );

            result.Type.Should().Be(ResultType.Ok);
            result.Data!.PeriodLabel.Should().Be("7 ngày qua");
            // PeriodTo should be end of today
            result
                .Data.PeriodTo.Should()
                .Be(
                    new DateTime(now.Year, now.Month, now.Day, 0, 0, 0, DateTimeKind.Utc).AddDays(1)
                );
            // PeriodFrom should be 6 calendar days ago at midnight UTC
            result.Data.PeriodFrom.Date.Should().Be(now.AddDays(-6).Date);
        }

        [Fact]
        public async Task GetDashboardSummaryAsync_ShouldSetYearBoundaries_WhenPeriodIsYear()
        {
            var now = DateTime.UtcNow;
            SetupDashboardDefaults();

            var result = await _sut.GetDashboardSummaryAsync(
                new DashboardQueryRequest { Period = ReportPeriod.YEAR }
            );

            result.Type.Should().Be(ResultType.Ok);
            result.Data!.PeriodLabel.Should().Be($"Năm {now.Year}");
            result
                .Data.PeriodFrom.Should()
                .Be(new DateTime(now.Year, 1, 1, 0, 0, 0, DateTimeKind.Utc));
            result
                .Data.PeriodTo.Should()
                .Be(new DateTime(now.Year + 1, 1, 1, 0, 0, 0, DateTimeKind.Utc));
        }

        [Fact]
        public async Task GetDashboardSummaryAsync_ShouldDefaultToMonth_WhenPeriodIsDefault()
        {
            var now = DateTime.UtcNow;
            SetupDashboardDefaults();

            var result = await _sut.GetDashboardSummaryAsync(new DashboardQueryRequest());

            result.Type.Should().Be(ResultType.Ok);
            result.Data!.PeriodLabel.Should().Be($"Tháng {now.Month}/{now.Year}");
            result
                .Data.PeriodFrom.Should()
                .Be(new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc));
        }

        #endregion

        // ═══════════════════════════════════════════════════════════════════════
        // GetDashboardSummaryAsync – Alert KPIs
        // ═══════════════════════════════════════════════════════════════════════

        #region GetDashboardSummaryAsync – Alert KPIs

        [Fact]
        public async Task GetDashboardSummaryAsync_ShouldMapAllAlertCounts_WhenAlertsExist()
        {
            SetupDashboardDefaults(
                totalAlerts: 10,
                openAlerts: 3,
                acknowledgedAlerts: 2,
                resolvedAlerts: 4,
                dismissedAlerts: 1
            );

            var result = await _sut.GetDashboardSummaryAsync(
                new DashboardQueryRequest { Period = ReportPeriod.MONTH }
            );

            result.Type.Should().Be(ResultType.Ok);
            result.Data!.TotalAlerts.Should().Be(10);
            result.Data.OpenAlerts.Should().Be(3);
            result.Data.AcknowledgedAlerts.Should().Be(2);
            result.Data.ResolvedAlerts.Should().Be(4);
            result.Data.DismissedAlerts.Should().Be(1);
        }

        [Fact]
        public async Task GetDashboardSummaryAsync_ShouldReturnZeroAlertCounts_WhenNoAlertsExist()
        {
            SetupDashboardDefaults(); // all zeros

            var result = await _sut.GetDashboardSummaryAsync(
                new DashboardQueryRequest { Period = ReportPeriod.MONTH }
            );

            result.Type.Should().Be(ResultType.Ok);
            result.Data!.TotalAlerts.Should().Be(0);
            result.Data.OpenAlerts.Should().Be(0);
            result.Data.AcknowledgedAlerts.Should().Be(0);
            result.Data.ResolvedAlerts.Should().Be(0);
            result.Data.DismissedAlerts.Should().Be(0);
        }

        #endregion

        // ═══════════════════════════════════════════════════════════════════════
        // GetDashboardSummaryAsync – Survival Rate
        // ═══════════════════════════════════════════════════════════════════════

        #region GetDashboardSummaryAsync – Survival Rate

        [Fact]
        public async Task GetDashboardSummaryAsync_ShouldReturnNullSurvivalRate_WhenNoActiveBatchesExist()
        {
            SetupDashboardDefaults(survivalData: new List<BatchSurvivalProjection>());

            var result = await _sut.GetDashboardSummaryAsync(
                new DashboardQueryRequest { Period = ReportPeriod.MONTH }
            );

            result.Type.Should().Be(ResultType.Ok);
            result.Data!.AverageSurvivalRate.Should().BeNull();
            result.Data.TotalInitialQuantity.Should().Be(0);
            result.Data.TotalCurrentQuantity.Should().Be(0);
        }

        [Fact]
        public async Task GetDashboardSummaryAsync_ShouldReturnNullSurvivalRate_WhenTotalInitialQuantityIsZero()
        {
            SetupDashboardDefaults(
                survivalData: new List<BatchSurvivalProjection>
                {
                    new() { InitialQuantity = 0, CurrentQuantity = 0 },
                }
            );

            var result = await _sut.GetDashboardSummaryAsync(
                new DashboardQueryRequest { Period = ReportPeriod.MONTH }
            );

            result.Type.Should().Be(ResultType.Ok);
            result.Data!.AverageSurvivalRate.Should().BeNull();
        }

        [Fact]
        public async Task GetDashboardSummaryAsync_ShouldComputeSurvivalRate_RoundedTo2DecimalPlaces()
        {
            // 850 / 1000 * 100 = 85.00
            SetupDashboardDefaults(
                survivalData: new List<BatchSurvivalProjection>
                {
                    new() { InitialQuantity = 1000, CurrentQuantity = 850 },
                }
            );

            var result = await _sut.GetDashboardSummaryAsync(
                new DashboardQueryRequest { Period = ReportPeriod.MONTH }
            );

            result.Type.Should().Be(ResultType.Ok);
            result.Data!.AverageSurvivalRate.Should().Be(85.0);
        }

        [Fact]
        public async Task GetDashboardSummaryAsync_ShouldComputeFleetWideSurvivalRate_NotPerBatchMean()
        {
            // Fleet-wide: (850 + 450) / (1000 + 500) * 100 = 1300 / 1500 * 100 = 86.67
            SetupDashboardDefaults(
                survivalData: new List<BatchSurvivalProjection>
                {
                    new() { InitialQuantity = 1000, CurrentQuantity = 850 },
                    new() { InitialQuantity = 500, CurrentQuantity = 450 },
                }
            );

            var result = await _sut.GetDashboardSummaryAsync(
                new DashboardQueryRequest { Period = ReportPeriod.MONTH }
            );

            result.Type.Should().Be(ResultType.Ok);
            result.Data!.AverageSurvivalRate.Should().Be(86.67);
            result.Data.TotalInitialQuantity.Should().Be(1500);
            result.Data.TotalCurrentQuantity.Should().Be(1300);
        }

        #endregion

        // ═══════════════════════════════════════════════════════════════════════
        // GetDashboardSummaryAsync – Batch KPIs
        // ═══════════════════════════════════════════════════════════════════════

        #region GetDashboardSummaryAsync – Batch KPIs

        [Fact]
        public async Task GetDashboardSummaryAsync_ShouldMapBatchCounts_Correctly()
        {
            SetupDashboardDefaults(activeBatches: 5, harvestedBatches: 2, totalBatches: 7);

            var result = await _sut.GetDashboardSummaryAsync(
                new DashboardQueryRequest { Period = ReportPeriod.MONTH }
            );

            result.Type.Should().Be(ResultType.Ok);
            result.Data!.ActiveBatches.Should().Be(5);
            result.Data.HarvestedBatches.Should().Be(2);
            result.Data.TotalBatches.Should().Be(7);
        }

        #endregion

        // ═══════════════════════════════════════════════════════════════════════
        // GetDashboardSummaryAsync – Mortality
        // ═══════════════════════════════════════════════════════════════════════

        #region GetDashboardSummaryAsync – Mortality

        [Fact]
        public async Task GetDashboardSummaryAsync_ShouldSumTotalMortality_FromMortalityLogs()
        {
            var now = DateTime.UtcNow;
            SetupDashboardDefaults(
                mortalityLogs: new List<MortalityLog>
                {
                    MakeMortalityLog(now, 30),
                    MakeMortalityLog(now, 70),
                }
            );

            var result = await _sut.GetDashboardSummaryAsync(
                new DashboardQueryRequest { Period = ReportPeriod.MONTH }
            );

            result.Type.Should().Be(ResultType.Ok);
            result.Data!.TotalMortality.Should().Be(100);
        }

        [Fact]
        public async Task GetDashboardSummaryAsync_ShouldReturnZeroMortality_WhenNoLogsExist()
        {
            SetupDashboardDefaults(mortalityLogs: new List<MortalityLog>());

            var result = await _sut.GetDashboardSummaryAsync(
                new DashboardQueryRequest { Period = ReportPeriod.MONTH }
            );

            result.Type.Should().Be(ResultType.Ok);
            result.Data!.TotalMortality.Should().Be(0);
        }

        #endregion

        // ═══════════════════════════════════════════════════════════════════════
        // GetDashboardSummaryAsync – Success Path
        // ═══════════════════════════════════════════════════════════════════════

        #region GetDashboardSummaryAsync – Success Path

        [Fact]
        public async Task GetDashboardSummaryAsync_ShouldReturnOkWithNonNullData_OnHappyPath()
        {
            SetupDashboardDefaults(
                totalAlerts: 5,
                openAlerts: 2,
                acknowledgedAlerts: 1,
                resolvedAlerts: 2,
                dismissedAlerts: 0,
                activeBatches: 3,
                harvestedBatches: 1,
                totalBatches: 4,
                survivalData: new List<BatchSurvivalProjection>
                {
                    new() { InitialQuantity = 1000, CurrentQuantity = 900 },
                },
                mortalityLogs: new List<MortalityLog> { MakeMortalityLog(DateTime.UtcNow, 100) }
            );

            var result = await _sut.GetDashboardSummaryAsync(
                new DashboardQueryRequest { Period = ReportPeriod.MONTH }
            );

            result.Type.Should().Be(ResultType.Ok);
            result.IsSuccess.Should().BeTrue();
            result.Data.Should().NotBeNull();
            result.Message.Should().Be("Lấy dữ liệu tổng quan thành công.");
        }

        #endregion

        // ═══════════════════════════════════════════════════════════════════════
        // GetDashboardSummaryAsync – Exception Handling
        // ═══════════════════════════════════════════════════════════════════════

        #region GetDashboardSummaryAsync – Exception Handling

        [Fact]
        public async Task GetDashboardSummaryAsync_ShouldReturnUnexpected_WhenRepositoryThrows()
        {
            // Ensure the service passes the UserFarm/FishTank early-exit guard
            // so it reaches the alert repository call.
            var dummyFarmId = Guid.NewGuid();
            var dummyTankId = Guid.NewGuid();
            _userFarmRepoMock
                .Setup(r =>
                    r.FindAllAsync(
                        It.IsAny<Expression<Func<UserFarm, bool>>>(),
                        It.IsAny<QueryType>()
                    )
                )
                .ReturnsAsync(
                    (IReadOnlyList<UserFarm>)new List<UserFarm> { new() { FarmId = dummyFarmId } }
                );
            _fishTankRepoMock
                .Setup(r =>
                    r.FindAllAsync(
                        It.IsAny<Expression<Func<FishTank, bool>>>(),
                        It.IsAny<QueryType>()
                    )
                )
                .ReturnsAsync(
                    (IReadOnlyList<FishTank>)
                        new List<FishTank>
                        {
                            new() { Id = dummyTankId, FarmId = dummyFarmId },
                        }
                );

            // The service calls ListAsync(AlertStatusCountSpec), not CountAsync.
            _alertRepoMock
                .Setup(r =>
                    r.ListAsync(
                        It.IsAny<ISpecification<Alert, AlertStatus>>(),
                        It.IsAny<QueryType>()
                    )
                )
                .ThrowsAsync(new InvalidOperationException("DB connection failure"));

            var result = await _sut.GetDashboardSummaryAsync(
                new DashboardQueryRequest { Period = ReportPeriod.MONTH }
            );

            result.Type.Should().Be(ResultType.Unexpected);
            result.IsSuccess.Should().BeFalse();
            result.Message.Should().Be("Có lỗi xảy ra khi lấy dữ liệu tổng quan.");
            result.Data.Should().BeNull();
        }

        #endregion

        // ═══════════════════════════════════════════════════════════════════════
        // GetWeeklyReportAsync – Period Resolution
        // ═══════════════════════════════════════════════════════════════════════

        #region GetWeeklyReportAsync – Period Resolution

        [Fact]
        public async Task GetWeeklyReportAsync_ShouldReturnCurrentIsoWeekBoundaries_WhenPeriodIsCurrent()
        {
            var now = DateTime.UtcNow;
            var today = now.Date;
            int isoDayOfWeek = (int)today.DayOfWeek == 0 ? 7 : (int)today.DayOfWeek;
            var monday = today.AddDays(-(isoDayOfWeek - 1));
            var sunday = monday.AddDays(6);

            SetupWeeklyDefaults();

            var result = await _sut.GetWeeklyReportAsync(
                new WeeklyReportQueryRequest { Period = "current" }
            );

            result.Type.Should().Be(ResultType.Ok);
            result
                .Data!.PeriodFrom.Should()
                .Be(new DateTime(monday.Year, monday.Month, monday.Day, 0, 0, 0, DateTimeKind.Utc));
            result
                .Data.PeriodTo.Should()
                .Be(
                    new DateTime(
                        sunday.Year,
                        sunday.Month,
                        sunday.Day,
                        0,
                        0,
                        0,
                        DateTimeKind.Utc
                    ).AddDays(1)
                );
        }

        [Fact]
        public async Task GetWeeklyReportAsync_ShouldReturnPreviousIsoWeekBoundaries_WhenPeriodIsLast()
        {
            var now = DateTime.UtcNow;
            var today = now.Date;
            int isoDayOfWeek = (int)today.DayOfWeek == 0 ? 7 : (int)today.DayOfWeek;
            var monday = today.AddDays(-(isoDayOfWeek - 1) - 7);
            var sunday = monday.AddDays(6);

            SetupWeeklyDefaults();

            var result = await _sut.GetWeeklyReportAsync(
                new WeeklyReportQueryRequest { Period = "last" }
            );

            result.Type.Should().Be(ResultType.Ok);
            result
                .Data!.PeriodFrom.Should()
                .Be(new DateTime(monday.Year, monday.Month, monday.Day, 0, 0, 0, DateTimeKind.Utc));
            result
                .Data.PeriodTo.Should()
                .Be(
                    new DateTime(
                        sunday.Year,
                        sunday.Month,
                        sunday.Day,
                        0,
                        0,
                        0,
                        DateTimeKind.Utc
                    ).AddDays(1)
                );
        }

        [Fact]
        public async Task GetWeeklyReportAsync_ShouldReturnCorrectBoundaries_WhenPeriodIsTwoWeeksAgo()
        {
            var now = DateTime.UtcNow;
            var today = now.Date;
            int isoDayOfWeek = (int)today.DayOfWeek == 0 ? 7 : (int)today.DayOfWeek;
            var monday = today.AddDays(-(isoDayOfWeek - 1) - 14); // 2 × 7 days back
            var sunday = monday.AddDays(6);

            SetupWeeklyDefaults();

            var result = await _sut.GetWeeklyReportAsync(
                new WeeklyReportQueryRequest { Period = "2" }
            );

            result.Type.Should().Be(ResultType.Ok);
            result
                .Data!.PeriodFrom.Should()
                .Be(new DateTime(monday.Year, monday.Month, monday.Day, 0, 0, 0, DateTimeKind.Utc));
            result
                .Data.PeriodTo.Should()
                .Be(
                    new DateTime(
                        sunday.Year,
                        sunday.Month,
                        sunday.Day,
                        0,
                        0,
                        0,
                        DateTimeKind.Utc
                    ).AddDays(1)
                );
        }

        [Theory]
        [InlineData(null)]
        [InlineData("invalid_period")]
        public async Task GetWeeklyReportAsync_ShouldDefaultToCurrentIsoWeek_WhenPeriodIsNullOrUnknown(
            string? period
        )
        {
            var now = DateTime.UtcNow;
            var today = now.Date;
            int isoDayOfWeek = (int)today.DayOfWeek == 0 ? 7 : (int)today.DayOfWeek;
            var monday = today.AddDays(-(isoDayOfWeek - 1));

            SetupWeeklyDefaults();

            var result = await _sut.GetWeeklyReportAsync(
                new WeeklyReportQueryRequest { Period = period! }
            );

            result.Type.Should().Be(ResultType.Ok);
            result
                .Data!.PeriodFrom.Should()
                .Be(new DateTime(monday.Year, monday.Month, monday.Day, 0, 0, 0, DateTimeKind.Utc));
        }

        [Fact]
        public async Task GetWeeklyReportAsync_ShouldFormatPeriodLabel_WithWeekNumberAndDateRange()
        {
            var now = DateTime.UtcNow;
            var today = now.Date;
            int isoDayOfWeek = (int)today.DayOfWeek == 0 ? 7 : (int)today.DayOfWeek;
            var monday = today.AddDays(-(isoDayOfWeek - 1));
            var sunday = monday.AddDays(6);

            var cal = System.Globalization.CultureInfo.InvariantCulture.Calendar;
            int weekNum = cal.GetWeekOfYear(
                monday,
                System.Globalization.CalendarWeekRule.FirstFourDayWeek,
                DayOfWeek.Monday
            );
            var expectedLabel = $"Tuần {weekNum}/{monday.Year} ({monday:dd/MM} – {sunday:dd/MM})";

            SetupWeeklyDefaults();

            var result = await _sut.GetWeeklyReportAsync(
                new WeeklyReportQueryRequest { Period = "current" }
            );

            result.Type.Should().Be(ResultType.Ok);
            result.Data!.PeriodLabel.Should().Be(expectedLabel);
        }

        #endregion

        // ═══════════════════════════════════════════════════════════════════════
        // GetWeeklyReportAsync – Alert KPIs
        // ═══════════════════════════════════════════════════════════════════════

        #region GetWeeklyReportAsync – Alert KPIs

        [Fact]
        public async Task GetWeeklyReportAsync_ShouldMapAllAlertCounts_Correctly()
        {
            SetupWeeklyDefaults(
                totalAlerts: 20,
                openAlerts: 5,
                acknowledgedAlerts: 3,
                resolvedAlerts: 10,
                dismissedAlerts: 2
            );

            var result = await _sut.GetWeeklyReportAsync(
                new WeeklyReportQueryRequest { Period = "current" }
            );

            result.Type.Should().Be(ResultType.Ok);
            result.Data!.TotalAlerts.Should().Be(20);
            result.Data.OpenAlerts.Should().Be(5);
            result.Data.AcknowledgedAlerts.Should().Be(3);
            result.Data.ResolvedAlerts.Should().Be(10);
            result.Data.DismissedAlerts.Should().Be(2);
        }

        [Fact]
        public async Task GetWeeklyReportAsync_ShouldSortTopIssues_InDescendingOrderByCount()
        {
            SetupWeeklyDefaults(
                sensorTypeNames: new List<string>
                {
                    "pH",
                    "Temperature",
                    "pH",
                    "DO",
                    "pH",
                    "Temperature",
                }
            );

            var result = await _sut.GetWeeklyReportAsync(
                new WeeklyReportQueryRequest { Period = "current" }
            );

            result.Type.Should().Be(ResultType.Ok);
            var topIssues = result.Data!.TopIssuesBySensorType;
            topIssues.Should().BeInDescendingOrder(x => x.Count);
            topIssues[0].SensorTypeName.Should().Be("pH");
            topIssues[0].Count.Should().Be(3);
            topIssues[1].SensorTypeName.Should().Be("Temperature");
            topIssues[1].Count.Should().Be(2);
            topIssues[2].SensorTypeName.Should().Be("DO");
            topIssues[2].Count.Should().Be(1);
        }

        [Fact]
        public async Task GetWeeklyReportAsync_ShouldGroupTopIssues_BySameNameIntoSingleEntry()
        {
            SetupWeeklyDefaults(
                sensorTypeNames: new List<string> { "Temperature", "Temperature", "Temperature" }
            );

            var result = await _sut.GetWeeklyReportAsync(
                new WeeklyReportQueryRequest { Period = "current" }
            );

            result.Type.Should().Be(ResultType.Ok);
            result.Data!.TopIssuesBySensorType.Should().HaveCount(1);
            result.Data.TopIssuesBySensorType[0].SensorTypeName.Should().Be("Temperature");
            result.Data.TopIssuesBySensorType[0].Count.Should().Be(3);
        }

        [Fact]
        public async Task GetWeeklyReportAsync_ShouldReturnEmptyTopIssues_WhenNoAlertsExist()
        {
            SetupWeeklyDefaults(sensorTypeNames: new List<string>());

            var result = await _sut.GetWeeklyReportAsync(
                new WeeklyReportQueryRequest { Period = "current" }
            );

            result.Type.Should().Be(ResultType.Ok);
            result.Data!.TopIssuesBySensorType.Should().BeEmpty();
        }

        #endregion

        // ═══════════════════════════════════════════════════════════════════════
        // GetWeeklyReportAsync – Corrective Actions
        // ═══════════════════════════════════════════════════════════════════════

        #region GetWeeklyReportAsync – Corrective Actions

        [Fact]
        public async Task GetWeeklyReportAsync_ShouldReportFullTotalCorrectiveActions_EvenWhenAbove20()
        {
            var items = Enumerable.Range(0, 25).Select(_ => MakeCAItem(DateTime.UtcNow)).ToList();

            SetupWeeklyDefaults(correctiveActions: items);

            var result = await _sut.GetWeeklyReportAsync(
                new WeeklyReportQueryRequest { Period = "current" }
            );

            result.Type.Should().Be(ResultType.Ok);
            result.Data!.TotalCorrectiveActions.Should().Be(25);
            result.Data.CorrectiveActions.Should().HaveCount(20);
        }

        [Fact]
        public async Task GetWeeklyReportAsync_ShouldReturnEmptyCorrectiveActions_WhenNoneExist()
        {
            SetupWeeklyDefaults(correctiveActions: new List<WeeklyCorrectiveActionItem>());

            var result = await _sut.GetWeeklyReportAsync(
                new WeeklyReportQueryRequest { Period = "current" }
            );

            result.Type.Should().Be(ResultType.Ok);
            result.Data!.TotalCorrectiveActions.Should().Be(0);
            result.Data.CorrectiveActions.Should().BeEmpty();
        }

        #endregion

        // ═══════════════════════════════════════════════════════════════════════
        // GetWeeklyReportAsync – Recommendations
        // ═══════════════════════════════════════════════════════════════════════

        #region GetWeeklyReportAsync – Recommendations

        [Fact]
        public async Task GetWeeklyReportAsync_ShouldReportFullTotalRecommendations_EvenWhenAbove20()
        {
            var items = Enumerable.Range(0, 30).Select(_ => MakeRecItem()).ToList();

            SetupWeeklyDefaults(recommendations: items);

            var result = await _sut.GetWeeklyReportAsync(
                new WeeklyReportQueryRequest { Period = "current" }
            );

            result.Type.Should().Be(ResultType.Ok);
            result.Data!.TotalRecommendations.Should().Be(30);
            result.Data.Recommendations.Should().HaveCount(20);
        }

        [Fact]
        public async Task GetWeeklyReportAsync_ShouldReturnEmptyRecommendations_WhenNoneExist()
        {
            SetupWeeklyDefaults(recommendations: new List<WeeklyRecommendationItem>());

            var result = await _sut.GetWeeklyReportAsync(
                new WeeklyReportQueryRequest { Period = "current" }
            );

            result.Type.Should().Be(ResultType.Ok);
            result.Data!.TotalRecommendations.Should().Be(0);
            result.Data.Recommendations.Should().BeEmpty();
        }

        #endregion

        // ═══════════════════════════════════════════════════════════════════════
        // GetWeeklyReportAsync – Mortality
        // ═══════════════════════════════════════════════════════════════════════

        #region GetWeeklyReportAsync – Mortality

        [Fact]
        public async Task GetWeeklyReportAsync_ShouldSumTotalMortality_FromMortalityLogs()
        {
            var now = DateTime.UtcNow;
            SetupWeeklyDefaults(
                mortalityLogs: new List<MortalityLog>
                {
                    MakeMortalityLog(now, 40),
                    MakeMortalityLog(now, 60),
                }
            );

            var result = await _sut.GetWeeklyReportAsync(
                new WeeklyReportQueryRequest { Period = "current" }
            );

            result.Type.Should().Be(ResultType.Ok);
            result.Data!.TotalMortality.Should().Be(100);
        }

        [Fact]
        public async Task GetWeeklyReportAsync_ShouldCountMortalityIncidents_AsTotalLogRows()
        {
            var now = DateTime.UtcNow;
            SetupWeeklyDefaults(
                mortalityLogs: new List<MortalityLog>
                {
                    MakeMortalityLog(now, 10),
                    MakeMortalityLog(now, 20),
                    MakeMortalityLog(now, 30),
                }
            );

            var result = await _sut.GetWeeklyReportAsync(
                new WeeklyReportQueryRequest { Period = "current" }
            );

            result.Type.Should().Be(ResultType.Ok);
            result.Data!.MortalityIncidents.Should().Be(3);
            result.Data.TotalMortality.Should().Be(60);
        }

        [Fact]
        public async Task GetWeeklyReportAsync_ShouldReturnZeroMortalityAndZeroIncidents_WhenNoLogsExist()
        {
            SetupWeeklyDefaults(mortalityLogs: new List<MortalityLog>());

            var result = await _sut.GetWeeklyReportAsync(
                new WeeklyReportQueryRequest { Period = "current" }
            );

            result.Type.Should().Be(ResultType.Ok);
            result.Data!.TotalMortality.Should().Be(0);
            result.Data.MortalityIncidents.Should().Be(0);
        }

        #endregion

        // ═══════════════════════════════════════════════════════════════════════
        // GetWeeklyReportAsync – Batch Health
        // ═══════════════════════════════════════════════════════════════════════

        #region GetWeeklyReportAsync – Batch Health

        [Fact]
        public async Task GetWeeklyReportAsync_ShouldMapActiveBatchCount_Correctly()
        {
            SetupWeeklyDefaults(activeBatches: 7);

            var result = await _sut.GetWeeklyReportAsync(
                new WeeklyReportQueryRequest { Period = "current" }
            );

            result.Type.Should().Be(ResultType.Ok);
            result.Data!.ActiveBatches.Should().Be(7);
        }

        [Fact]
        public async Task GetWeeklyReportAsync_ShouldReturnNullSurvivalRate_WhenNoActiveBatchesExist()
        {
            SetupWeeklyDefaults(survivalData: new List<BatchSurvivalProjection>());

            var result = await _sut.GetWeeklyReportAsync(
                new WeeklyReportQueryRequest { Period = "current" }
            );

            result.Type.Should().Be(ResultType.Ok);
            result.Data!.AverageSurvivalRate.Should().BeNull();
        }

        [Fact]
        public async Task GetWeeklyReportAsync_ShouldComputeFleetWideSurvivalRate_WhenActiveBatchesExist()
        {
            // (400 + 500) / (500 + 500) * 100 = 900 / 1000 * 100 = 90.00
            SetupWeeklyDefaults(
                survivalData: new List<BatchSurvivalProjection>
                {
                    new() { InitialQuantity = 500, CurrentQuantity = 400 },
                    new() { InitialQuantity = 500, CurrentQuantity = 500 },
                }
            );

            var result = await _sut.GetWeeklyReportAsync(
                new WeeklyReportQueryRequest { Period = "current" }
            );

            result.Type.Should().Be(ResultType.Ok);
            result.Data!.AverageSurvivalRate.Should().Be(90.0);
        }

        #endregion

        // ═══════════════════════════════════════════════════════════════════════
        // GetWeeklyReportAsync – Success Path
        // ═══════════════════════════════════════════════════════════════════════

        #region GetWeeklyReportAsync – Success Path

        [Fact]
        public async Task GetWeeklyReportAsync_ShouldReturnOkWithNonNullData_OnHappyPath()
        {
            SetupWeeklyDefaults(
                totalAlerts: 8,
                openAlerts: 2,
                acknowledgedAlerts: 1,
                resolvedAlerts: 4,
                dismissedAlerts: 1,
                sensorTypeNames: new List<string> { "pH", "Temperature", "pH" },
                correctiveActions: new List<WeeklyCorrectiveActionItem>
                {
                    MakeCAItem(DateTime.UtcNow),
                },
                recommendations: new List<WeeklyRecommendationItem> { MakeRecItem() },
                mortalityLogs: new List<MortalityLog> { MakeMortalityLog(DateTime.UtcNow, 50) },
                activeBatches: 3,
                survivalData: new List<BatchSurvivalProjection>
                {
                    new() { InitialQuantity = 1000, CurrentQuantity = 850 },
                }
            );

            var result = await _sut.GetWeeklyReportAsync(
                new WeeklyReportQueryRequest { Period = "current" }
            );

            result.Type.Should().Be(ResultType.Ok);
            result.IsSuccess.Should().BeTrue();
            result.Data.Should().NotBeNull();
            result.Message.Should().Be("Lấy báo cáo tuần thành công.");
        }

        [Fact]
        public async Task GetWeeklyReportAsync_ShouldPopulateGeneratedAt_WithApproximateUtcNow()
        {
            var before = DateTime.UtcNow;
            SetupWeeklyDefaults();

            var result = await _sut.GetWeeklyReportAsync(
                new WeeklyReportQueryRequest { Period = "current" }
            );

            var after = DateTime.UtcNow;
            result.Type.Should().Be(ResultType.Ok);
            result.Data!.GeneratedAt.Should().BeOnOrAfter(before).And.BeOnOrBefore(after);
        }

        #endregion

        // ═══════════════════════════════════════════════════════════════════════
        // GetWeeklyReportAsync – Exception Handling
        // ═══════════════════════════════════════════════════════════════════════

        #region GetWeeklyReportAsync – Exception Handling

        [Fact]
        public async Task GetWeeklyReportAsync_ShouldReturnUnexpected_WhenRepositoryThrows()
        {
            // Ensure the service passes the UserFarm/FishTank early-exit guard
            // so it reaches the alert repository call.
            var dummyFarmId = Guid.NewGuid();
            var dummyTankId = Guid.NewGuid();
            _userFarmRepoMock
                .Setup(r =>
                    r.FindAllAsync(
                        It.IsAny<Expression<Func<UserFarm, bool>>>(),
                        It.IsAny<QueryType>()
                    )
                )
                .ReturnsAsync(
                    (IReadOnlyList<UserFarm>)new List<UserFarm> { new() { FarmId = dummyFarmId } }
                );
            _fishTankRepoMock
                .Setup(r =>
                    r.FindAllAsync(
                        It.IsAny<Expression<Func<FishTank, bool>>>(),
                        It.IsAny<QueryType>()
                    )
                )
                .ReturnsAsync(
                    (IReadOnlyList<FishTank>)
                        new List<FishTank>
                        {
                            new() { Id = dummyTankId, FarmId = dummyFarmId },
                        }
                );

            // The service calls ListAsync(AlertStatusCountSpec), not CountAsync.
            _alertRepoMock
                .Setup(r =>
                    r.ListAsync(
                        It.IsAny<ISpecification<Alert, AlertStatus>>(),
                        It.IsAny<QueryType>()
                    )
                )
                .ThrowsAsync(new InvalidOperationException("DB connection failure"));

            var result = await _sut.GetWeeklyReportAsync(
                new WeeklyReportQueryRequest { Period = "current" }
            );

            result.Type.Should().Be(ResultType.Unexpected);
            result.IsSuccess.Should().BeFalse();
            result.Message.Should().Be("Có lỗi xảy ra khi tạo báo cáo tuần.");
            result.Data.Should().BeNull();
        }

        #endregion
    }
}

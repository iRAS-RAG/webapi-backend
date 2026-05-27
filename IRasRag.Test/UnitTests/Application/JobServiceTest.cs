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
using IRasRag.Application.Specifications.JobSpecifications;
using IRasRag.Domain.Entities;
using IRasRag.Domain.Enums;
using IRasRag.Test.UnitTests.Helpers;
using Microsoft.Extensions.Logging;
using Moq;

namespace IRasRag.Test.UnitTests.Application
{
    public class JobServiceTest
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<ILogger<JobService>> _loggerMock;
        private readonly IMapper _mapper;

        private readonly Mock<IRepository<Job>> _jobRepoMock;
        private readonly Mock<IRepository<JobType>> _jobTypeRepoMock;
        private readonly Mock<IRepository<Sensor>> _sensorRepoMock;
        private readonly Mock<IRepository<JobControlMapping>> _mappingRepoMock;
        private readonly Mock<IRepository<ControlDevice>> _controlDeviceRepoMock;

        private readonly JobService _sut;

        public JobServiceTest()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _loggerMock = new Mock<ILogger<JobService>>();

            // Cần cả JobProfile và JobControlMappingProfile vì Job.JobControlMappings -> JobDto.Mappings
            _mapper = AutoMapperTestHelper.GetMapper(
                new JobProfile(),
                new JobControlMappingProfile()
            );

            _jobRepoMock = new Mock<IRepository<Job>>();
            _jobTypeRepoMock = new Mock<IRepository<JobType>>();
            _sensorRepoMock = new Mock<IRepository<Sensor>>();
            _mappingRepoMock = new Mock<IRepository<JobControlMapping>>();
            _controlDeviceRepoMock = new Mock<IRepository<ControlDevice>>();

            _unitOfWorkMock.Setup(x => x.GetRepository<Job>()).Returns(_jobRepoMock.Object);
            _unitOfWorkMock.Setup(x => x.GetRepository<JobType>()).Returns(_jobTypeRepoMock.Object);
            _unitOfWorkMock.Setup(x => x.GetRepository<Sensor>()).Returns(_sensorRepoMock.Object);
            _unitOfWorkMock
                .Setup(x => x.GetRepository<JobControlMapping>())
                .Returns(_mappingRepoMock.Object);
            _unitOfWorkMock
                .Setup(x => x.GetRepository<ControlDevice>())
                .Returns(_controlDeviceRepoMock.Object);
            _unitOfWorkMock
                .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            _sut = new JobService(_unitOfWorkMock.Object, _loggerMock.Object, _mapper);
        }

        // Helper tạo JobDto mẫu dùng chung trong các test
        private static JobDto BuildJobDto(Guid id, string name = "Cho ăn buổi sáng") =>
            new()
            {
                Id = id,
                Name = name,
                Description = "Mô tả",
                JobTypeId = Guid.NewGuid(),
                JobTypeName = "Lịch trình",
                IsActive = true,
                DefaultState = false,
                ExecutionDays = "ALL",
            };

        #region GetAllJobsAsync Tests

        [Fact]
        public async Task GetAllJobsAsync_ShouldReturnPagedResult_WhenJobsExist()
        {
            // Arrange
            var request = new JobListRequest { Page = 1, PageSize = 10 };
            var jobTypeId = Guid.NewGuid();
            var jobs = new List<Job>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    Name = "Sục khí định kỳ",
                    JobTypeId = jobTypeId,
                    JobType = new JobType { Name = "Lịch trình" },
                    IsActive = true,
                    DefaultState = false,
                    ExecutionDays = "ALL",
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    Name = "Cho ăn buổi sáng",
                    JobTypeId = jobTypeId,
                    JobType = new JobType { Name = "Lịch trình" },
                    IsActive = true,
                    DefaultState = false,
                    ExecutionDays = "ALL",
                },
            };

            _jobRepoMock
                .Setup(r =>
                    r.GetPagedAsync(
                        It.IsAny<ISpecification<Job, JobDto>>(),
                        request.Page,
                        request.PageSize,
                        It.IsAny<QueryType>()
                    )
                )
                .ReturnsAsync(
                    (ISpecification<Job, JobDto> spec, int page, int pageSize, QueryType _) =>
                        SpecificationTestHelper.ApplySpecificationWithPaging(
                            jobs,
                            spec,
                            page,
                            pageSize
                        )
                );

            // Act
            var result = await _sut.GetAllJobsAsync(request);

            // Assert
            result.Should().NotBeNull();
            result.Message.Should().Be("Lấy danh sách công việc thành công");
            result.Data.Should().HaveCount(2);
            // Mặc định sắp xếp theo tên
            result
                .Data.Select(x => x.Name)
                .Should()
                .ContainInOrder("Cho ăn buổi sáng", "Sục khí định kỳ");
            result.Meta.Should().NotBeNull();
            result.Meta!.TotalItems.Should().Be(2);
            result.Links.Should().NotBeNull();

            _jobRepoMock.Verify(
                r =>
                    r.GetPagedAsync(
                        It.Is<ISpecification<Job, JobDto>>(s => s is JobDtoListSpec),
                        request.Page,
                        request.PageSize,
                        It.IsAny<QueryType>()
                    ),
                Times.Once
            );
        }

        [Fact]
        public async Task GetAllJobsAsync_ShouldReturnEmptyMessage_WhenNoJobsExist()
        {
            // Arrange
            var request = new JobListRequest { Page = 1, PageSize = 10 };

            _jobRepoMock
                .Setup(r =>
                    r.GetPagedAsync(
                        It.IsAny<ISpecification<Job, JobDto>>(),
                        request.Page,
                        request.PageSize,
                        It.IsAny<QueryType>()
                    )
                )
                .ReturnsAsync(
                    new PagedResult<JobDto> { Items = new List<JobDto>(), TotalItems = 0 }
                );

            // Act
            var result = await _sut.GetAllJobsAsync(request);

            // Assert
            result.Data.Should().BeEmpty();
            result.Message.Should().Be("Không có công việc nào");
            result.Meta!.TotalItems.Should().Be(0);
        }

        [Fact]
        public async Task GetAllJobsAsync_ShouldApplyIsActiveFilter_WhenProvided()
        {
            // Arrange
            var request = new JobListRequest
            {
                Page = 1,
                PageSize = 10,
                IsActive = true,
            };
            var jobTypeId = Guid.NewGuid();
            var jobs = new List<Job>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    Name = "Job A",
                    JobTypeId = jobTypeId,
                    JobType = new JobType { Name = "T" },
                    IsActive = true,
                    DefaultState = false,
                    ExecutionDays = "ALL",
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    Name = "Job B",
                    JobTypeId = jobTypeId,
                    JobType = new JobType { Name = "T" },
                    IsActive = false, // bị lọc ra
                    DefaultState = false,
                    ExecutionDays = "ALL",
                },
            };

            _jobRepoMock
                .Setup(r =>
                    r.GetPagedAsync(
                        It.IsAny<ISpecification<Job, JobDto>>(),
                        request.Page,
                        request.PageSize,
                        It.IsAny<QueryType>()
                    )
                )
                .ReturnsAsync(
                    (ISpecification<Job, JobDto> spec, int page, int pageSize, QueryType _) =>
                        SpecificationTestHelper.ApplySpecificationWithPaging(
                            jobs,
                            spec,
                            page,
                            pageSize
                        )
                );

            // Act
            var result = await _sut.GetAllJobsAsync(request);

            // Assert
            result.Data.Should().HaveCount(1);
            result.Data.First().Name.Should().Be("Job A");
        }

        [Fact]
        public async Task GetAllJobsAsync_ShouldReturnErrorMessage_WhenExceptionThrown()
        {
            // Arrange
            var request = new JobListRequest { Page = 1, PageSize = 10 };

            _jobRepoMock
                .Setup(r =>
                    r.GetPagedAsync(
                        It.IsAny<ISpecification<Job, JobDto>>(),
                        It.IsAny<int>(),
                        It.IsAny<int>(),
                        It.IsAny<QueryType>()
                    )
                )
                .ThrowsAsync(new Exception());

            // Act
            var result = await _sut.GetAllJobsAsync(request);

            // Assert
            result.Data.Should().BeEmpty();
            result.Message.Should().Be("Đã xảy ra lỗi khi lấy danh sách công việc");
            result.Meta.Should().BeNull();
            result.Links.Should().BeNull();
        }

        #endregion

        #region GetJobByIdAsync Tests

        [Fact]
        public async Task GetJobByIdAsync_ShouldReturnSuccess_WhenJobExists()
        {
            // Arrange
            var jobId = Guid.NewGuid();
            var jobDto = BuildJobDto(jobId);

            // Mock FirstOrDefaultAsync với projected spec (JobDtoByIdSpec)
            _jobRepoMock
                .Setup(r =>
                    r.FirstOrDefaultAsync(
                        It.IsAny<ISpecification<Job, JobDto>>(),
                        It.IsAny<QueryType>()
                    )
                )
                .ReturnsAsync(jobDto);

            // Act
            var result = await _sut.GetJobByIdAsync(jobId);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Type.Should().Be(ResultType.Ok);
            result.Message.Should().Be("Lấy công việc thành công");
            result.Data.Should().NotBeNull();
            result.Data!.Id.Should().Be(jobId);
            result.Data.Name.Should().Be(jobDto.Name);

            _jobRepoMock.Verify(
                r =>
                    r.FirstOrDefaultAsync(
                        It.Is<ISpecification<Job, JobDto>>(s => s is JobDtoByIdSpec),
                        It.IsAny<QueryType>()
                    ),
                Times.Once
            );
        }

        [Fact]
        public async Task GetJobByIdAsync_ShouldReturnNotFound_WhenJobDoesNotExist()
        {
            // Arrange
            var jobId = Guid.NewGuid();

            _jobRepoMock
                .Setup(r =>
                    r.FirstOrDefaultAsync(
                        It.IsAny<ISpecification<Job, JobDto>>(),
                        It.IsAny<QueryType>()
                    )
                )
                .ReturnsAsync((JobDto?)null);

            // Act
            var result = await _sut.GetJobByIdAsync(jobId);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Type.Should().Be(ResultType.NotFound);
            result.Data.Should().BeNull();
        }

        [Fact]
        public async Task GetJobByIdAsync_ShouldReturnUnexpected_WhenExceptionThrown()
        {
            // Arrange
            var jobId = Guid.NewGuid();

            _jobRepoMock
                .Setup(r =>
                    r.FirstOrDefaultAsync(
                        It.IsAny<ISpecification<Job, JobDto>>(),
                        It.IsAny<QueryType>()
                    )
                )
                .ThrowsAsync(new Exception());

            // Act
            var result = await _sut.GetJobByIdAsync(jobId);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Type.Should().Be(ResultType.Unexpected);
        }

        #endregion

        #region CreateJobAsync Tests

        [Fact]
        public async Task CreateJobAsync_ShouldReturnSuccess_WhenDataIsValid()
        {
            // Arrange
            var jobTypeId = Guid.NewGuid();
            var createDto = new CreateJobDto
            {
                Name = "Cho ăn buổi sáng",
                Description = "Cho cá ăn lúc 6h",
                JobTypeId = jobTypeId,
                IsActive = true,
                DefaultState = false,
                StartTime = new TimeSpan(6, 0, 0),
                EndTime = new TimeSpan(6, 5, 0),
                ExecutionDays = "ALL",
            };
            var jobId = Guid.NewGuid();
            var expectedDto = BuildJobDto(jobId, createDto.Name);

            _jobTypeRepoMock
                .Setup(r => r.GetByIdAsync(jobTypeId, QueryType.ActiveOnly))
                .ReturnsAsync(new JobType { Id = jobTypeId, Name = "Lịch trình" });

            _jobRepoMock
                .Setup(r =>
                    r.FirstOrDefaultAsync(
                        It.IsAny<ISpecification<Job, JobDto>>(),
                        It.IsAny<QueryType>()
                    )
                )
                .ReturnsAsync(expectedDto);

            // Act
            var result = await _sut.CreateJobAsync(createDto);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Type.Should().Be(ResultType.Ok);
            result.Message.Should().Be("Tạo công việc thành công");
            result.Data.Should().NotBeNull();
            result.Data!.Name.Should().Be(createDto.Name);

            _jobRepoMock.Verify(
                r => r.AddAsync(It.Is<Job>(j => j.Name == createDto.Name)),
                Times.Once
            );
            // CommitTransactionAsync được gọi 1 lần — lưu Job trong transaction
            _unitOfWorkMock.Verify(
                u => u.CommitTransactionAsync(It.IsAny<CancellationToken>()),
                Times.Once
            );
        }

        [Fact]
        public async Task CreateJobAsync_ShouldCreateMappings_WhenMappingsProvided()
        {
            // Arrange
            var jobTypeId = Guid.NewGuid();
            var deviceId = Guid.NewGuid();
            var createDto = new CreateJobDto
            {
                Name = "Kiểm soát nhiệt độ",
                JobTypeId = jobTypeId,
                Mappings =
                [
                    new()
                    {
                        ControlDeviceId = deviceId,
                        TargetState = true,
                        TriggerCondition = JobTriggerCondition.ABOVE_MAX,
                    },
                ],
            };
            var expectedDto = BuildJobDto(Guid.NewGuid(), createDto.Name);

            _jobTypeRepoMock
                .Setup(r => r.GetByIdAsync(jobTypeId, QueryType.ActiveOnly))
                .ReturnsAsync(new JobType { Id = jobTypeId, Name = "Cảm biến" });

            // Thiết bị điều khiển tồn tại — dùng AnyAsync (cách service kiểm tra hiện tại)
            _controlDeviceRepoMock
                .Setup(r =>
                    r.AnyAsync(
                        It.IsAny<Expression<Func<ControlDevice, bool>>>(),
                        It.IsAny<QueryType>()
                    )
                )
                .ReturnsAsync(true);

            _jobRepoMock
                .Setup(r =>
                    r.FirstOrDefaultAsync(
                        It.IsAny<ISpecification<Job, JobDto>>(),
                        It.IsAny<QueryType>()
                    )
                )
                .ReturnsAsync(expectedDto);

            // Act
            var result = await _sut.CreateJobAsync(createDto);

            // Assert
            result.IsSuccess.Should().BeTrue();

            // Phải gọi AddAsync cho mapping
            _mappingRepoMock.Verify(
                r =>
                    r.AddAsync(
                        It.Is<JobControlMapping>(m =>
                            m.ControlDeviceId == deviceId && m.TargetState == true
                        )
                    ),
                Times.Once
            );

            // CommitTransactionAsync được gọi 1 lần — lưu Job + Mappings nguyên tử
            _unitOfWorkMock.Verify(
                u => u.CommitTransactionAsync(It.IsAny<CancellationToken>()),
                Times.Once
            );
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData("\t")]
        public async Task CreateJobAsync_ShouldReturnBadRequest_WhenNameIsEmpty(string name)
        {
            // Arrange
            var createDto = new CreateJobDto { Name = name, JobTypeId = Guid.NewGuid() };

            // Act
            var result = await _sut.CreateJobAsync(createDto);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Type.Should().Be(ResultType.BadRequest);
            result.Message.Should().Be("Tên công việc là bắt buộc");

            _jobRepoMock.Verify(r => r.AddAsync(It.IsAny<Job>()), Times.Never);
            _unitOfWorkMock.Verify(
                u => u.SaveChangesAsync(It.IsAny<CancellationToken>()),
                Times.Never
            );
        }

        [Fact]
        public async Task CreateJobAsync_ShouldReturnNotFound_WhenJobTypeNotExists()
        {
            // Arrange
            var jobTypeId = Guid.NewGuid();
            var createDto = new CreateJobDto { Name = "Job mới", JobTypeId = jobTypeId };

            _jobTypeRepoMock
                .Setup(r => r.GetByIdAsync(jobTypeId, QueryType.ActiveOnly))
                .ReturnsAsync((JobType?)null);

            // Act
            var result = await _sut.CreateJobAsync(createDto);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Type.Should().Be(ResultType.NotFound);
            result.Message.Should().Contain(jobTypeId.ToString());

            _jobRepoMock.Verify(r => r.AddAsync(It.IsAny<Job>()), Times.Never);
        }

        [Fact]
        public async Task CreateJobAsync_ShouldReturnNotFound_WhenSensorNotExists()
        {
            // Arrange
            var jobTypeId = Guid.NewGuid();
            var sensorId = Guid.NewGuid();
            var createDto = new CreateJobDto
            {
                Name = "Job cảm biến",
                JobTypeId = jobTypeId,
                SensorId = sensorId,
            };

            _jobTypeRepoMock
                .Setup(r => r.GetByIdAsync(jobTypeId, QueryType.ActiveOnly))
                .ReturnsAsync(new JobType { Id = jobTypeId, Name = "Cảm biến" });

            _sensorRepoMock
                .Setup(r => r.GetByIdAsync(sensorId, QueryType.ActiveOnly))
                .ReturnsAsync((Sensor?)null);

            // Act
            var result = await _sut.CreateJobAsync(createDto);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Type.Should().Be(ResultType.NotFound);
            result.Message.Should().Contain(sensorId.ToString());

            _jobRepoMock.Verify(r => r.AddAsync(It.IsAny<Job>()), Times.Never);
        }

        [Fact]
        public async Task CreateJobAsync_ShouldReturnBadRequest_WhenStartTimeGreaterThanOrEqualEndTime()
        {
            // Arrange
            var jobTypeId = Guid.NewGuid();
            var createDto = new CreateJobDto
            {
                Name = "Job lịch",
                JobTypeId = jobTypeId,
                StartTime = new TimeSpan(8, 0, 0),
                EndTime = new TimeSpan(6, 0, 0), // EndTime < StartTime
            };

            _jobTypeRepoMock
                .Setup(r => r.GetByIdAsync(jobTypeId, QueryType.ActiveOnly))
                .ReturnsAsync(new JobType { Id = jobTypeId, Name = "Lịch trình" });

            // Act
            var result = await _sut.CreateJobAsync(createDto);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Type.Should().Be(ResultType.BadRequest);
            result.Message.Should().Be("Thời gian bắt đầu phải nhỏ hơn thời gian kết thúc");
        }

        [Fact]
        public async Task CreateJobAsync_ShouldReturnBadRequest_WhenStartTimeEqualEndTime()
        {
            // Arrange
            var jobTypeId = Guid.NewGuid();
            var createDto = new CreateJobDto
            {
                Name = "Job lịch",
                JobTypeId = jobTypeId,
                StartTime = new TimeSpan(6, 0, 0),
                EndTime = new TimeSpan(6, 0, 0), // Bằng nhau
            };

            _jobTypeRepoMock
                .Setup(r => r.GetByIdAsync(jobTypeId, QueryType.ActiveOnly))
                .ReturnsAsync(new JobType { Id = jobTypeId, Name = "Lịch trình" });

            // Act
            var result = await _sut.CreateJobAsync(createDto);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Type.Should().Be(ResultType.BadRequest);
        }

        [Fact]
        public async Task CreateJobAsync_ShouldReturnBadRequest_WhenDuplicateControlDeviceInMappings()
        {
            // Arrange
            var jobTypeId = Guid.NewGuid();
            var deviceId = Guid.NewGuid();
            var createDto = new CreateJobDto
            {
                Name = "Job trùng mapping",
                JobTypeId = jobTypeId,
                Mappings =
                [
                    new() { ControlDeviceId = deviceId, TargetState = true },
                    new() { ControlDeviceId = deviceId, TargetState = false }, // Trùng
                ],
            };

            _jobTypeRepoMock
                .Setup(r => r.GetByIdAsync(jobTypeId, QueryType.ActiveOnly))
                .ReturnsAsync(new JobType { Id = jobTypeId, Name = "Lịch trình" });

            // Act
            var result = await _sut.CreateJobAsync(createDto);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Type.Should().Be(ResultType.BadRequest);
            result
                .Message.Should()
                .Be("Danh sách mappings không được có thiết bị điều khiển trùng nhau");

            _mappingRepoMock.Verify(r => r.AddAsync(It.IsAny<JobControlMapping>()), Times.Never);
        }

        [Fact]
        public async Task CreateJobAsync_ShouldReturnNotFound_WhenControlDeviceInMappingNotExists()
        {
            // Arrange
            var jobTypeId = Guid.NewGuid();
            var deviceId = Guid.NewGuid();
            var createDto = new CreateJobDto
            {
                Name = "Job với mapping lỗi",
                JobTypeId = jobTypeId,
                Mappings = [new() { ControlDeviceId = deviceId, TargetState = true }],
            };

            _jobTypeRepoMock
                .Setup(r => r.GetByIdAsync(jobTypeId, QueryType.ActiveOnly))
                .ReturnsAsync(new JobType { Id = jobTypeId, Name = "Lịch trình" });

            // Thiết bị không tồn tại — dùng AnyAsync
            _controlDeviceRepoMock
                .Setup(r =>
                    r.AnyAsync(
                        It.IsAny<Expression<Func<ControlDevice, bool>>>(),
                        It.IsAny<QueryType>()
                    )
                )
                .ReturnsAsync(false);

            // Act
            var result = await _sut.CreateJobAsync(createDto);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Type.Should().Be(ResultType.NotFound);
            result.Message.Should().Contain(deviceId.ToString());

            _mappingRepoMock.Verify(r => r.AddAsync(It.IsAny<JobControlMapping>()), Times.Never);
        }

        [Fact]
        public async Task CreateJobAsync_ShouldReturnUnexpected_WhenExceptionThrown()
        {
            // Arrange
            var createDto = new CreateJobDto { Name = "Job lỗi", JobTypeId = Guid.NewGuid() };

            _jobTypeRepoMock
                .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), QueryType.ActiveOnly))
                .ThrowsAsync(new Exception());

            // Act
            var result = await _sut.CreateJobAsync(createDto);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Type.Should().Be(ResultType.Unexpected);
            result.Message.Should().Be("Đã xảy ra lỗi khi tạo công việc");
        }

        #endregion

        #region UpdateJobAsync Tests

        [Fact]
        public async Task UpdateJobAsync_ShouldReturnSuccess_WhenDataIsValid()
        {
            // Arrange
            var jobId = Guid.NewGuid();
            var existingJob = new Job
            {
                Id = jobId,
                Name = "Job cũ",
                JobTypeId = Guid.NewGuid(),
                IsActive = true,
                DefaultState = false,
                ExecutionDays = "ALL",
            };
            var updateDto = new UpdateJobDto { Name = "Job mới", IsActive = false };
            var expectedDto = BuildJobDto(jobId, "Job mới");

            _jobRepoMock
                .Setup(r => r.GetByIdAsync(jobId, QueryType.ActiveOnly))
                .ReturnsAsync(existingJob);

            _jobRepoMock
                .Setup(r =>
                    r.FirstOrDefaultAsync(
                        It.IsAny<ISpecification<Job, JobDto>>(),
                        It.IsAny<QueryType>()
                    )
                )
                .ReturnsAsync(expectedDto);

            // Act
            var result = await _sut.UpdateJobAsync(jobId, updateDto);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Type.Should().Be(ResultType.Ok);
            result.Message.Should().Be("Cập nhật công việc thành công");
            result.Data.Should().NotBeNull();
            result.Data!.Name.Should().Be("Job mới");

            // Tên và IsActive đã được cập nhật trên entity
            existingJob.Name.Should().Be("Job mới");
            existingJob.IsActive.Should().BeFalse();

            _jobRepoMock.Verify(r => r.Update(It.Is<Job>(j => j.Id == jobId)), Times.Once);
            // CommitTransactionAsync được gọi 1 lần — lưu Job trong transaction
            _unitOfWorkMock.Verify(
                u => u.CommitTransactionAsync(It.IsAny<CancellationToken>()),
                Times.Once
            );
        }

        [Fact]
        public async Task UpdateJobAsync_ShouldNotUpdateName_WhenNameIsNullOrWhitespace()
        {
            // Arrange
            var jobId = Guid.NewGuid();
            var existingJob = new Job
            {
                Id = jobId,
                Name = "Tên gốc",
                JobTypeId = Guid.NewGuid(),
                IsActive = true,
                DefaultState = false,
                ExecutionDays = "ALL",
            };
            var updateDto = new UpdateJobDto { Name = "   " }; // Khoảng trắng → không cập nhật

            _jobRepoMock
                .Setup(r => r.GetByIdAsync(jobId, QueryType.ActiveOnly))
                .ReturnsAsync(existingJob);

            _jobRepoMock
                .Setup(r =>
                    r.FirstOrDefaultAsync(
                        It.IsAny<ISpecification<Job, JobDto>>(),
                        It.IsAny<QueryType>()
                    )
                )
                .ReturnsAsync(BuildJobDto(jobId, "Tên gốc"));

            // Act
            var result = await _sut.UpdateJobAsync(jobId, updateDto);

            // Assert
            result.IsSuccess.Should().BeTrue();
            existingJob.Name.Should().Be("Tên gốc"); // Tên không thay đổi
        }

        [Fact]
        public async Task UpdateJobAsync_ShouldReturnNotFound_WhenJobDoesNotExist()
        {
            // Arrange
            var jobId = Guid.NewGuid();

            _jobRepoMock
                .Setup(r => r.GetByIdAsync(jobId, QueryType.ActiveOnly))
                .ReturnsAsync((Job?)null);

            // Act
            var result = await _sut.UpdateJobAsync(jobId, new UpdateJobDto { Name = "x" });

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Type.Should().Be(ResultType.NotFound);
            result.Message.Should().Contain(jobId.ToString());

            _jobRepoMock.Verify(r => r.Update(It.IsAny<Job>()), Times.Never);
            _unitOfWorkMock.Verify(
                u => u.SaveChangesAsync(It.IsAny<CancellationToken>()),
                Times.Never
            );
        }

        [Fact]
        public async Task UpdateJobAsync_ShouldReturnNotFound_WhenNewJobTypeNotExists()
        {
            // Arrange
            var jobId = Guid.NewGuid();
            var newJobTypeId = Guid.NewGuid();

            _jobRepoMock
                .Setup(r => r.GetByIdAsync(jobId, QueryType.ActiveOnly))
                .ReturnsAsync(
                    new Job
                    {
                        Id = jobId,
                        Name = "Job",
                        JobTypeId = Guid.NewGuid(),
                        IsActive = true,
                        DefaultState = false,
                    }
                );

            _jobTypeRepoMock
                .Setup(r => r.GetByIdAsync(newJobTypeId, QueryType.ActiveOnly))
                .ReturnsAsync((JobType?)null);

            // Act
            var result = await _sut.UpdateJobAsync(
                jobId,
                new UpdateJobDto { JobTypeId = newJobTypeId }
            );

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Type.Should().Be(ResultType.NotFound);
            result.Message.Should().Contain(newJobTypeId.ToString());
        }

        [Fact]
        public async Task UpdateJobAsync_ShouldReturnNotFound_WhenNewSensorNotExists()
        {
            // Arrange
            var jobId = Guid.NewGuid();
            var newSensorId = Guid.NewGuid();

            _jobRepoMock
                .Setup(r => r.GetByIdAsync(jobId, QueryType.ActiveOnly))
                .ReturnsAsync(
                    new Job
                    {
                        Id = jobId,
                        Name = "Job",
                        JobTypeId = Guid.NewGuid(),
                        IsActive = true,
                        DefaultState = false,
                    }
                );

            _sensorRepoMock
                .Setup(r => r.GetByIdAsync(newSensorId, QueryType.ActiveOnly))
                .ReturnsAsync((Sensor?)null);

            // Act
            var result = await _sut.UpdateJobAsync(
                jobId,
                new UpdateJobDto { SensorId = newSensorId }
            );

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Type.Should().Be(ResultType.NotFound);
            result.Message.Should().Contain(newSensorId.ToString());
        }

        [Fact]
        public async Task UpdateJobAsync_ShouldReturnBadRequest_WhenUpdatedStartTimeGreaterThanEndTime()
        {
            // Arrange
            var jobId = Guid.NewGuid();
            var existingJob = new Job
            {
                Id = jobId,
                Name = "Job",
                JobTypeId = Guid.NewGuid(),
                IsActive = true,
                DefaultState = false,
                StartTime = new TimeSpan(6, 0, 0),
                EndTime = new TimeSpan(8, 0, 0),
            };

            _jobRepoMock
                .Setup(r => r.GetByIdAsync(jobId, QueryType.ActiveOnly))
                .ReturnsAsync(existingJob);

            // Cập nhật StartTime lớn hơn EndTime hiện tại
            var updateDto = new UpdateJobDto { StartTime = new TimeSpan(10, 0, 0) };

            // Act
            var result = await _sut.UpdateJobAsync(jobId, updateDto);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Type.Should().Be(ResultType.BadRequest);
            result.Message.Should().Be("Thời gian bắt đầu phải nhỏ hơn thời gian kết thúc");
        }

        [Fact]
        public async Task UpdateJobAsync_ShouldReplaceMappings_WhenMappingsListProvided()
        {
            // Arrange
            var jobId = Guid.NewGuid();
            var newDeviceId = Guid.NewGuid();
            var existingMappings = new List<JobControlMapping>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    JobId = jobId,
                    ControlDeviceId = Guid.NewGuid(),
                },
            };

            _jobRepoMock
                .Setup(r => r.GetByIdAsync(jobId, QueryType.ActiveOnly))
                .ReturnsAsync(
                    new Job
                    {
                        Id = jobId,
                        Name = "Job",
                        JobTypeId = Guid.NewGuid(),
                        IsActive = true,
                        DefaultState = false,
                    }
                );

            _mappingRepoMock
                .Setup(r =>
                    r.FindAllAsync(
                        It.IsAny<Expression<Func<JobControlMapping, bool>>>(),
                        It.IsAny<QueryType>()
                    )
                )
                .ReturnsAsync(existingMappings);

            // Thiết bị mới tồn tại — dùng AnyAsync
            _controlDeviceRepoMock
                .Setup(r =>
                    r.AnyAsync(
                        It.IsAny<Expression<Func<ControlDevice, bool>>>(),
                        It.IsAny<QueryType>()
                    )
                )
                .ReturnsAsync(true);

            _jobRepoMock
                .Setup(r =>
                    r.FirstOrDefaultAsync(
                        It.IsAny<ISpecification<Job, JobDto>>(),
                        It.IsAny<QueryType>()
                    )
                )
                .ReturnsAsync(BuildJobDto(jobId));

            var updateDto = new UpdateJobDto
            {
                Mappings = [new() { ControlDeviceId = newDeviceId, TargetState = false }],
            };

            // Act
            var result = await _sut.UpdateJobAsync(jobId, updateDto);

            // Assert
            result.IsSuccess.Should().BeTrue();

            // Phải xóa mapping cũ
            _mappingRepoMock.Verify(
                r => r.Delete(It.Is<JobControlMapping>(m => m.JobId == jobId)),
                Times.Once
            );

            // Phải thêm mapping mới
            _mappingRepoMock.Verify(
                r => r.AddAsync(It.Is<JobControlMapping>(m => m.ControlDeviceId == newDeviceId)),
                Times.Once
            );
        }

        [Fact]
        public async Task UpdateJobAsync_ShouldDeleteAllMappings_WhenEmptyMappingsListProvided()
        {
            // Arrange
            var jobId = Guid.NewGuid();
            var existingMappings = new List<JobControlMapping>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    JobId = jobId,
                    ControlDeviceId = Guid.NewGuid(),
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    JobId = jobId,
                    ControlDeviceId = Guid.NewGuid(),
                },
            };

            _jobRepoMock
                .Setup(r => r.GetByIdAsync(jobId, QueryType.ActiveOnly))
                .ReturnsAsync(
                    new Job
                    {
                        Id = jobId,
                        Name = "Job",
                        JobTypeId = Guid.NewGuid(),
                        IsActive = true,
                        DefaultState = false,
                    }
                );

            _mappingRepoMock
                .Setup(r =>
                    r.FindAllAsync(
                        It.IsAny<Expression<Func<JobControlMapping, bool>>>(),
                        It.IsAny<QueryType>()
                    )
                )
                .ReturnsAsync(existingMappings);

            _jobRepoMock
                .Setup(r =>
                    r.FirstOrDefaultAsync(
                        It.IsAny<ISpecification<Job, JobDto>>(),
                        It.IsAny<QueryType>()
                    )
                )
                .ReturnsAsync(BuildJobDto(jobId));

            // Truyền mảng rỗng → xóa hết mappings
            var updateDto = new UpdateJobDto { Mappings = [] };

            // Act
            var result = await _sut.UpdateJobAsync(jobId, updateDto);

            // Assert
            result.IsSuccess.Should().BeTrue();

            // Phải xóa 2 mappings cũ
            _mappingRepoMock.Verify(r => r.Delete(It.IsAny<JobControlMapping>()), Times.Exactly(2));

            // Không thêm mapping mới nào
            _mappingRepoMock.Verify(r => r.AddAsync(It.IsAny<JobControlMapping>()), Times.Never);
        }

        [Fact]
        public async Task UpdateJobAsync_ShouldKeepMappingsUnchanged_WhenMappingsFieldIsNull()
        {
            // Arrange
            var jobId = Guid.NewGuid();

            _jobRepoMock
                .Setup(r => r.GetByIdAsync(jobId, QueryType.ActiveOnly))
                .ReturnsAsync(
                    new Job
                    {
                        Id = jobId,
                        Name = "Job",
                        JobTypeId = Guid.NewGuid(),
                        IsActive = true,
                        DefaultState = false,
                    }
                );

            _jobRepoMock
                .Setup(r =>
                    r.FirstOrDefaultAsync(
                        It.IsAny<ISpecification<Job, JobDto>>(),
                        It.IsAny<QueryType>()
                    )
                )
                .ReturnsAsync(BuildJobDto(jobId));

            // Không truyền Mappings → giữ nguyên
            var updateDto = new UpdateJobDto { Name = "Job đã đổi tên" };

            // Act
            var result = await _sut.UpdateJobAsync(jobId, updateDto);

            // Assert
            result.IsSuccess.Should().BeTrue();

            // Không được gọi FindAllAsync hay Delete/AddAsync cho mappings
            _mappingRepoMock.Verify(
                r =>
                    r.FindAllAsync(
                        It.IsAny<Expression<Func<JobControlMapping, bool>>>(),
                        It.IsAny<QueryType>()
                    ),
                Times.Never
            );
            _mappingRepoMock.Verify(r => r.Delete(It.IsAny<JobControlMapping>()), Times.Never);
            _mappingRepoMock.Verify(r => r.AddAsync(It.IsAny<JobControlMapping>()), Times.Never);
        }

        [Fact]
        public async Task UpdateJobAsync_ShouldReturnBadRequest_WhenDuplicateDeviceInNewMappings()
        {
            // Arrange
            var jobId = Guid.NewGuid();
            var deviceId = Guid.NewGuid();

            _jobRepoMock
                .Setup(r => r.GetByIdAsync(jobId, QueryType.ActiveOnly))
                .ReturnsAsync(
                    new Job
                    {
                        Id = jobId,
                        Name = "Job",
                        JobTypeId = Guid.NewGuid(),
                        IsActive = true,
                        DefaultState = false,
                    }
                );

            var updateDto = new UpdateJobDto
            {
                Mappings =
                [
                    new() { ControlDeviceId = deviceId, TargetState = true },
                    new() { ControlDeviceId = deviceId, TargetState = false }, // Trùng
                ],
            };

            // Act
            var result = await _sut.UpdateJobAsync(jobId, updateDto);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Type.Should().Be(ResultType.BadRequest);
            result
                .Message.Should()
                .Be("Danh sách mappings không được có thiết bị điều khiển trùng nhau");
        }

        [Fact]
        public async Task UpdateJobAsync_ShouldReturnNotFound_WhenControlDeviceInNewMappingNotExists()
        {
            // Arrange
            var jobId = Guid.NewGuid();
            var deviceId = Guid.NewGuid();

            _jobRepoMock
                .Setup(r => r.GetByIdAsync(jobId, QueryType.ActiveOnly))
                .ReturnsAsync(
                    new Job
                    {
                        Id = jobId,
                        Name = "Job",
                        JobTypeId = Guid.NewGuid(),
                        IsActive = true,
                        DefaultState = false,
                    }
                );

            _mappingRepoMock
                .Setup(r =>
                    r.FindAllAsync(
                        It.IsAny<Expression<Func<JobControlMapping, bool>>>(),
                        It.IsAny<QueryType>()
                    )
                )
                .ReturnsAsync(new List<JobControlMapping>());

            // Thiết bị không tồn tại — dùng AnyAsync
            _controlDeviceRepoMock
                .Setup(r =>
                    r.AnyAsync(
                        It.IsAny<Expression<Func<ControlDevice, bool>>>(),
                        It.IsAny<QueryType>()
                    )
                )
                .ReturnsAsync(false);

            var updateDto = new UpdateJobDto
            {
                Mappings = [new() { ControlDeviceId = deviceId, TargetState = true }],
            };

            // Act
            var result = await _sut.UpdateJobAsync(jobId, updateDto);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Type.Should().Be(ResultType.NotFound);
            result.Message.Should().Contain(deviceId.ToString());
        }

        [Fact]
        public async Task UpdateJobAsync_ShouldReturnUnexpected_WhenExceptionThrown()
        {
            // Arrange
            var jobId = Guid.NewGuid();

            _jobRepoMock
                .Setup(r => r.GetByIdAsync(jobId, QueryType.ActiveOnly))
                .ThrowsAsync(new Exception());

            // Act
            var result = await _sut.UpdateJobAsync(jobId, new UpdateJobDto());

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Type.Should().Be(ResultType.Unexpected);
            result.Message.Should().Be("Đã xảy ra lỗi khi cập nhật công việc");
        }

        #endregion

        #region DeleteJobAsync Tests

        [Fact]
        public async Task DeleteJobAsync_ShouldReturnSuccess_AndDeleteMappings_WhenJobExists()
        {
            // Arrange
            var jobId = Guid.NewGuid();
            var existingJob = new Job
            {
                Id = jobId,
                Name = "Cho ăn buổi sáng",
                JobTypeId = Guid.NewGuid(),
                IsActive = true,
                DefaultState = false,
            };
            var relatedMappings = new List<JobControlMapping>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    JobId = jobId,
                    ControlDeviceId = Guid.NewGuid(),
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    JobId = jobId,
                    ControlDeviceId = Guid.NewGuid(),
                },
            };

            _jobRepoMock
                .Setup(r => r.GetByIdAsync(jobId, QueryType.ActiveOnly))
                .ReturnsAsync(existingJob);

            _mappingRepoMock
                .Setup(r =>
                    r.FindAllAsync(
                        It.IsAny<Expression<Func<JobControlMapping, bool>>>(),
                        It.IsAny<QueryType>()
                    )
                )
                .ReturnsAsync(relatedMappings);

            // Act
            var result = await _sut.DeleteJobAsync(jobId);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Type.Should().Be(ResultType.Ok);
            result.Message.Should().Be("Xóa công việc thành công");

            // Phải xóa hết 2 mappings liên quan trước
            _mappingRepoMock.Verify(r => r.Delete(It.IsAny<JobControlMapping>()), Times.Exactly(2));

            // Sau đó mới soft-delete job
            _jobRepoMock.Verify(r => r.Delete(It.Is<Job>(j => j.Id == jobId)), Times.Once);

            _unitOfWorkMock.Verify(
                u => u.SaveChangesAsync(It.IsAny<CancellationToken>()),
                Times.Once
            );
        }

        [Fact]
        public async Task DeleteJobAsync_ShouldReturnSuccess_WhenJobHasNoMappings()
        {
            // Arrange
            var jobId = Guid.NewGuid();

            _jobRepoMock
                .Setup(r => r.GetByIdAsync(jobId, QueryType.ActiveOnly))
                .ReturnsAsync(
                    new Job
                    {
                        Id = jobId,
                        Name = "Job trống",
                        JobTypeId = Guid.NewGuid(),
                        IsActive = true,
                        DefaultState = false,
                    }
                );

            // Không có mapping nào
            _mappingRepoMock
                .Setup(r =>
                    r.FindAllAsync(
                        It.IsAny<Expression<Func<JobControlMapping, bool>>>(),
                        It.IsAny<QueryType>()
                    )
                )
                .ReturnsAsync(new List<JobControlMapping>());

            // Act
            var result = await _sut.DeleteJobAsync(jobId);

            // Assert
            result.IsSuccess.Should().BeTrue();

            // Không gọi Delete trên mapping repository
            _mappingRepoMock.Verify(r => r.Delete(It.IsAny<JobControlMapping>()), Times.Never);

            // Vẫn phải soft-delete job
            _jobRepoMock.Verify(r => r.Delete(It.Is<Job>(j => j.Id == jobId)), Times.Once);
        }

        [Fact]
        public async Task DeleteJobAsync_ShouldReturnNotFound_WhenJobDoesNotExist()
        {
            // Arrange
            var jobId = Guid.NewGuid();

            _jobRepoMock
                .Setup(r => r.GetByIdAsync(jobId, QueryType.ActiveOnly))
                .ReturnsAsync((Job?)null);

            // Act
            var result = await _sut.DeleteJobAsync(jobId);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Type.Should().Be(ResultType.NotFound);
            result.Message.Should().Contain(jobId.ToString());

            _mappingRepoMock.Verify(r => r.Delete(It.IsAny<JobControlMapping>()), Times.Never);
            _jobRepoMock.Verify(r => r.Delete(It.IsAny<Job>()), Times.Never);
            _unitOfWorkMock.Verify(
                u => u.SaveChangesAsync(It.IsAny<CancellationToken>()),
                Times.Never
            );
        }

        [Fact]
        public async Task DeleteJobAsync_ShouldReturnUnexpected_WhenExceptionThrown()
        {
            // Arrange
            var jobId = Guid.NewGuid();

            _jobRepoMock
                .Setup(r => r.GetByIdAsync(jobId, QueryType.ActiveOnly))
                .ThrowsAsync(new Exception());

            // Act
            var result = await _sut.DeleteJobAsync(jobId);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Type.Should().Be(ResultType.Unexpected);
            result.Message.Should().Be("Đã xảy ra lỗi khi xóa công việc");

            _unitOfWorkMock.Verify(
                u => u.SaveChangesAsync(It.IsAny<CancellationToken>()),
                Times.Never
            );
        }

        #endregion
    }
}

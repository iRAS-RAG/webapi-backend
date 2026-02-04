using AutoMapper;
using IRasRag.Application.Common.Interfaces.Persistence;
using IRasRag.Application.Common.Models;
using IRasRag.Application.DTOs;
using IRasRag.Application.Services.Interfaces;
using IRasRag.Application.Specifications;
using IRasRag.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace IRasRag.Application.Services.Implementations
{
    public class JobService : IJobService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<JobService> _logger;
        private readonly IMapper _mapper;

        public JobService(IUnitOfWork unitOfWork, ILogger<JobService> logger, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
        }

        #region Get Methods
        public async Task<Result<IEnumerable<JobDto>>> GetAllJobsAsync()
        {
            try
            {
                _logger.LogInformation("Bắt đầu lấy danh sách công việc");

                var jobRepository = _unitOfWork.GetRepository<Job>();
                var spec = new JobDtoListSpec();
                var jobDtos = await jobRepository.ListAsync(spec);

                _logger.LogInformation(
                    "Lấy danh sách công việc thành công: {Count} công việc",
                    jobDtos.Count()
                );

                return Result<IEnumerable<JobDto>>.Success(
                    jobDtos,
                    "Lấy danh sách công việc thành công"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy danh sách công việc");
                return Result<IEnumerable<JobDto>>.Failure(
                    "Đã xảy ra lỗi khi lấy danh sách công việc",
                    ResultType.Unexpected
                );
            }
        }

        public async Task<Result<JobDto>> GetJobByIdAsync(Guid id)
        {
            try
            {
                _logger.LogInformation("Bắt đầu lấy công việc với Id: {Id}", id);

                var jobRepository = _unitOfWork.GetRepository<Job>();
                var job = await jobRepository.GetByIdAsync(id);

                if (job == null)
                {
                    _logger.LogWarning("Không tìm thấy công việc với Id: {Id}", id);
                    return Result<JobDto>.Failure(
                        $"Không tìm thấy công việc với Id: {id}",
                        ResultType.NotFound
                    );
                }

                var jobDto = _mapper.Map<JobDto>(job);
                _logger.LogInformation("Lấy công việc thành công: {Id}", id);

                return Result<JobDto>.Success(jobDto, "Lấy công việc thành công");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy công việc với Id: {Id}", id);
                return Result<JobDto>.Failure(
                    "Đã xảy ra lỗi khi lấy công việc",
                    ResultType.Unexpected
                );
            }
        }
        #endregion

        #region Create Method
        public async Task<Result<JobDto>> CreateJobAsync(CreateJobDto createDto)
        {
            try
            {
                _logger.LogInformation("Bắt đầu tạo công việc mới: {Name}", createDto.Name);

                // Validate Name
                if (string.IsNullOrWhiteSpace(createDto.Name))
                {
                    _logger.LogWarning("Tên công việc không được để trống");
                    return Result<JobDto>.Failure(
                        "Tên công việc là bắt buộc",
                        ResultType.BadRequest
                    );
                }

                var jobRepository = _unitOfWork.GetRepository<Job>();

                // Check if JobType exists
                var jobTypeRepository = _unitOfWork.GetRepository<JobType>();
                var jobType = await jobTypeRepository.GetByIdAsync(createDto.JobTypeId);

                if (jobType == null)
                {
                    _logger.LogWarning(
                        "Không tìm thấy loại công việc với Id: {JobTypeId}",
                        createDto.JobTypeId
                    );
                    return Result<JobDto>.Failure(
                        $"Không tìm thấy loại công việc với Id: {createDto.JobTypeId}",
                        ResultType.NotFound
                    );
                }

                // Check if Sensor exists (if SensorId is provided)
                if (createDto.SensorId.HasValue)
                {
                    var sensorRepository = _unitOfWork.GetRepository<Sensor>();
                    var sensor = await sensorRepository.GetByIdAsync(createDto.SensorId.Value);

                    if (sensor == null)
                    {
                        _logger.LogWarning(
                            "Không tìm thấy cảm biến với Id: {SensorId}",
                            createDto.SensorId.Value
                        );
                        return Result<JobDto>.Failure(
                            $"Không tìm thấy cảm biến với Id: {createDto.SensorId.Value}",
                            ResultType.NotFound
                        );
                    }
                }

                // Validate time range if both StartTime and EndTime are provided
                if (createDto.StartTime.HasValue && createDto.EndTime.HasValue)
                {
                    if (createDto.StartTime.Value >= createDto.EndTime.Value)
                    {
                        _logger.LogWarning("Thời gian bắt đầu phải nhỏ hơn thời gian kết thúc");
                        return Result<JobDto>.Failure(
                            "Thời gian bắt đầu phải nhỏ hơn thời gian kết thúc",
                            ResultType.BadRequest
                        );
                    }
                }

                // Create new Job
                var job = _mapper.Map<Job>(createDto);
                await jobRepository.AddAsync(job);
                await _unitOfWork.SaveChangesAsync();

                var jobDto = _mapper.Map<JobDto>(job);
                _logger.LogInformation("Tạo công việc thành công: {Id}", job.Id);

                return Result<JobDto>.Success(jobDto, "Tạo công việc thành công");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tạo công việc");
                return Result<JobDto>.Failure(
                    "Đã xảy ra lỗi khi tạo công việc",
                    ResultType.Unexpected
                );
            }
        }
        #endregion

        #region Update Method
        public async Task<Result> UpdateJobAsync(Guid id, UpdateJobDto updateDto)
        {
            try
            {
                _logger.LogInformation("Bắt đầu cập nhật công việc: {Id}", id);

                // Check if Job exists
                var jobRepository = _unitOfWork.GetRepository<Job>();
                var job = await jobRepository.GetByIdAsync(id);

                if (job == null)
                {
                    _logger.LogWarning("Không tìm thấy công việc với Id: {Id}", id);
                    return Result.Failure(
                        $"Không tìm thấy công việc với Id: {id}",
                        ResultType.NotFound
                    );
                }

                // Validate and update Name if provided
                if (!string.IsNullOrWhiteSpace(updateDto.Name))
                {
                    job.Name = updateDto.Name.Trim();
                }

                // Update Description
                if (updateDto.Description != null)
                {
                    job.Description = updateDto.Description.Trim();
                }

                // Validate and update JobTypeId if provided
                if (updateDto.JobTypeId.HasValue)
                {
                    var jobTypeRepository = _unitOfWork.GetRepository<JobType>();
                    var jobType = await jobTypeRepository.GetByIdAsync(updateDto.JobTypeId.Value);

                    if (jobType == null)
                    {
                        _logger.LogWarning(
                            "Không tìm thấy loại công việc với Id: {JobTypeId}",
                            updateDto.JobTypeId.Value
                        );
                        return Result.Failure(
                            $"Không tìm thấy loại công việc với Id: {updateDto.JobTypeId.Value}",
                            ResultType.NotFound
                        );
                    }

                    job.JobTypeId = updateDto.JobTypeId.Value;
                }

                // Validate and update SensorId if provided
                if (updateDto.SensorId.HasValue)
                {
                    var sensorRepository = _unitOfWork.GetRepository<Sensor>();
                    var sensor = await sensorRepository.GetByIdAsync(updateDto.SensorId.Value);

                    if (sensor == null)
                    {
                        _logger.LogWarning(
                            "Không tìm thấy cảm biến với Id: {SensorId}",
                            updateDto.SensorId.Value
                        );
                        return Result.Failure(
                            $"Không tìm thấy cảm biến với Id: {updateDto.SensorId.Value}",
                            ResultType.NotFound
                        );
                    }

                    job.SensorId = updateDto.SensorId.Value;
                }

                // Update MinValue and MaxValue
                if (updateDto.MinValue.HasValue)
                {
                    job.MinValue = updateDto.MinValue.Value;
                }

                if (updateDto.MaxValue.HasValue)
                {
                    job.MaxValue = updateDto.MaxValue.Value;
                }

                // Update DefaultState
                if (updateDto.DefaultState.HasValue)
                {
                    job.DefaultState = updateDto.DefaultState.Value;
                }

                // Update IsActive
                if (updateDto.IsActive.HasValue)
                {
                    job.IsActive = updateDto.IsActive.Value;
                }

                // Update StartTime and EndTime
                if (updateDto.StartTime.HasValue)
                {
                    job.StartTime = updateDto.StartTime.Value;
                }

                if (updateDto.EndTime.HasValue)
                {
                    job.EndTime = updateDto.EndTime.Value;
                }

                // Validate time range if both are set
                if (job.StartTime.HasValue && job.EndTime.HasValue)
                {
                    if (job.StartTime.Value >= job.EndTime.Value)
                    {
                        _logger.LogWarning("Thời gian bắt đầu phải nhỏ hơn thời gian kết thúc");
                        return Result.Failure(
                            "Thời gian bắt đầu phải nhỏ hơn thời gian kết thúc",
                            ResultType.BadRequest
                        );
                    }
                }

                // Update RepeatIntervalMinutes
                if (updateDto.RepeatIntervalMinutes.HasValue)
                {
                    job.RepeatIntervalMinutes = updateDto.RepeatIntervalMinutes.Value;
                }

                // Update ExecutionDays
                if (updateDto.ExecutionDays != null)
                {
                    job.ExecutionDays = updateDto.ExecutionDays.Trim();
                }

                jobRepository.Update(job);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Cập nhật công việc thành công: {Id}", id);

                return Result.Success("Cập nhật công việc thành công");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi cập nhật công việc với Id: {Id}", id);
                return Result.Failure(
                    "Đã xảy ra lỗi khi cập nhật công việc",
                    ResultType.Unexpected
                );
            }
        }
        #endregion

        #region Delete Method
        public async Task<Result> DeleteJobAsync(Guid id)
        {
            try
            {
                _logger.LogInformation("Bắt đầu xóa công việc: {Id}", id);

                var jobRepository = _unitOfWork.GetRepository<Job>();
                var job = await jobRepository.GetByIdAsync(id);

                if (job == null)
                {
                    _logger.LogWarning("Không tìm thấy công việc với Id: {Id}", id);
                    return Result.Failure(
                        $"Không tìm thấy công việc với Id: {id}",
                        ResultType.NotFound
                    );
                }

                // Check if Job has related JobControlMappings
                var hasJobControlMappings = await jobRepository.AnyAsync(j =>
                    j.Id == id && j.JobControlMappings.Any()
                );

                if (hasJobControlMappings)
                {
                    _logger.LogWarning(
                        "Không thể xóa công việc {Id} vì đang có liên kết với thiết bị điều khiển",
                        id
                    );
                    return Result.Failure(
                        "Không thể xóa công việc vì đang có liên kết với thiết bị điều khiển",
                        ResultType.Conflict
                    );
                }

                jobRepository.Delete(job);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Xóa công việc thành công: {Id}", id);
                return Result.Success("Xóa công việc thành công");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xóa công việc với Id: {Id}", id);
                return Result.Failure("Đã xảy ra lỗi khi xóa công việc", ResultType.Unexpected);
            }
        }
        #endregion
    }
}

using AutoMapper;
using IRasRag.Application.Common.Interfaces.Persistence;
using IRasRag.Application.Common.Models;
using IRasRag.Application.Common.Models.Pagination;
using IRasRag.Application.Common.Utils;
using IRasRag.Application.DTOs;
using IRasRag.Application.Services.Interfaces;
using IRasRag.Application.Specifications;
using IRasRag.Application.Specifications.JobSpecifications;
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
        public async Task<PaginatedResult<JobDto>> GetAllJobsAsync(JobListRequest request)
        {
            try
            {
                _logger.LogInformation(
                    "Starting to retrieve job list (Page: {Page}, PageSize: {PageSize})",
                    request.Page,
                    request.PageSize
                );

                var repository = _unitOfWork.GetRepository<Job>();
                var spec = new JobDtoListSpec(request);
                var pagedResult = await repository.GetPagedAsync(
                    spec,
                    request.Page,
                    request.PageSize
                );

                var jobDtos = pagedResult.Items;

                _logger.LogInformation(
                    "Successfully retrieved job list: {Count} job(s)",
                    pagedResult.Items.Count
                );

                return new PaginatedResult<JobDto>
                {
                    Message =
                        jobDtos.Count == 0
                            ? "Không có công việc nào"
                            : "Lấy danh sách công việc thành công",
                    Data = jobDtos,
                    Meta = PaginationBuilder.BuildPaginationMetadata(
                        request.Page,
                        request.PageSize,
                        pagedResult.TotalItems
                    ),
                    Links = PaginationBuilder.BuildPaginationLinks(
                        request.Page,
                        request.PageSize,
                        pagedResult.TotalItems
                    ),
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving job list");

                return new PaginatedResult<JobDto>
                {
                    Message = "Đã xảy ra lỗi khi lấy danh sách công việc",
                    Data = Array.Empty<JobDto>(),
                    Meta = null,
                    Links = null,
                };
            }
        }

        public async Task<Result<JobDto>> GetJobByIdAsync(Guid id)
        {
            try
            {
                _logger.LogInformation("Starting to retrieve job with Id: {Id}", id);

                var jobRepository = _unitOfWork.GetRepository<Job>();

                // Dùng spec để projection kèm danh sách Mappings ngay trong query
                var jobDto = await jobRepository.FirstOrDefaultAsync(new JobDtoByIdSpec(id));

                if (jobDto == null)
                {
                    _logger.LogWarning("Job not found with Id: {Id}", id);
                    return Result<JobDto>.Failure(
                        $"Không tìm thấy công việc với Id: {id}",
                        ResultType.NotFound
                    );
                }

                _logger.LogInformation("Successfully retrieved job: {Id}", id);
                return Result<JobDto>.Success(jobDto, "Lấy công việc thành công");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving job with Id: {Id}", id);
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
                _logger.LogInformation("Starting to create new job: {Name}", createDto.Name);

                // 1. Validate tên công việc
                if (string.IsNullOrWhiteSpace(createDto.Name))
                {
                    _logger.LogWarning("Job name cannot be empty");
                    return Result<JobDto>.Failure(
                        "Tên công việc là bắt buộc",
                        ResultType.BadRequest
                    );
                }

                // 2. Kiểm tra loại công việc tồn tại
                var jobTypeRepository = _unitOfWork.GetRepository<JobType>();
                var jobType = await jobTypeRepository.GetByIdAsync(createDto.JobTypeId);

                if (jobType == null)
                {
                    _logger.LogWarning(
                        "JobType not found with Id: {JobTypeId}",
                        createDto.JobTypeId
                    );
                    return Result<JobDto>.Failure(
                        $"Không tìm thấy loại công việc với Id: {createDto.JobTypeId}",
                        ResultType.NotFound
                    );
                }

                // 3. Kiểm tra cảm biến tồn tại (nếu có SensorId)
                if (createDto.SensorId.HasValue)
                {
                    var sensorRepository = _unitOfWork.GetRepository<Sensor>();
                    var sensor = await sensorRepository.GetByIdAsync(createDto.SensorId.Value);

                    if (sensor == null)
                    {
                        _logger.LogWarning(
                            "Sensor not found with Id: {SensorId}",
                            createDto.SensorId.Value
                        );
                        return Result<JobDto>.Failure(
                            $"Không tìm thấy cảm biến với Id: {createDto.SensorId.Value}",
                            ResultType.NotFound
                        );
                    }
                }

                // 4. Validate khoảng thời gian StartTime < EndTime
                if (createDto.StartTime.HasValue && createDto.EndTime.HasValue)
                {
                    if (createDto.StartTime.Value >= createDto.EndTime.Value)
                    {
                        _logger.LogWarning("StartTime must be less than EndTime");
                        return Result<JobDto>.Failure(
                            "Thời gian bắt đầu phải nhỏ hơn thời gian kết thúc",
                            ResultType.BadRequest
                        );
                    }
                }

                // 5. Validate MinValue <= MaxValue (nếu cả hai được cung cấp)
                if (createDto.MinValue.HasValue && createDto.MaxValue.HasValue)
                {
                    if (createDto.MinValue.Value > createDto.MaxValue.Value)
                    {
                        _logger.LogWarning("MinValue cannot be greater than MaxValue");
                        return Result<JobDto>.Failure(
                            "Giá trị min không được lớn hơn giá trị max",
                            ResultType.BadRequest
                        );
                    }
                }

                // 6. Validate danh sách mappings
                if (createDto.Mappings != null && createDto.Mappings.Count > 0)
                {
                    // Kiểm tra trùng ControlDeviceId trong danh sách đầu vào
                    var duplicateDeviceIds = createDto
                        .Mappings.GroupBy(m => m.ControlDeviceId)
                        .Where(g => g.Count() > 1)
                        .Select(g => g.Key)
                        .ToList();

                    if (duplicateDeviceIds.Count > 0)
                    {
                        _logger.LogWarning(
                            "Duplicate ControlDeviceId in mappings: {Ids}",
                            string.Join(", ", duplicateDeviceIds)
                        );
                        return Result<JobDto>.Failure(
                            "Danh sách mappings không được có thiết bị điều khiển trùng nhau",
                            ResultType.BadRequest
                        );
                    }

                    // Kiểm tra từng ControlDevice có tồn tại không
                    var controlDeviceRepository = _unitOfWork.GetRepository<ControlDevice>();
                    foreach (var mappingItem in createDto.Mappings)
                    {
                        var deviceExists = await controlDeviceRepository.AnyAsync(cd =>
                            cd.Id == mappingItem.ControlDeviceId
                        );

                        if (!deviceExists)
                        {
                            _logger.LogWarning(
                                "ControlDevice not found with Id: {ControlDeviceId}",
                                mappingItem.ControlDeviceId
                            );
                            return Result<JobDto>.Failure(
                                $"Không tìm thấy thiết bị điều khiển với Id: {mappingItem.ControlDeviceId}",
                                ResultType.NotFound
                            );
                        }
                    }
                }

                // 7. Bắt đầu transaction - toàn bộ thao tác ghi được thực hiện nguyên tử
                await _unitOfWork.BeginTransactionAsync();

                // 8. Tạo mới Job
                var jobRepository = _unitOfWork.GetRepository<Job>();
                var job = _mapper.Map<Job>(createDto);
                job.Name = createDto.Name.Trim();
                await jobRepository.AddAsync(job);

                // 9. Tạo các JobControlMapping kèm theo (nếu có)
                if (createDto.Mappings != null && createDto.Mappings.Count > 0)
                {
                    var mappingRepository = _unitOfWork.GetRepository<JobControlMapping>();
                    foreach (var mappingItem in createDto.Mappings)
                    {
                        await mappingRepository.AddAsync(
                            new JobControlMapping
                            {
                                JobId = job.Id,
                                ControlDeviceId = mappingItem.ControlDeviceId,
                                TargetState = mappingItem.TargetState,
                                TriggerCondition = mappingItem.TriggerCondition,
                            }
                        );
                    }
                    _logger.LogInformation(
                        "Preparing to create {Count} mapping(s) for job: {JobId}",
                        createDto.Mappings.Count,
                        job.Id
                    );
                }

                // 10. Commit transaction - lưu Job + Mappings trong một lần duy nhất
                await _unitOfWork.CommitTransactionAsync();

                // 11. Lấy lại job với projection đầy đủ (kèm JobTypeName, SensorName, Mappings)
                var jobDto = await jobRepository.FirstOrDefaultAsync(new JobDtoByIdSpec(job.Id));

                _logger.LogInformation("Successfully created job: {Id}", job.Id);
                return Result<JobDto>.Success(jobDto!, "Tạo công việc thành công");
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex, "Error creating job");
                return Result<JobDto>.Failure(
                    "Đã xảy ra lỗi khi tạo công việc",
                    ResultType.Unexpected
                );
            }
        }
        #endregion

        #region Update Method
        public async Task<Result<JobDto>> UpdateJobAsync(Guid id, UpdateJobDto updateDto)
        {
            try
            {
                _logger.LogInformation("Starting to update job: {Id}", id);

                // 1. Kiểm tra công việc tồn tại
                var jobRepository = _unitOfWork.GetRepository<Job>();
                var job = await jobRepository.GetByIdAsync(id);

                if (job == null)
                {
                    _logger.LogWarning("Job not found with Id: {Id}", id);
                    return Result<JobDto>.Failure(
                        $"Không tìm thấy công việc với Id: {id}",
                        ResultType.NotFound
                    );
                }

                // 2. Áp dụng các giá trị cập nhật vào đối tượng in-memory để validate
                if (!string.IsNullOrWhiteSpace(updateDto.Name))
                {
                    job.Name = updateDto.Name.Trim();
                }

                if (updateDto.Description != null)
                {
                    job.Description = updateDto.Description.Trim();
                }

                // 3. Kiểm tra loại công việc mới (nếu có)
                if (updateDto.JobTypeId.HasValue)
                {
                    var jobTypeRepository = _unitOfWork.GetRepository<JobType>();
                    var jobType = await jobTypeRepository.GetByIdAsync(updateDto.JobTypeId.Value);

                    if (jobType == null)
                    {
                        _logger.LogWarning(
                            "JobType not found with Id: {JobTypeId}",
                            updateDto.JobTypeId.Value
                        );
                        return Result<JobDto>.Failure(
                            $"Không tìm thấy loại công việc với Id: {updateDto.JobTypeId.Value}",
                            ResultType.NotFound
                        );
                    }

                    job.JobTypeId = updateDto.JobTypeId.Value;
                }

                // 4. Kiểm tra cảm biến mới (nếu có)
                if (updateDto.SensorId.HasValue)
                {
                    var sensorRepository = _unitOfWork.GetRepository<Sensor>();
                    var sensor = await sensorRepository.GetByIdAsync(updateDto.SensorId.Value);

                    if (sensor == null)
                    {
                        _logger.LogWarning(
                            "Sensor not found with Id: {SensorId}",
                            updateDto.SensorId.Value
                        );
                        return Result<JobDto>.Failure(
                            $"Không tìm thấy cảm biến với Id: {updateDto.SensorId.Value}",
                            ResultType.NotFound
                        );
                    }

                    job.SensorId = updateDto.SensorId.Value;
                }

                // 5. Áp dụng các trường số và trạng thái
                if (updateDto.MinValue.HasValue)
                    job.MinValue = updateDto.MinValue.Value;
                if (updateDto.MaxValue.HasValue)
                    job.MaxValue = updateDto.MaxValue.Value;
                if (updateDto.DefaultState.HasValue)
                    job.DefaultState = updateDto.DefaultState.Value;
                if (updateDto.IsActive.HasValue)
                    job.IsActive = updateDto.IsActive.Value;
                if (updateDto.StartTime.HasValue)
                    job.StartTime = updateDto.StartTime.Value;
                if (updateDto.EndTime.HasValue)
                    job.EndTime = updateDto.EndTime.Value;
                if (updateDto.RepeatIntervalMinutes.HasValue)
                    job.RepeatIntervalMinutes = updateDto.RepeatIntervalMinutes.Value;
                if (updateDto.ExecutionDays != null)
                    job.ExecutionDays = updateDto.ExecutionDays.Trim();

                // 6. Validate khoảng thời gian sau khi áp dụng giá trị mới
                if (job.StartTime.HasValue && job.EndTime.HasValue)
                {
                    if (job.StartTime.Value >= job.EndTime.Value)
                    {
                        _logger.LogWarning("StartTime must be less than EndTime");
                        return Result<JobDto>.Failure(
                            "Thời gian bắt đầu phải nhỏ hơn thời gian kết thúc",
                            ResultType.BadRequest
                        );
                    }
                }

                // 7. Validate MinValue <= MaxValue (dùng giá trị đã được áp dụng)
                if (job.MinValue.HasValue && job.MaxValue.HasValue)
                {
                    if (job.MinValue.Value > job.MaxValue.Value)
                    {
                        _logger.LogWarning("MinValue cannot be greater than MaxValue");
                        return Result<JobDto>.Failure(
                            "Giá trị min không được lớn hơn giá trị max",
                            ResultType.BadRequest
                        );
                    }
                }

                // 8. Validate danh sách mappings TRƯỚC KHI ghi DB
                if (updateDto.Mappings != null)
                {
                    // Kiểm tra trùng ControlDeviceId trong danh sách mới
                    var duplicateDeviceIds = updateDto
                        .Mappings.GroupBy(m => m.ControlDeviceId)
                        .Where(g => g.Count() > 1)
                        .Select(g => g.Key)
                        .ToList();

                    if (duplicateDeviceIds.Count > 0)
                    {
                        _logger.LogWarning(
                            "Duplicate ControlDeviceId in mappings: {Ids}",
                            string.Join(", ", duplicateDeviceIds)
                        );
                        return Result<JobDto>.Failure(
                            "Danh sách mappings không được có thiết bị điều khiển trùng nhau",
                            ResultType.BadRequest
                        );
                    }

                    // Kiểm tra từng ControlDevice có tồn tại không
                    var controlDeviceRepository = _unitOfWork.GetRepository<ControlDevice>();
                    foreach (var mappingItem in updateDto.Mappings)
                    {
                        var deviceExists = await controlDeviceRepository.AnyAsync(cd =>
                            cd.Id == mappingItem.ControlDeviceId
                        );

                        if (!deviceExists)
                        {
                            _logger.LogWarning(
                                "ControlDevice not found with Id: {ControlDeviceId}",
                                mappingItem.ControlDeviceId
                            );
                            return Result<JobDto>.Failure(
                                $"Không tìm thấy thiết bị điều khiển với Id: {mappingItem.ControlDeviceId}",
                                ResultType.NotFound
                            );
                        }
                    }
                }

                // 9. Bắt đầu transaction - toàn bộ thao tác ghi được thực hiện nguyên tử
                await _unitOfWork.BeginTransactionAsync();

                // 10. Lưu cập nhật job
                jobRepository.Update(job);

                // 11. Xử lý cập nhật mappings nếu được cung cấp (chiến lược thay thế toàn bộ)
                if (updateDto.Mappings != null)
                {
                    var mappingRepository = _unitOfWork.GetRepository<JobControlMapping>();

                    // Xóa toàn bộ mappings hiện tại của job này
                    var existingMappings = await mappingRepository.FindAllAsync(m => m.JobId == id);
                    foreach (var existing in existingMappings)
                    {
                        mappingRepository.Delete(existing);
                    }

                    // Thêm danh sách mappings mới
                    foreach (var mappingItem in updateDto.Mappings)
                    {
                        await mappingRepository.AddAsync(
                            new JobControlMapping
                            {
                                JobId = id,
                                ControlDeviceId = mappingItem.ControlDeviceId,
                                TargetState = mappingItem.TargetState,
                                TriggerCondition = mappingItem.TriggerCondition,
                            }
                        );
                    }

                    _logger.LogInformation(
                        "Preparing to update {Count} mapping(s) for job: {JobId}",
                        updateDto.Mappings.Count,
                        id
                    );
                }

                // 12. Commit transaction - lưu Job + Mappings trong một lần duy nhất
                await _unitOfWork.CommitTransactionAsync();

                // 13. Lấy lại job với projection đầy đủ (kèm JobTypeName, SensorName, Mappings)
                var updatedJobDto = await jobRepository.FirstOrDefaultAsync(new JobDtoByIdSpec(id));

                _logger.LogInformation("Successfully updated job: {Id}", id);
                return Result<JobDto>.Success(updatedJobDto!, "Cập nhật công việc thành công");
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex, "Error updating job with Id: {Id}", id);
                return Result<JobDto>.Failure(
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
                _logger.LogInformation("Starting to delete job: {Id}", id);

                var jobRepository = _unitOfWork.GetRepository<Job>();
                var job = await jobRepository.GetByIdAsync(id);

                if (job == null)
                {
                    _logger.LogWarning("Job not found with Id: {Id}", id);
                    return Result.Failure(
                        $"Không tìm thấy công việc với Id: {id}",
                        ResultType.NotFound
                    );
                }

                // Xóa tất cả JobControlMappings liên quan trước khi xóa job.
                var mappingRepository = _unitOfWork.GetRepository<JobControlMapping>();
                var relatedMappings = (
                    await mappingRepository.FindAllAsync(m => m.JobId == id)
                ).ToList();
                foreach (var mapping in relatedMappings)
                {
                    mappingRepository.Delete(mapping);
                }

                // Soft-delete job
                jobRepository.Delete(job);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation(
                    "Successfully deleted job: {Id} (with {Count} mapping(s))",
                    id,
                    relatedMappings.Count
                );
                return Result.Success("Xóa công việc thành công");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting job with Id: {Id}", id);
                return Result.Failure("Đã xảy ra lỗi khi xóa công việc", ResultType.Unexpected);
            }
        }
        #endregion
    }
}

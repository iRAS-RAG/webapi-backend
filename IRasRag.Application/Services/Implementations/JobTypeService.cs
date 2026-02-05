using AutoMapper;
using IRasRag.Application.Common.Interfaces.Persistence;
using IRasRag.Application.Common.Models;
using IRasRag.Application.Common.Models.Pagination;
using IRasRag.Application.Common.Utils;
using IRasRag.Application.DTOs;
using IRasRag.Application.Services.Interfaces;
using IRasRag.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace IRasRag.Application.Services.Implementations
{
    public class JobTypeService : IJobTypeService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<JobTypeService> _logger;
        private readonly IMapper _mapper;

        public JobTypeService(
            IUnitOfWork unitOfWork,
            ILogger<JobTypeService> logger,
            IMapper mapper
        )
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
        }

        #region Get Methods
        public async Task<PaginatedResult<JobTypeDto>> GetAllJobTypesAsync(int page, int pageSize)
        {
            try
            {
                _logger.LogInformation(
                    "Bắt đầu lấy danh sách loại công việc (Page: {Page}, PageSize: {PageSize})",
                    page,
                    pageSize
                );

                var repository = _unitOfWork.GetRepository<JobType>();
                var pagedResult = await repository.GetPagedAsync(page, pageSize);

                var jobTypeDtos = _mapper.Map<IReadOnlyList<JobTypeDto>>(pagedResult.Items);

                _logger.LogInformation(
                    "Lấy danh sách loại công việc thành công: {Count} loại",
                    pagedResult.Items.Count
                );

                return new PaginatedResult<JobTypeDto>
                {
                    Message =
                        jobTypeDtos.Count == 0
                            ? "Không có loại công việc nào"
                            : "Lấy danh sách loại công việc thành công",
                    Data = jobTypeDtos,
                    Meta = PaginationBuilder.BuildPaginationMetadata(
                        page,
                        pageSize,
                        pagedResult.TotalItems
                    ),
                    Links = PaginationBuilder.BuildPaginationLinks(
                        page,
                        pageSize,
                        pagedResult.TotalItems
                    ),
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy danh sách loại công việc");

                return new PaginatedResult<JobTypeDto>
                {
                    Message = "Đã xảy ra lỗi khi lấy danh sách loại công việc",
                    Data = Array.Empty<JobTypeDto>(),
                    Meta = null,
                    Links = null,
                };
            }
        }

        public async Task<Result<JobTypeDto>> GetJobTypeByIdAsync(Guid id)
        {
            try
            {
                _logger.LogInformation("Bắt đầu lấy loại công việc với Id: {Id}", id);

                var jobTypeRepository = _unitOfWork.GetRepository<JobType>();
                var jobType = await jobTypeRepository.GetByIdAsync(id);

                if (jobType == null)
                {
                    _logger.LogWarning("Không tìm thấy loại công việc với Id: {Id}", id);
                    return Result<JobTypeDto>.Failure(
                        $"Không tìm thấy loại công việc với Id: {id}",
                        ResultType.NotFound
                    );
                }

                var jobTypeDto = _mapper.Map<JobTypeDto>(jobType);
                _logger.LogInformation("Lấy loại công việc thành công: {Id}", id);

                return Result<JobTypeDto>.Success(jobTypeDto, "Lấy loại công việc thành công");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy loại công việc với Id: {Id}", id);
                return Result<JobTypeDto>.Failure(
                    "Đã xảy ra lỗi khi lấy loại công việc",
                    ResultType.Unexpected
                );
            }
        }
        #endregion

        #region Create Method
        public async Task<Result<JobTypeDto>> CreateJobTypeAsync(CreateJobTypeDto createDto)
        {
            try
            {
                _logger.LogInformation("Bắt đầu tạo loại công việc mới: {Name}", createDto.Name);

                // Validate Name
                if (string.IsNullOrWhiteSpace(createDto.Name))
                {
                    _logger.LogWarning("Tên loại công việc không được để trống");
                    return Result<JobTypeDto>.Failure(
                        "Tên loại công việc là bắt buộc",
                        ResultType.BadRequest
                    );
                }

                // Check duplicate Name
                var jobTypeRepository = _unitOfWork.GetRepository<JobType>();
                var existingJobType = await jobTypeRepository.FirstOrDefaultAsync(jt =>
                    jt.Name.ToLower() == createDto.Name.ToLower()
                );

                if (existingJobType != null)
                {
                    _logger.LogWarning(
                        "Loại công việc với tên '{Name}' đã tồn tại",
                        createDto.Name
                    );
                    return Result<JobTypeDto>.Failure(
                        $"Loại công việc với tên '{createDto.Name}' đã tồn tại",
                        ResultType.Conflict
                    );
                }

                // Create new JobType
                var jobType = _mapper.Map<JobType>(createDto);
                await jobTypeRepository.AddAsync(jobType);
                await _unitOfWork.SaveChangesAsync();

                var jobTypeDto = _mapper.Map<JobTypeDto>(jobType);
                _logger.LogInformation("Tạo loại công việc thành công: {Id}", jobType.Id);

                return Result<JobTypeDto>.Success(jobTypeDto, "Tạo loại công việc thành công");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tạo loại công việc");
                return Result<JobTypeDto>.Failure(
                    "Đã xảy ra lỗi khi tạo loại công việc",
                    ResultType.Unexpected
                );
            }
        }
        #endregion

        #region Update Method
        public async Task<Result> UpdateJobTypeAsync(Guid id, UpdateJobTypeDto updateDto)
        {
            try
            {
                _logger.LogInformation("Bắt đầu cập nhật loại công việc: {Id}", id);

                // Check if JobType exists
                var jobTypeRepository = _unitOfWork.GetRepository<JobType>();
                var jobType = await jobTypeRepository.GetByIdAsync(id);

                if (jobType == null)
                {
                    _logger.LogWarning("Không tìm thấy loại công việc với Id: {Id}", id);
                    return Result.Failure(
                        $"Không tìm thấy loại công việc với Id: {id}",
                        ResultType.NotFound
                    );
                }

                // Validate and update Name if provided
                if (!string.IsNullOrWhiteSpace(updateDto.Name))
                {
                    // Check duplicate Name (excluding current record)
                    var existingJobType = await jobTypeRepository.FirstOrDefaultAsync(jt =>
                        jt.Name.ToLower() == updateDto.Name.ToLower() && jt.Id != id
                    );

                    if (existingJobType != null)
                    {
                        _logger.LogWarning(
                            "Loại công việc với tên '{Name}' đã tồn tại",
                            updateDto.Name
                        );
                        return Result.Failure(
                            $"Loại công việc với tên '{updateDto.Name}' đã tồn tại",
                            ResultType.Conflict
                        );
                    }

                    jobType.Name = updateDto.Name;
                }

                // Update Description if provided
                if (updateDto.Description != null)
                {
                    jobType.Description = updateDto.Description;
                }

                jobTypeRepository.Update(jobType);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Cập nhật loại công việc thành công: {Id}", id);

                return Result.Success("Cập nhật loại công việc thành công");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi cập nhật loại công việc với Id: {Id}", id);
                return Result.Failure(
                    "Đã xảy ra lỗi khi cập nhật loại công việc",
                    ResultType.Unexpected
                );
            }
        }
        #endregion

        #region Delete Method
        public async Task<Result> DeleteJobTypeAsync(Guid id)
        {
            try
            {
                _logger.LogInformation("Bắt đầu xóa loại công việc: {Id}", id);

                var jobTypeRepository = _unitOfWork.GetRepository<JobType>();
                var jobType = await jobTypeRepository.GetByIdAsync(id);

                if (jobType == null)
                {
                    _logger.LogWarning("Không tìm thấy loại công việc với Id: {Id}", id);
                    return Result.Failure(
                        $"Không tìm thấy loại công việc với Id: {id}",
                        ResultType.NotFound
                    );
                }

                // Check if JobType is being used by any Jobs
                var hasJobs = await jobTypeRepository.AnyAsync(jt => jt.Id == id && jt.Jobs.Any());

                if (hasJobs)
                {
                    _logger.LogWarning(
                        "Không thể xóa loại công việc {Id} vì đang được sử dụng",
                        id
                    );
                    return Result.Failure(
                        "Không thể xóa loại công việc vì đang được sử dụng bởi công việc khác",
                        ResultType.Conflict
                    );
                }

                jobTypeRepository.Delete(jobType);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Xóa loại công việc thành công: {Id}", id);
                return Result.Success("Xóa loại công việc thành công");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xóa loại công việc với Id: {Id}", id);
                return Result.Failure(
                    "Đã xảy ra lỗi khi xóa loại công việc",
                    ResultType.Unexpected
                );
            }
        }
        #endregion
    }
}

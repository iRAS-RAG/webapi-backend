using AutoMapper;
using IRasRag.Application.Common.Interfaces;
using IRasRag.Application.Common.Interfaces.Persistence;
using IRasRag.Application.Common.Models;
using IRasRag.Application.Common.Models.Pagination;
using IRasRag.Application.Common.Utils;
using IRasRag.Application.DTOs;
using IRasRag.Application.Specifications;
using IRasRag.Domain.Entities;
using IRasRag.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace IRasRag.Application.Services.Implementations
{
    public class JobControlMappingService : IJobControlMappingService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<JobControlMappingService> _logger;

        public JobControlMappingService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<JobControlMappingService> logger
        )
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        #region Get Methods
        public async Task<PaginatedResult<JobControlMappingDto>> GetAllJobControlMappingsAsync(
            int page,
            int pageSize
        )
        {
            try
            {
                var spec = new JobControlMappingDtoListSpec();
                var pagedResult = await _unitOfWork
                    .GetRepository<JobControlMapping>()
                    .GetPagedAsync(spec, page, pageSize);

                var meta = PaginationBuilder.BuildPaginationMetadata(
                    page,
                    pageSize,
                    pagedResult.TotalItems
                );

                var links = PaginationBuilder.BuildPaginationLinks(
                    page,
                    pageSize,
                    pagedResult.TotalItems
                );

                return new PaginatedResult<JobControlMappingDto>
                {
                    Message = "Lấy danh sách ánh xạ job-thiết bị điều khiển thành công",
                    Data = pagedResult.Items.ToList(),
                    Meta = meta,
                    Links = links,
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy danh sách ánh xạ job-thiết bị điều khiển");
                return new PaginatedResult<JobControlMappingDto>
                {
                    Message = "Lỗi khi lấy danh sách ánh xạ job-thiết bị điều khiển",
                    Data = new List<JobControlMappingDto>(),
                    Meta = new PaginationMeta(),
                    Links = new PaginationLinks(),
                };
            }
        }

        public async Task<Result<JobControlMappingDto>> GetJobControlMappingByIdAsync(Guid id)
        {
            try
            {
                var mappingRepo = _unitOfWork.GetRepository<JobControlMapping>();
                var mapping = await mappingRepo.FirstOrDefaultAsync(jcm => jcm.Id == id);

                if (mapping == null)
                {
                    return Result<JobControlMappingDto>.Failure(
                        "Không tìm thấy ánh xạ job-thiết bị điều khiển",
                        ResultType.NotFound
                    );
                }

                // Load related entities
                var jobRepo = _unitOfWork.GetRepository<Job>();
                var job = await jobRepo.GetByIdAsync(mapping.JobId);

                var controlDeviceRepo = _unitOfWork.GetRepository<ControlDevice>();
                var controlDevice = await controlDeviceRepo.GetByIdAsync(mapping.ControlDeviceId);

                mapping.Job = job!;
                mapping.ControlDevice = controlDevice!;

                var mappingDto = _mapper.Map<JobControlMappingDto>(mapping);

                return Result<JobControlMappingDto>.Success(
                    mappingDto,
                    "Lấy thông tin ánh xạ job-thiết bị điều khiển thành công"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Lỗi khi lấy thông tin ánh xạ job-thiết bị điều khiển với ID {Id}",
                    id
                );
                return Result<JobControlMappingDto>.Failure(
                    "Lỗi khi lấy thông tin ánh xạ job-thiết bị điều khiển",
                    ResultType.Unexpected
                );
            }
        }
        #endregion

        #region Create Methods
        public async Task<Result<JobControlMappingDto>> CreateJobControlMappingAsync(
            CreateJobControlMappingDto createDto
        )
        {
            try
            {
                // Validate Job exists
                var jobRepo = _unitOfWork.GetRepository<Job>();
                var jobExists = await jobRepo.AnyAsync(j => j.Id == createDto.JobId);
                if (!jobExists)
                {
                    return Result<JobControlMappingDto>.Failure(
                        "Job không tồn tại",
                        ResultType.BadRequest
                    );
                }

                // Validate ControlDevice exists
                var controlDeviceRepo = _unitOfWork.GetRepository<ControlDevice>();
                var controlDeviceExists = await controlDeviceRepo.AnyAsync(cd =>
                    cd.Id == createDto.ControlDeviceId
                );
                if (!controlDeviceExists)
                {
                    return Result<JobControlMappingDto>.Failure(
                        "Thiết bị điều khiển không tồn tại",
                        ResultType.BadRequest
                    );
                }

                // Check if mapping already exists
                var mappingRepo = _unitOfWork.GetRepository<JobControlMapping>();
                var existingMapping = await mappingRepo.AnyAsync(jcm =>
                    jcm.JobId == createDto.JobId && jcm.ControlDeviceId == createDto.ControlDeviceId
                );
                if (existingMapping)
                {
                    return Result<JobControlMappingDto>.Failure(
                        "Ánh xạ giữa Job và Thiết bị điều khiển này đã tồn tại",
                        ResultType.Conflict
                    );
                }

                // Map and create
                var mapping = _mapper.Map<JobControlMapping>(createDto);

                await mappingRepo.AddAsync(mapping);
                await _unitOfWork.SaveChangesAsync();

                // Load related entities for response
                var job = await jobRepo.GetByIdAsync(mapping.JobId);
                var controlDevice = await controlDeviceRepo.GetByIdAsync(mapping.ControlDeviceId);
                mapping.Job = job!;
                mapping.ControlDevice = controlDevice!;

                var mappingDto = _mapper.Map<JobControlMappingDto>(mapping);

                return Result<JobControlMappingDto>.Success(
                    mappingDto,
                    "Tạo ánh xạ job-thiết bị điều khiển thành công"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tạo ánh xạ job-thiết bị điều khiển");
                return Result<JobControlMappingDto>.Failure(
                    "Lỗi khi tạo ánh xạ job-thiết bị điều khiển",
                    ResultType.Unexpected
                );
            }
        }
        #endregion

        #region Update Methods
        public async Task<Result> UpdateJobControlMappingAsync(
            Guid id,
            UpdateJobControlMappingDto updateDto
        )
        {
            try
            {
                var mappingRepo = _unitOfWork.GetRepository<JobControlMapping>();
                var mapping = await mappingRepo.GetByIdAsync(id);

                if (mapping == null)
                {
                    return Result.Failure(
                        "Không tìm thấy ánh xạ job-thiết bị điều khiển",
                        ResultType.NotFound
                    );
                }

                // Map and update
                _mapper.Map(updateDto, mapping);
                mappingRepo.Update(mapping);
                await _unitOfWork.SaveChangesAsync();

                return Result.Success("Cập nhật ánh xạ job-thiết bị điều khiển thành công");
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Lỗi khi cập nhật ánh xạ job-thiết bị điều khiển với ID {Id}",
                    id
                );
                return Result.Failure(
                    "Lỗi khi cập nhật ánh xạ job-thiết bị điều khiển",
                    ResultType.Unexpected
                );
            }
        }
        #endregion

        #region Delete Methods
        public async Task<Result> DeleteJobControlMappingAsync(Guid id)
        {
            try
            {
                var mappingRepo = _unitOfWork.GetRepository<JobControlMapping>();
                var mapping = await mappingRepo.GetByIdAsync(id);

                if (mapping == null)
                {
                    return Result.Failure(
                        "Không tìm thấy ánh xạ job-thiết bị điều khiển",
                        ResultType.NotFound
                    );
                }

                mappingRepo.Delete(mapping);
                await _unitOfWork.SaveChangesAsync();

                return Result.Success("Xóa ánh xạ job-thiết bị điều khiển thành công");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xóa ánh xạ job-thiết bị điều khiển với ID {Id}", id);
                return Result.Failure(
                    "Lỗi khi xóa ánh xạ job-thiết bị điều khiển",
                    ResultType.Unexpected
                );
            }
        }
        #endregion
    }
}

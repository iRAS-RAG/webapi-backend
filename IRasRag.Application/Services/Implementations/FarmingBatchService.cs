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
    public class FarmingBatchService : IFarmingBatchService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<FarmingBatchService> _logger;

        public FarmingBatchService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<FarmingBatchService> logger
        )
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        #region Get Methods
        public async Task<PaginatedResult<FarmingBatchDto>> GetAllFarmingBatchesAsync(
            int page,
            int pageSize
        )
        {
            try
            {
                var spec = new FarmingBatchDtoListSpec();
                var pagedResult = await _unitOfWork
                    .GetRepository<FarmingBatch>()
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

                return new PaginatedResult<FarmingBatchDto>
                {
                    Message = "Lấy danh sách lô nuôi thành công",
                    Data = pagedResult.Items.ToList(),
                    Meta = meta,
                    Links = links,
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy danh sách lô nuôi");
                return new PaginatedResult<FarmingBatchDto>
                {
                    Message = "Lỗi khi lấy danh sách lô nuôi",
                    Data = new List<FarmingBatchDto>(),
                    Meta = new PaginationMeta(),
                    Links = new PaginationLinks(),
                };
            }
        }

        public async Task<Result<FarmingBatchDto>> GetFarmingBatchByIdAsync(Guid id)
        {
            try
            {
                var farmingBatchRepo = _unitOfWork.GetRepository<FarmingBatch>();
                var farmingBatch = await farmingBatchRepo.FirstOrDefaultAsync(fb => fb.Id == id);

                if (farmingBatch == null)
                {
                    return Result<FarmingBatchDto>.Failure(
                        "Không tìm thấy lô nuôi",
                        ResultType.NotFound
                    );
                }

                // Load related entities
                var fishTankRepo = _unitOfWork.GetRepository<FishTank>();
                var fishTank = await fishTankRepo.GetByIdAsync(farmingBatch.FishTankId);

                var speciesRepo = _unitOfWork.GetRepository<Species>();
                var species = await speciesRepo.GetByIdAsync(farmingBatch.SpeciesId);

                farmingBatch.FishTank = fishTank!;
                farmingBatch.Species = species!;

                var farmingBatchDto = _mapper.Map<FarmingBatchDto>(farmingBatch);

                return Result<FarmingBatchDto>.Success(
                    farmingBatchDto,
                    "Lấy thông tin lô nuôi thành công"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy thông tin lô nuôi với ID {Id}", id);
                return Result<FarmingBatchDto>.Failure(
                    "Lỗi khi lấy thông tin lô nuôi",
                    ResultType.Unexpected
                );
            }
        }
        #endregion

        #region Create Methods
        public async Task<Result<FarmingBatchDto>> CreateFarmingBatchAsync(
            CreateFarmingBatchDto createDto
        )
        {
            try
            {
                // Validate FishTank exists
                var fishTankRepo = _unitOfWork.GetRepository<FishTank>();
                var fishTankExists = await fishTankRepo.AnyAsync(ft =>
                    ft.Id == createDto.FishTankId
                );
                if (!fishTankExists)
                {
                    return Result<FarmingBatchDto>.Failure(
                        "Bể cá không tồn tại",
                        ResultType.BadRequest
                    );
                }

                // Validate Species exists
                var speciesRepo = _unitOfWork.GetRepository<Species>();
                var speciesExists = await speciesRepo.AnyAsync(s => s.Id == createDto.SpeciesId);
                if (!speciesExists)
                {
                    return Result<FarmingBatchDto>.Failure(
                        "Loài cá không tồn tại",
                        ResultType.BadRequest
                    );
                }

                // Validate inputs
                var trimmedName = createDto.Name?.Trim() ?? string.Empty;
                if (string.IsNullOrWhiteSpace(trimmedName))
                {
                    return Result<FarmingBatchDto>.Failure(
                        "Tên lô nuôi không được để trống",
                        ResultType.BadRequest
                    );
                }

                var trimmedUnitOfMeasure = createDto.UnitOfMeasure?.Trim() ?? string.Empty;
                if (string.IsNullOrWhiteSpace(trimmedUnitOfMeasure))
                {
                    return Result<FarmingBatchDto>.Failure(
                        "Đơn vị đo không được để trống",
                        ResultType.BadRequest
                    );
                }

                if (createDto.InitialQuantity < 0)
                {
                    return Result<FarmingBatchDto>.Failure(
                        "Số lượng ban đầu phải lớn hơn hoặc bằng 0",
                        ResultType.BadRequest
                    );
                }

                // Map and create
                var farmingBatch = _mapper.Map<FarmingBatch>(createDto);
                farmingBatch.Name = trimmedName;
                farmingBatch.UnitOfMeasure = trimmedUnitOfMeasure;

                var farmingBatchRepo = _unitOfWork.GetRepository<FarmingBatch>();
                await farmingBatchRepo.AddAsync(farmingBatch);
                await _unitOfWork.SaveChangesAsync();

                // Load related entities for response
                var fishTank = await fishTankRepo.GetByIdAsync(farmingBatch.FishTankId);
                var species = await speciesRepo.GetByIdAsync(farmingBatch.SpeciesId);
                farmingBatch.FishTank = fishTank!;
                farmingBatch.Species = species!;

                var farmingBatchDto = _mapper.Map<FarmingBatchDto>(farmingBatch);

                return Result<FarmingBatchDto>.Success(farmingBatchDto, "Tạo lô nuôi thành công");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tạo lô nuôi");
                return Result<FarmingBatchDto>.Failure(
                    "Lỗi khi tạo lô nuôi",
                    ResultType.Unexpected
                );
            }
        }
        #endregion

        #region Update Methods
        public async Task<Result> UpdateFarmingBatchAsync(Guid id, UpdateFarmingBatchDto updateDto)
        {
            try
            {
                var farmingBatchRepo = _unitOfWork.GetRepository<FarmingBatch>();
                var farmingBatch = await farmingBatchRepo.GetByIdAsync(id);

                if (farmingBatch == null)
                {
                    return Result.Failure("Không tìm thấy lô nuôi", ResultType.NotFound);
                }

                // Validate inputs
                if (updateDto.Name != null)
                {
                    var trimmedName = updateDto.Name.Trim();
                    if (string.IsNullOrWhiteSpace(trimmedName))
                    {
                        return Result.Failure(
                            "Tên lô nuôi không được để trống",
                            ResultType.BadRequest
                        );
                    }
                    updateDto.Name = trimmedName;
                }

                if (updateDto.UnitOfMeasure != null)
                {
                    var trimmedUnitOfMeasure = updateDto.UnitOfMeasure.Trim();
                    if (string.IsNullOrWhiteSpace(trimmedUnitOfMeasure))
                    {
                        return Result.Failure(
                            "Đơn vị đo không được để trống",
                            ResultType.BadRequest
                        );
                    }
                    updateDto.UnitOfMeasure = trimmedUnitOfMeasure;
                }

                if (updateDto.CurrentQuantity.HasValue && updateDto.CurrentQuantity.Value < 0)
                {
                    return Result.Failure(
                        "Số lượng hiện tại phải lớn hơn hoặc bằng 0",
                        ResultType.BadRequest
                    );
                }

                // Map and update
                _mapper.Map(updateDto, farmingBatch);
                farmingBatchRepo.Update(farmingBatch);
                await _unitOfWork.SaveChangesAsync();

                return Result.Success("Cập nhật lô nuôi thành công");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi cập nhật lô nuôi với ID {Id}", id);
                return Result.Failure("Lỗi khi cập nhật lô nuôi", ResultType.Unexpected);
            }
        }
        #endregion

        #region Delete Methods
        public async Task<Result> DeleteFarmingBatchAsync(Guid id)
        {
            try
            {
                var farmingBatchRepo = _unitOfWork.GetRepository<FarmingBatch>();
                var farmingBatch = await farmingBatchRepo.GetByIdAsync(id);

                if (farmingBatch == null)
                {
                    return Result.Failure("Không tìm thấy lô nuôi", ResultType.NotFound);
                }

                // Check if there are related FeedingLogs
                var feedingLogRepo = _unitOfWork.GetRepository<FeedingLog>();
                var hasFeedingLogs = await feedingLogRepo.AnyAsync(fl => fl.FarmingBatchId == id);
                if (hasFeedingLogs)
                {
                    return Result.Failure(
                        "Không thể xóa lô nuôi vì có dữ liệu nhật ký cho ăn liên quan",
                        ResultType.BadRequest
                    );
                }

                // Check if there are related MortalityLogs
                var mortalityLogRepo = _unitOfWork.GetRepository<MortalityLog>();
                var hasMortalityLogs = await mortalityLogRepo.AnyAsync(ml => ml.BatchId == id);
                if (hasMortalityLogs)
                {
                    return Result.Failure(
                        "Không thể xóa lô nuôi vì có dữ liệu nhật ký chết liên quan",
                        ResultType.BadRequest
                    );
                }

                // Check if there are related Alerts
                var alertRepo = _unitOfWork.GetRepository<Alert>();
                var hasAlerts = await alertRepo.AnyAsync(a => a.FarmingBatchId == id);
                if (hasAlerts)
                {
                    return Result.Failure(
                        "Không thể xóa lô nuôi vì có cảnh báo liên quan",
                        ResultType.BadRequest
                    );
                }

                farmingBatchRepo.Delete(farmingBatch);
                await _unitOfWork.SaveChangesAsync();

                return Result.Success("Xóa lô nuôi thành công");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xóa lô nuôi với ID {Id}", id);
                return Result.Failure("Lỗi khi xóa lô nuôi", ResultType.Unexpected);
            }
        }
        #endregion
    }
}

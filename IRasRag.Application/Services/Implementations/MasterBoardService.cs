using AutoMapper;
using IRasRag.Application.Common.Constants;
using IRasRag.Application.Common.Interfaces.Auth;
using IRasRag.Application.Common.Interfaces.Persistence;
using IRasRag.Application.Common.Interfaces.Telemetry;
using IRasRag.Application.Common.Models;
using IRasRag.Application.Common.Models.Pagination;
using IRasRag.Application.Common.Utils;
using IRasRag.Application.DTOs;
using IRasRag.Application.Services.Interfaces;
using IRasRag.Application.Specifications.MasterBoardSpecifications;
using IRasRag.Domain.Entities;
using IRasRag.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace IRasRag.Application.Services.Implementations
{
    public class MasterBoardService : IMasterBoardService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<MasterBoardService> _logger;
        private readonly IMapper _mapper;
        private readonly ITelemetryCacheService _telemetryCache;
        private readonly IAuditLogService _auditLogService;
        private readonly ICurrentUserAccessor _currentUserAccessor;

        public MasterBoardService(
            IUnitOfWork unitOfWork,
            ILogger<MasterBoardService> logger,
            IMapper mapper,
            ITelemetryCacheService telemetryCache,
            IAuditLogService auditLogService,
            ICurrentUserAccessor currentUserAccessor
        )
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
            _telemetryCache = telemetryCache;
            _auditLogService = auditLogService;
            _currentUserAccessor = currentUserAccessor;
        }

        #region Get Methods
        public async Task<PaginatedResult<MasterBoardDto>> GetAllMasterBoardsAsync(
            MasterBoardListRequest request
        )
        {
            try
            {
                _logger.LogInformation(
                    "Bắt đầu lấy danh sách bảng mạch (Page: {Page}, PageSize: {PageSize})",
                    request.Page,
                    request.PageSize
                );

                var repository = _unitOfWork.GetRepository<MasterBoard>();
                var pagedResult = await repository.GetPagedAsync(
                    new MasterBoardDtoListSpec(request),
                    request.Page,
                    request.PageSize
                );

                _logger.LogInformation(
                    "Lấy danh sách bảng mạch thành công: {Count} bảng mạch",
                    pagedResult.Items.Count
                );

                return new PaginatedResult<MasterBoardDto>
                {
                    Message =
                        pagedResult.Items.Count == 0
                            ? "Không có bảng mạch nào"
                            : "Lấy danh sách bảng mạch thành công",
                    Data = pagedResult.Items,
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
                _logger.LogError(ex, "Lỗi khi lấy danh sách bảng mạch");

                return new PaginatedResult<MasterBoardDto>
                {
                    Message = "Đã xảy ra lỗi khi lấy danh sách bảng mạch",
                    Data = Array.Empty<MasterBoardDto>(),
                    Meta = null,
                    Links = null,
                };
            }
        }

        public async Task<Result<MasterBoardDto>> GetMasterBoardByIdAsync(Guid id)
        {
            try
            {
                _logger.LogInformation("Bắt đầu lấy bảng mạch với Id: {Id}", id);

                var masterBoardRepository = _unitOfWork.GetRepository<MasterBoard>();
                var masterBoardDto = await masterBoardRepository.FirstOrDefaultAsync(
                    new MasterBoardDtoByIdSpec(id)
                );

                if (masterBoardDto == null)
                {
                    _logger.LogWarning("Không tìm thấy bảng mạch với Id: {Id}", id);
                    return Result<MasterBoardDto>.Failure(
                        $"Không tìm thấy bảng mạch với Id: {id}",
                        ResultType.NotFound
                    );
                }

                _logger.LogInformation("Lấy bảng mạch thành công: {Id}", id);

                return Result<MasterBoardDto>.Success(masterBoardDto, "Lấy bảng mạch thành công");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy bảng mạch với Id: {Id}", id);
                return Result<MasterBoardDto>.Failure(
                    "Đã xảy ra lỗi khi lấy bảng mạch",
                    ResultType.Unexpected
                );
            }
        }
        #endregion

        #region Create Method
        public async Task<Result<MasterBoardDto>> CreateMasterBoardAsync(
            CreateMasterBoardDto createDto
        )
        {
            try
            {
                _logger.LogInformation("Bắt đầu tạo bảng mạch mới: {Name}", createDto.Name);

                // Check duplicate MacAddress
                var masterBoardRepository = _unitOfWork.GetRepository<MasterBoard>();
                var existingMasterBoard = await masterBoardRepository.FirstOrDefaultAsync(mb =>
                    mb.MacAddress.ToLower() == createDto.MacAddress.ToLower()
                );

                if (existingMasterBoard != null)
                {
                    _logger.LogWarning(
                        "Bảng mạch với địa chỉ MAC '{MacAddress}' đã tồn tại",
                        createDto.MacAddress
                    );
                    return Result<MasterBoardDto>.Failure(
                        $"Bảng mạch với địa chỉ MAC '{createDto.MacAddress}' đã tồn tại",
                        ResultType.Conflict
                    );
                }

                // Check if FishTank exists
                var fishTankRepository = _unitOfWork.GetRepository<FishTank>();
                var fishTank = await fishTankRepository.GetByIdAsync(createDto.FishTankId);

                if (fishTank == null)
                {
                    _logger.LogWarning(
                        "Không tìm thấy hồ cá với Id: {FishTankId}",
                        createDto.FishTankId
                    );
                    return Result<MasterBoardDto>.Failure(
                        $"Không tìm thấy hồ cá với Id: {createDto.FishTankId}",
                        ResultType.NotFound
                    );
                }

                // Create new MasterBoard
                var masterBoard = _mapper.Map<MasterBoard>(createDto);
                await masterBoardRepository.AddAsync(masterBoard);
                await _unitOfWork.SaveChangesAsync();

                await WriteCreateAuditLogAsync(masterBoard, fishTank.Name);

                var masterBoardDto = _mapper.Map<MasterBoardDto>(masterBoard);
                masterBoardDto.FishTankName = fishTank.Name;
                _logger.LogInformation("Tạo bảng mạch thành công: {Id}", masterBoard.Id);

                return Result<MasterBoardDto>.Success(masterBoardDto, "Tạo bảng mạch thành công");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tạo bảng mạch");
                return Result<MasterBoardDto>.Failure(
                    "Đã xảy ra lỗi khi tạo bảng mạch",
                    ResultType.Unexpected
                );
            }
        }
        #endregion

        #region Update Method
        public async Task<Result> UpdateMasterBoardAsync(Guid id, UpdateMasterBoardDto updateDto)
        {
            try
            {
                _logger.LogInformation("Bắt đầu cập nhật bảng mạch: {Id}", id);

                // Check if MasterBoard exists
                var masterBoardRepository = _unitOfWork.GetRepository<MasterBoard>();
                var masterBoard = await masterBoardRepository.GetByIdAsync(id);

                if (masterBoard == null)
                {
                    _logger.LogWarning("Không tìm thấy bảng mạch với Id: {Id}", id);
                    return Result.Failure(
                        $"Không tìm thấy bảng mạch với Id: {id}",
                        ResultType.NotFound
                    );
                }

                var originalFishTank = await _unitOfWork
                    .GetRepository<FishTank>()
                    .GetByIdAsync(masterBoard.FishTankId);
                var oldSnapshot = new
                {
                    masterBoard.Name,
                    masterBoard.MacAddress,
                    FishTankName = originalFishTank?.Name ?? "Unknown",
                };

                var updatedFishTankName = originalFishTank?.Name ?? "Unknown";

                // Validate and update Name if provided
                if (!string.IsNullOrWhiteSpace(updateDto.Name))
                {
                    masterBoard.Name = updateDto.Name.Trim();
                }

                // Validate and update MacAddress if provided
                if (!string.IsNullOrWhiteSpace(updateDto.MacAddress))
                {
                    // Check duplicate MacAddress (excluding current record)
                    var existingMasterBoard = await masterBoardRepository.FirstOrDefaultAsync(mb =>
                        mb.MacAddress.ToLower() == updateDto.MacAddress.ToLower() && mb.Id != id
                    );

                    if (existingMasterBoard != null)
                    {
                        _logger.LogWarning(
                            "Bảng mạch với địa chỉ MAC '{MacAddress}' đã tồn tại",
                            updateDto.MacAddress
                        );
                        return Result.Failure(
                            $"Bảng mạch với địa chỉ MAC '{updateDto.MacAddress}' đã tồn tại",
                            ResultType.Conflict
                        );
                    }

                    _telemetryCache.InvalidateMasterboard(masterBoard.MacAddress);
                    masterBoard.MacAddress = updateDto.MacAddress.Trim();
                }

                // Validate and update FishTankId if provided
                if (updateDto.FishTankId.HasValue)
                {
                    var fishTankRepository = _unitOfWork.GetRepository<FishTank>();
                    var fishTank = await fishTankRepository.GetByIdAsync(
                        updateDto.FishTankId.Value
                    );

                    if (fishTank == null)
                    {
                        _logger.LogWarning(
                            "Không tìm thấy hồ cá với Id: {FishTankId}",
                            updateDto.FishTankId.Value
                        );
                        return Result.Failure(
                            $"Không tìm thấy hồ cá với Id: {updateDto.FishTankId.Value}",
                            ResultType.NotFound
                        );
                    }

                    masterBoard.FishTankId = updateDto.FishTankId.Value;
                    updatedFishTankName = fishTank.Name;
                }

                masterBoardRepository.Update(masterBoard);
                await _unitOfWork.SaveChangesAsync();

                await WriteUpdateAuditLogAsync(masterBoard, oldSnapshot, updatedFishTankName);

                _logger.LogInformation("Cập nhật bảng mạch thành công: {Id}", id);

                return Result.Success("Cập nhật bảng mạch thành công");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi cập nhật bảng mạch với Id: {Id}", id);
                return Result.Failure(
                    "Đã xảy ra lỗi khi cập nhật bảng mạch",
                    ResultType.Unexpected
                );
            }
        }
        #endregion

        #region Delete Method
        public async Task<Result> DeleteMasterBoardAsync(Guid id)
        {
            try
            {
                _logger.LogInformation("Bắt đầu xóa bảng mạch: {Id}", id);

                var masterBoardRepository = _unitOfWork.GetRepository<MasterBoard>();
                var masterBoard = await masterBoardRepository.GetByIdAsync(id);

                if (masterBoard == null)
                {
                    _logger.LogWarning("Không tìm thấy bảng mạch với Id: {Id}", id);
                    return Result.Failure(
                        $"Không tìm thấy bảng mạch với Id: {id}",
                        ResultType.NotFound
                    );
                }

                var fishTank = await _unitOfWork
                    .GetRepository<FishTank>()
                    .GetByIdAsync(masterBoard.FishTankId);
                var oldSnapshot = new
                {
                    masterBoard.Name,
                    masterBoard.MacAddress,
                    FishTankName = fishTank?.Name ?? "Unknown",
                };

                // Check if MasterBoard has related Sensors
                var hasSensors = await masterBoardRepository.AnyAsync(mb =>
                    mb.Id == id && mb.Sensors.Any()
                );

                if (hasSensors)
                {
                    _logger.LogWarning(
                        "Không thể xóa bảng mạch {Id} vì đang có cảm biến liên kết",
                        id
                    );
                    return Result.Failure(
                        "Không thể xóa bảng mạch vì đang có cảm biến liên kết",
                        ResultType.Conflict
                    );
                }

                // Check if MasterBoard has related ControlDevices
                var hasControlDevices = await masterBoardRepository.AnyAsync(mb =>
                    mb.Id == id && mb.ControlDevices.Any()
                );

                if (hasControlDevices)
                {
                    _logger.LogWarning(
                        "Không thể xóa bảng mạch {Id} vì đang có thiết bị điều khiển liên kết",
                        id
                    );
                    return Result.Failure(
                        "Không thể xóa bảng mạch vì đang có thiết bị điều khiển liên kết",
                        ResultType.Conflict
                    );
                }

                masterBoardRepository.Delete(masterBoard);
                await _unitOfWork.SaveChangesAsync();
                _telemetryCache.InvalidateMasterboard(masterBoard.MacAddress);
                _telemetryCache.InvalidateSensors(masterBoard.Id);

                await WriteDeleteAuditLogAsync(masterBoard.Id, oldSnapshot);

                _logger.LogInformation("Xóa bảng mạch thành công: {Id}", id);
                return Result.Success("Xóa bảng mạch thành công");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xóa bảng mạch với Id: {Id}", id);
                return Result.Failure("Đã xảy ra lỗi khi xóa bảng mạch", ResultType.Unexpected);
            }
        }
        #endregion

        #region Audit Log Helpers
        private async Task<User?> GetAuditActorAsync(string operation)
        {
            var currentUserId = _currentUserAccessor.GetUserId();
            if (currentUserId is null)
            {
                _logger.LogDebug(
                    "Skipping {Operation} audit entry because no authenticated user was found.",
                    operation
                );
                return null;
            }

            var actor = await _unitOfWork
                .GetRepository<User>()
                .FirstOrDefaultAsync(u => u.Id == currentUserId.Value, QueryType.IncludeDeleted);

            if (actor == null)
            {
                _logger.LogWarning(
                    "Skipping {Operation} audit entry because the current user {UserId} could not be resolved.",
                    operation,
                    currentUserId.Value
                );
            }

            return actor;
        }

        private async Task WriteAuditLogAsync(
            string action,
            string entityId,
            object? oldValue,
            object? newValue,
            string operation
        )
        {
            try
            {
                var actor = await GetAuditActorAsync(operation);
                if (actor == null)
                    return;

                await _auditLogService.AddAsync(
                    AuditLogHelper.Create(
                        actor,
                        action,
                        nameof(MasterBoard),
                        entityId,
                        oldValue,
                        newValue
                    )
                );

                await _unitOfWork.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Failed to write {Operation} audit entry for {EntityType} {EntityId}",
                    operation,
                    nameof(MasterBoard),
                    entityId
                );
            }
        }

        private async Task WriteCreateAuditLogAsync(MasterBoard masterBoard, string fishTankName)
        {
            await WriteAuditLogAsync(
                AuditLogActions.Create,
                masterBoard.Id.ToString(),
                null,
                new
                {
                    masterBoard.Name,
                    masterBoard.MacAddress,
                    FishTankName = fishTankName,
                },
                "create-master-board"
            );
        }

        private async Task WriteUpdateAuditLogAsync(
            MasterBoard masterBoard,
            object oldSnapshot,
            string fishTankName
        )
        {
            await WriteAuditLogAsync(
                AuditLogActions.Update,
                masterBoard.Id.ToString(),
                oldSnapshot,
                new
                {
                    masterBoard.Name,
                    masterBoard.MacAddress,
                    FishTankName = fishTankName,
                },
                "update-master-board"
            );
        }

        private async Task WriteDeleteAuditLogAsync(Guid id, object oldSnapshot)
        {
            await WriteAuditLogAsync(
                AuditLogActions.Delete,
                id.ToString(),
                oldSnapshot,
                null,
                "delete-master-board"
            );
        }
        #endregion
    }
}

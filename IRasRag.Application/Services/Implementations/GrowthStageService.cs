using AutoMapper;
using IRasRag.Application.Common.Interfaces.Persistence;
using IRasRag.Application.Common.Interfaces.Auth;
using IRasRag.Application.Common.Models;
using IRasRag.Application.Common.Models.Pagination;
using IRasRag.Application.Common.Utils;
using IRasRag.Application.Common.Constants;
using IRasRag.Application.DTOs;
using IRasRag.Application.Services.Interfaces;
using IRasRag.Application.Specifications.GrowthStageSpecifications;
using IRasRag.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace IRasRag.Application.Services.Implementations
{
    public class GrowthStageService : IGrowthStageService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<GrowthStageService> _logger;
        private readonly IMapper _mapper;
        private readonly IAuditLogService? _auditLogService;
        private readonly ICurrentUserAccessor? _currentUserAccessor;

        public GrowthStageService(
            IUnitOfWork unitOfWork,
            ILogger<GrowthStageService> logger,
            IMapper mapper,
            IAuditLogService? auditLogService = null,
            ICurrentUserAccessor? currentUserAccessor = null
        )
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
            _auditLogService = auditLogService;
            _currentUserAccessor = currentUserAccessor;
        }



        public async Task<PaginatedResult<GrowthStageDto>> GetGrowthStagesBySpeciesIdAsync(
            Guid speciesId,
            GrowthStageListRequest request
        )
        {
            try
            {
                var speciesExists = await _unitOfWork
                    .GetRepository<Species>()
                    .AnyAsync(s => s.Id == speciesId);
                if (!speciesExists)
                    return new PaginatedResult<GrowthStageDto>
                    {
                        Message = "Loài cá không tồn tại",
                        Data = Array.Empty<GrowthStageDto>(),
                        Meta = null,
                        Links = null,
                    };

                var repo = _unitOfWork.GetRepository<GrowthStage>();
                var pagedResult = await repo.GetPagedAsync(
                    new GrowthStageBySpeciesIdSpec(speciesId, request),
                    request.Page,
                    request.PageSize
                );

                return new PaginatedResult<GrowthStageDto>
                {
                    Message =
                        pagedResult.Items.Count == 0
                            ? "Không có giai đoạn sinh trưởng cho loài này"
                            : "Lấy giai đoạn sinh trưởng cho loài thành công",
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
                _logger.LogError(ex, "Error retrieving growth stages by species id");
                return new PaginatedResult<GrowthStageDto>
                {
                    Message = "Lỗi khi truy xuất giai đoạn sinh trưởng",
                    Data = Array.Empty<GrowthStageDto>(),
                    Meta = null,
                    Links = null,
                };
            }
        }

        public async Task<Result<GrowthStageDto>> CreateGrowthStageAsync(
            CreateGrowthStageDto createDto
        )
        {
            try
            {
                if (string.IsNullOrWhiteSpace(createDto.Name))
                    return Result<GrowthStageDto>.Failure(
                        "Tên giai đoạn sinh trưởng không được để trống.",
                        ResultType.BadRequest
                    );

                if (string.IsNullOrWhiteSpace(createDto.Description))
                    return Result<GrowthStageDto>.Failure(
                        "Mô tả giai đoạn sinh trưởng không được để trống.",
                        ResultType.BadRequest
                    );

                // ensure species exists
                var speciesExists = await _unitOfWork
                    .GetRepository<Species>()
                    .AnyAsync(s => s.Id == createDto.SpeciesId);
                if (!speciesExists)
                    return Result<GrowthStageDto>.Failure(
                        "Loài cá không tồn tại.",
                        ResultType.BadRequest
                    );

                // ensure unique name per species
                var nameExists = await _unitOfWork
                    .GetRepository<GrowthStage>()
                    .AnyAsync(gs =>
                        gs.SpeciesId == createDto.SpeciesId && gs.Name == createDto.Name.Trim()
                    );
                if (nameExists)
                    return Result<GrowthStageDto>.Failure(
                        "Giai đoạn với tên này đã tồn tại cho loài cá.",
                        ResultType.Conflict
                    );

                var newGrowthStage = new GrowthStage
                {
                    Name = createDto.Name.Trim(),
                    Description = createDto.Description.Trim(),
                    SpeciesId = createDto.SpeciesId,
                };

                await _unitOfWork.GetRepository<GrowthStage>().AddAsync(newGrowthStage);
                await _unitOfWork.SaveChangesAsync();

                var dto = _mapper.Map<GrowthStageDto>(newGrowthStage);
                // Audit: creation
                await WriteCreateAuditLogAsync(newGrowthStage);

                return Result<GrowthStageDto>.Success(dto, "Tạo giai đoạn sinh trưởng thành công.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating growth stage");
                return Result<GrowthStageDto>.Failure(
                    "Lỗi khi tạo giai đoạn sinh trưởng.",
                    ResultType.Unexpected
                );
            }
        }

        public async Task<Result> DeleteGrowthStageAsync(Guid id)
        {
            try
            {
                var growthStage = await _unitOfWork.GetRepository<GrowthStage>().GetByIdAsync(id);

                if (growthStage == null)
                {
                    return Result.Failure(
                        "Giai đoạn sinh trưởng không tồn tại.",
                        ResultType.NotFound
                    );
                }

                _unitOfWork.GetRepository<GrowthStage>().Delete(growthStage);
                // capture old value for audit
                var oldValue = new
                {
                    growthStage.Id,
                    growthStage.Name,
                    growthStage.Description,
                    growthStage.SpeciesId,
                };

                await _unitOfWork.SaveChangesAsync();

                // Audit: delete
                await WriteDeleteAuditLogAsync(growthStage.Id, oldValue);

                return Result.Success("Xóa giai đoạn sinh trưởng thành công.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting growth stage");
                return Result.Failure("Lỗi khi xóa giai đoạn sinh trưởng.", ResultType.Unexpected);
            }
        }

        public async Task<PaginatedResult<GrowthStageDto>> GetAllGrowthStagesAsync(
            GrowthStageListRequest request
        )
        {
            try
            {
                var repository = _unitOfWork.GetRepository<GrowthStage>();
                var spec = new GrowthStageDtoListSpec(request);
                var pagedResult = await repository.GetPagedAsync(
                    spec,
                    request.Page,
                    request.PageSize
                );

                var growthStageDtos = pagedResult.Items;

                return new PaginatedResult<GrowthStageDto>
                {
                    Message =
                        growthStageDtos.Count == 0
                            ? "Không có giai đoạn sinh trưởng nào"
                            : "Lấy danh sách giai đoạn sinh trưởng thành công.",
                    Data = growthStageDtos,
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
                _logger.LogError(ex, "Error retrieving all growth stages");

                return new PaginatedResult<GrowthStageDto>
                {
                    Message = "Lỗi khi truy xuất danh sách giai đoạn sinh trưởng.",
                    Data = Array.Empty<GrowthStageDto>(),
                    Meta = null,
                    Links = null,
                };
            }
        }

        public async Task<Result<GrowthStageDto>> GetGrowthStageByIdAsync(Guid id)
        {
            try
            {
                var growthStage = await _unitOfWork.GetRepository<GrowthStage>().GetByIdAsync(id);

                if (growthStage == null)
                    return Result<GrowthStageDto>.Failure(
                        "Giai đoạn sinh trưởng không tồn tại.",
                        ResultType.NotFound
                    );

                var dto = new GrowthStageDto
                {
                    Id = growthStage.Id,
                    Name = growthStage.Name,
                    Description = growthStage.Description,
                    SpeciesId = growthStage.SpeciesId,
                };

                // populate species name
                var species = await _unitOfWork
                    .GetRepository<Species>()
                    .GetByIdAsync(growthStage.SpeciesId);
                dto.SpeciesName = species?.Name ?? string.Empty;

                return Result<GrowthStageDto>.Success(dto, "Lấy giai đoạn sinh trưởng thành công.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving growth stage by ID");
                return Result<GrowthStageDto>.Failure(
                    "Lỗi khi truy xuất giai đoạn sinh trưởng.",
                    ResultType.Unexpected
                );
            }
        }

        public async Task<Result> UpdateGrowthStageAsync(Guid id, UpdateGrowthStageDto dto)
        {
            try
            {
                var nameToUpdate = string.IsNullOrWhiteSpace(dto.Name) ? null : dto.Name;

                var descriptionToUpdate = string.IsNullOrWhiteSpace(dto.Description)
                    ? null
                    : dto.Description;

                var growthStage = await _unitOfWork.GetRepository<GrowthStage>().GetByIdAsync(id);

                if (growthStage == null)
                    return Result.Failure(
                        "Giai đoạn sinh trưởng không tồn tại.",
                        ResultType.NotFound
                    );

                // snapshot old value for audit
                var oldValue = new
                {
                    growthStage.Id,
                    growthStage.Name,
                    growthStage.Description,
                    growthStage.SpeciesId,
                };

                if (!string.IsNullOrWhiteSpace(nameToUpdate))
                {
                    growthStage.Name = nameToUpdate.Trim();
                }

                if (!string.IsNullOrWhiteSpace(descriptionToUpdate))
                {
                    growthStage.Description = descriptionToUpdate.Trim();
                }

                if (dto.SpeciesId != null && dto.SpeciesId != Guid.Empty)
                {
                    var speciesExists = await _unitOfWork
                        .GetRepository<Species>()
                        .AnyAsync(s => s.Id == dto.SpeciesId.Value);
                    if (!speciesExists)
                        return Result.Failure("Loài cá không tồn tại.", ResultType.BadRequest);

                    // ensure unique name per species if name was updated
                    if (!string.IsNullOrWhiteSpace(nameToUpdate))
                    {
                        var nameExists = await _unitOfWork
                            .GetRepository<GrowthStage>()
                            .AnyAsync(gs =>
                                gs.SpeciesId == dto.SpeciesId.Value
                                && gs.Name == nameToUpdate.Trim()
                                && gs.Id != id
                            );
                        if (nameExists)
                            return Result.Failure(
                                "Giai đoạn với tên này đã tồn tại cho loài cá.",
                                ResultType.Conflict
                            );
                    }

                    growthStage.SpeciesId = dto.SpeciesId.Value;
                }

                await _unitOfWork.SaveChangesAsync();

                // Audit: update
                await WriteUpdateAuditLogAsync(growthStage, oldValue);

                return Result.Success("Cập nhật giai đoạn sinh trưởng thành công.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating growth stage");
                return Result.Failure(
                    "Lỗi khi cập nhật giai đoạn sinh trưởng.",
                    ResultType.Unexpected
                );
            }
        }
        #region Private Helper Methods for Audit Logging
        private async Task<User?> GetAuditActorAsync(string operation)
        {
            if (_currentUserAccessor == null)
            {
                _logger.LogDebug(
                    "Skipping {Operation} audit entry because no ICurrentUserAccessor is configured.",
                    operation
                );
                return null;
            }

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
                .FirstOrDefaultAsync(u => u.Id == currentUserId.Value, Domain.Enums.QueryType.IncludeDeleted);

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

                    if (_auditLogService == null)
                    {
                        _logger.LogDebug("AuditLogService not configured, skipping audit write for {Operation}", operation);
                        return;
                    }

                    await _auditLogService.AddAsync(
                        AuditLogHelper.Create(
                            actor,
                            action,
                            nameof(GrowthStage),
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
                    nameof(GrowthStage),
                    entityId
                );
            }
        }

        private Task WriteCreateAuditLogAsync(GrowthStage growthStage)
        {
            return WriteAuditLogAsync(
                AuditLogActions.Create,
                growthStage.Id.ToString(),
                oldValue: null,
                newValue: new
                {
                    Created = "Đã được tạo",
                    growthStage.Name,
                    growthStage.Description,
                    growthStage.SpeciesId,
                },
                "create-growth-stage"
            );
        }

        private Task WriteUpdateAuditLogAsync(GrowthStage growthStage, object oldSnapshot)
        {
            return WriteAuditLogAsync(
                AuditLogActions.Update,
                growthStage.Id.ToString(),
                oldSnapshot,
                new
                {
                    Updated = "Đã được cập nhật",
                    growthStage.Name,
                    growthStage.Description,
                    growthStage.SpeciesId,
                },
                "update-growth-stage"
            );
        }

        private Task WriteDeleteAuditLogAsync(Guid id, object oldSnapshot)
        {
            return WriteAuditLogAsync(
                AuditLogActions.Delete,
                id.ToString(),
                oldSnapshot,
                new { Deleted = "Đã được xóa" },
                "delete-growth-stage"
            );
        }
        #endregion
    }
}

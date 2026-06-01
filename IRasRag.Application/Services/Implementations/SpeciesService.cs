using AutoMapper;
using IRasRag.Application.Common.Constants;
using IRasRag.Application.Common.Interfaces.Auth;
using IRasRag.Application.Common.Interfaces.Persistence;
using IRasRag.Application.Common.Models;
using IRasRag.Application.Common.Models.Pagination;
using IRasRag.Application.Common.Utils;
using IRasRag.Application.DTOs;
using IRasRag.Application.Services.Interfaces;
using IRasRag.Application.Specifications.SpeciesSpecifications;
using IRasRag.Domain.Entities;
using IRasRag.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace IRasRag.Application.Services.Implementations
{
    public class SpeciesService : ISpeciesService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<SpeciesService> _logger;
        private readonly IMapper _mapper;
        private readonly IAuditLogService _auditLogService;
        private readonly ICurrentUserAccessor _currentUserAccessor;

        public SpeciesService(
            IUnitOfWork unitOfWork,
            ILogger<SpeciesService> logger,
            IMapper mapper,
            IAuditLogService auditLogService,
            ICurrentUserAccessor currentUserAccessor
        )
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
            _auditLogService = auditLogService;
            _currentUserAccessor = currentUserAccessor;
        }

        public async Task<Result<SpeciesDto>> CreateSpeciesAsync(CreateSpeciesDto createDto)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(createDto.Name))
                    return Result<SpeciesDto>.Failure(
                        "Tên loài không được để trống.",
                        ResultType.BadRequest
                    );

                // Kiểm tra trùng tên
                var existingSpecies = await _unitOfWork
                    .GetRepository<Species>()
                    .FirstOrDefaultAsync(s => s.Name.ToLower() == createDto.Name.Trim().ToLower());

                if (existingSpecies != null)
                    return Result<SpeciesDto>.Failure(
                        "Loài với tên này đã tồn tại.",
                        ResultType.Conflict
                    );

                var newSpecies = new Species { Name = createDto.Name.Trim() };

                await _unitOfWork.GetRepository<Species>().AddAsync(newSpecies);
                await _unitOfWork.SaveChangesAsync();

                await WriteCreateAuditLogAsync(newSpecies);

                return Result<SpeciesDto>.Success(
                    _mapper.Map<SpeciesDto>(newSpecies),
                    "Tạo loài thành công."
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tạo loài");
                return Result<SpeciesDto>.Failure("Lỗi khi tạo loài.", ResultType.Unexpected);
            }
        }

        public async Task<Result> DeleteSpeciesAsync(Guid id)
        {
            try
            {
                var species = await _unitOfWork.GetRepository<Species>().GetByIdAsync(id);

                if (species == null)
                {
                    return Result.Failure("Loài không tồn tại.", ResultType.NotFound);
                }

                var oldSnapshot = new
                {
                    species.Name,
                };

                _unitOfWork.GetRepository<Species>().Delete(species);
                await _unitOfWork.SaveChangesAsync();

                await WriteDeleteAuditLogAsync(species.Id, oldSnapshot);

                return Result.Success("Xóa loài thành công.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xóa loài");
                return Result.Failure("Lỗi khi xóa loài.", ResultType.Unexpected);
            }
        }

        public async Task<PaginatedResult<SpeciesDto>> GetAllSpeciesAsync(
            SpeciesListRequest request
        )
        {
            try
            {
                _logger.LogInformation(
                    "Bắt đầu lấy danh sách loài (Page: {Page}, PageSize: {PageSize})",
                    request.Page,
                    request.PageSize
                );

                var repository = _unitOfWork.GetRepository<Species>();
                var spec = new SpeciesDtoListSpec(request);
                var pagedResult = await repository.GetPagedAsync(
                    spec,
                    request.Page,
                    request.PageSize
                );

                var speciesDtos = pagedResult.Items;

                _logger.LogInformation(
                    "Lấy danh sách loài thành công: {Count} loài",
                    speciesDtos.Count
                );

                return new PaginatedResult<SpeciesDto>
                {
                    Message =
                        speciesDtos.Count == 0
                            ? "Không có loài nào"
                            : "Lấy danh sách loài thành công",
                    Data = speciesDtos,
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
                _logger.LogError(ex, "Lỗi khi truy xuất danh sách loài");

                return new PaginatedResult<SpeciesDto>
                {
                    Message = "Lỗi khi truy xuất danh sách loài",
                    Data = Array.Empty<SpeciesDto>(),
                    Meta = null,
                    Links = null,
                };
            }
        }

        public async Task<Result<SpeciesDto>> GetSpeciesByIdAsync(Guid id)
        {
            try
            {
                var species = await _unitOfWork.GetRepository<Species>().GetByIdAsync(id);

                if (species == null)
                    return Result<SpeciesDto>.Failure("Loài không tồn tại.", ResultType.NotFound);

                var oldSnapshot = new
                {
                    species.Name,
                };

                var dto = new SpeciesDto { Id = species.Id, Name = species.Name };

                return Result<SpeciesDto>.Success(dto, "Lấy thông tin loài thành công.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi truy xuất thông tin loài");
                return Result<SpeciesDto>.Failure(
                    "Lỗi khi truy xuất thông tin loài.",
                    ResultType.Unexpected
                );
            }
        }

        public async Task<Result> UpdateSpeciesAsync(Guid id, UpdateSpeciesDto dto)
        {
            try
            {
                var species = await _unitOfWork.GetRepository<Species>().GetByIdAsync(id);

                if (species == null)
                    return Result.Failure("Loài không tồn tại.", ResultType.NotFound);

                var oldSnapshot = new
                {
                    species.Name,
                };

                if (!string.IsNullOrWhiteSpace(dto.Name))
                {
                    var nameToUpdate = dto.Name.Trim();

                    // Kiểm tra trùng tên với loài khác
                    var existingSpecies = await _unitOfWork
                        .GetRepository<Species>()
                        .FirstOrDefaultAsync(s =>
                            s.Name.ToLower() == nameToUpdate.ToLower() && s.Id != id
                        );

                    if (existingSpecies != null)
                        return Result.Failure("Loài với tên này đã tồn tại.", ResultType.Conflict);

                    species.Name = nameToUpdate;
                }

                _unitOfWork.GetRepository<Species>().Update(species);
                await _unitOfWork.SaveChangesAsync();

                await WriteUpdateAuditLogAsync(species, oldSnapshot);

                return Result.Success("Cập nhật loài thành công.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi cập nhật loài");
                return Result.Failure("Lỗi khi cập nhật loài.", ResultType.Unexpected);
            }
        }

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
                        nameof(Species),
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
                    nameof(Species),
                    entityId
                );
            }
        }

        private Task WriteCreateAuditLogAsync(Species species)
        {
            return WriteAuditLogAsync(
                AuditLogActions.Create,
                species.Id.ToString(),
                null,
                new
                {
                    Created = "Đã được tạo",
                    species.Name,
                },
                "create-species"
            );
        }

        private Task WriteUpdateAuditLogAsync(Species species, object oldSnapshot)
        {
            return WriteAuditLogAsync(
                AuditLogActions.Update,
                species.Id.ToString(),
                oldSnapshot,
                new
                {
                    Updated = "Đã được cập nhật",
                    species.Name,
                },
                "update-species"
            );
        }

        private Task WriteDeleteAuditLogAsync(Guid id, object oldSnapshot)
        {
            return WriteAuditLogAsync(
                AuditLogActions.Delete,
                id.ToString(),
                oldSnapshot,
                new { Deleted = "Đã được xóa" },
                "delete-species"
            );
        }
        #endregion
    }
}

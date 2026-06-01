using AutoMapper;
using IRasRag.Application.Common.Constants;
using IRasRag.Application.Common.Interfaces.Auth;
using IRasRag.Application.Common.Interfaces.Persistence;
using IRasRag.Application.Common.Models;
using IRasRag.Application.Common.Models.Pagination;
using IRasRag.Application.Common.Utils;
using IRasRag.Application.DTOs;
using IRasRag.Application.Services.Interfaces;
using IRasRag.Application.Specifications.FarmSpecifications;
using IRasRag.Domain.Entities;
using IRasRag.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace IRasRag.Application.Services.Implementations
{
    public class FarmService : IFarmService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<FarmService> _logger;
        private readonly IMapper _mapper;
        private readonly IAuditLogService _auditLogService;
        private readonly ICurrentUserAccessor _currentUserAccessor;

        public FarmService(
            IUnitOfWork unitOfWork,
            ILogger<FarmService> logger,
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

        public async Task<Result<FarmDto>> CreateFarmAsync(CreateFarmDto createDto)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(createDto.Name))
                    return Result<FarmDto>.Failure(
                        "Tên trang trại không được để trống.",
                        ResultType.BadRequest
                    );

                if (string.IsNullOrWhiteSpace(createDto.Address))
                    return Result<FarmDto>.Failure(
                        "Địa chỉ không được để trống.",
                        ResultType.BadRequest
                    );

                if (string.IsNullOrWhiteSpace(createDto.PhoneNumber))
                    return Result<FarmDto>.Failure(
                        "Số điện thoại không được để trống.",
                        ResultType.BadRequest
                    );

                if (string.IsNullOrWhiteSpace(createDto.Email))
                    return Result<FarmDto>.Failure(
                        "Email không được để trống.",
                        ResultType.BadRequest
                    );

                // Kiểm tra trùng email
                var existingFarm = await _unitOfWork
                    .GetRepository<Farm>()
                    .FirstOrDefaultAsync(f =>
                        f.Email.ToLower() == createDto.Email.Trim().ToLower()
                    );

                if (existingFarm != null)
                    return Result<FarmDto>.Failure(
                        "Email này đã được sử dụng cho trang trại khác.",
                        ResultType.Conflict
                    );

                var newFarm = new Farm
                {
                    Name = createDto.Name.Trim(),
                    Address = createDto.Address.Trim(),
                    PhoneNumber = createDto.PhoneNumber.Trim(),
                    Email = createDto.Email.Trim(),
                };

                await _unitOfWork.GetRepository<Farm>().AddAsync(newFarm);
                await _unitOfWork.SaveChangesAsync();

                await WriteCreateAuditLogAsync(newFarm);

                return Result<FarmDto>.Success(
                    _mapper.Map<FarmDto>(newFarm),
                    "Tạo trang trại thành công."
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tạo trang trại");
                return Result<FarmDto>.Failure("Lỗi khi tạo trang trại.", ResultType.Unexpected);
            }
        }

        public async Task<Result> DeleteFarmAsync(Guid id)
        {
            try
            {
                var farm = await _unitOfWork.GetRepository<Farm>().GetByIdAsync(id);

                if (farm == null)
                {
                    return Result.Failure("Trang trại không tồn tại.", ResultType.NotFound);
                }

                var oldSnapshot = new
                {
                    farm.Name,
                    farm.Address,
                    farm.PhoneNumber,
                    farm.Email,
                };

                _unitOfWork.GetRepository<Farm>().Delete(farm);
                await _unitOfWork.SaveChangesAsync();

                await WriteDeleteAuditLogAsync(farm.Id, oldSnapshot);

                return Result.Success("Xóa trang trại thành công.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xóa trang trại");
                return Result.Failure("Lỗi khi xóa trang trại.", ResultType.Unexpected);
            }
        }

        public async Task<PaginatedResult<FarmDto>> GetAllFarmsAsync(FarmListRequest request)
        {
            try
            {
                var repository = _unitOfWork.GetRepository<Farm>();
                var pagedResult = await repository.GetPagedAsync(
                    new FarmListDtoSpec(request),
                    request.Page,
                    request.PageSize
                );

                return new PaginatedResult<FarmDto>
                {
                    Message =
                        pagedResult.Items.Count == 0
                            ? "Không có trang trại nào"
                            : "Lấy danh sách trang trại thành công.",
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
                _logger.LogError(ex, "Lỗi khi truy xuất danh sách trang trại");

                return new PaginatedResult<FarmDto>
                {
                    Message = "Lỗi khi truy xuất danh sách trang trại.",
                    Data = Array.Empty<FarmDto>(),
                    Meta = null,
                    Links = null,
                };
            }
        }

        public async Task<Result<FarmDto>> GetFarmByIdAsync(Guid id)
        {
            try
            {
                var farm = await _unitOfWork.GetRepository<Farm>().GetByIdAsync(id);

                if (farm == null)
                    return Result<FarmDto>.Failure(
                        "Trang trại không tồn tại.",
                        ResultType.NotFound
                    );

                var dto = new FarmDto
                {
                    Id = farm.Id,
                    Name = farm.Name,
                    Address = farm.Address,
                    PhoneNumber = farm.PhoneNumber,
                    Email = farm.Email,
                };

                return Result<FarmDto>.Success(dto, "Lấy thông tin trang trại thành công.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi truy xuất thông tin trang trại");
                return Result<FarmDto>.Failure(
                    "Lỗi khi truy xuất thông tin trang trại.",
                    ResultType.Unexpected
                );
            }
        }

        public async Task<Result> UpdateFarmAsync(Guid id, UpdateFarmDto dto)
        {
            try
            {
                var farm = await _unitOfWork.GetRepository<Farm>().GetByIdAsync(id);

                if (farm == null)
                    return Result.Failure("Trang trại không tồn tại.", ResultType.NotFound);

                var oldSnapshot = new
                {
                    farm.Name,
                    farm.Address,
                    farm.PhoneNumber,
                    farm.Email,
                };

                if (!string.IsNullOrWhiteSpace(dto.Name))
                {
                    farm.Name = dto.Name.Trim();
                }

                if (!string.IsNullOrWhiteSpace(dto.Address))
                {
                    farm.Address = dto.Address.Trim();
                }

                if (!string.IsNullOrWhiteSpace(dto.PhoneNumber))
                {
                    farm.PhoneNumber = dto.PhoneNumber.Trim();
                }

                if (!string.IsNullOrWhiteSpace(dto.Email))
                {
                    var emailToUpdate = dto.Email.Trim();

                    // Kiểm tra trùng email với trang trại khác
                    var existingFarm = await _unitOfWork
                        .GetRepository<Farm>()
                        .FirstOrDefaultAsync(f =>
                            f.Email.ToLower() == emailToUpdate.ToLower() && f.Id != id
                        );

                    if (existingFarm != null)
                        return Result.Failure(
                            "Email này đã được sử dụng cho trang trại khác.",
                            ResultType.Conflict
                        );

                    farm.Email = emailToUpdate;
                }

                _unitOfWork.GetRepository<Farm>().Update(farm);
                await _unitOfWork.SaveChangesAsync();

                await WriteUpdateAuditLogAsync(farm, oldSnapshot);

                return Result.Success("Cập nhật trang trại thành công.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi cập nhật trang trại");
                return Result.Failure("Lỗi khi cập nhật trang trại.", ResultType.Unexpected);
            }
        }

        #region Audit Log Helpers
        private async Task WriteCreateAuditLogAsync(Farm farm)
        {
            await _auditLogService.WriteSemanticAsync(
                    AuditLogActions.Create,
                    nameof(Farm),
                    farm.Id.ToString(),
                    oldValue: null,
                newValue: new
                {
                    farm.Name,
                    farm.Address,
                    farm.PhoneNumber,
                    farm.Email,
                }
            );

            await _unitOfWork.SaveChangesAsync();
        }

        private async Task WriteUpdateAuditLogAsync(Farm farm, object oldSnapshot)
        {
            await _auditLogService.WriteSemanticAsync(
                    AuditLogActions.Update,
                    nameof(Farm),
                    farm.Id.ToString(),
                    oldValue: oldSnapshot,
                newValue: new
                {
                    farm.Name,
                    farm.Address,
                    farm.PhoneNumber,
                    farm.Email,
                }
            );

            await _unitOfWork.SaveChangesAsync();
        }

        private async Task WriteDeleteAuditLogAsync(Guid farmId, object oldSnapshot)
        {
            await _auditLogService.WriteSemanticAsync(
                AuditLogActions.Delete,
                nameof(Farm),
                farmId.ToString(),
                oldValue: oldSnapshot,
                newValue: null
            );

            await _unitOfWork.SaveChangesAsync();
        }
    }
    #endregion
}

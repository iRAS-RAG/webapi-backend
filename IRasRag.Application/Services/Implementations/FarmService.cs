using AutoMapper;
using IRasRag.Application.Common.Interfaces;
using IRasRag.Application.Common.Models;
using IRasRag.Application.DTOs;
using IRasRag.Application.Services.Interfaces;
using IRasRag.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace IRasRag.Application.Services.Implementations
{
    public class FarmService : IFarmService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<FarmService> _logger;
        private readonly IMapper _mapper;

        public FarmService(IUnitOfWork unitOfWork, ILogger<FarmService> logger, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
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
                        f.Email.ToLower() == createDto.Email.Trim().ToLower() && !f.IsDeleted
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
                    IsDeleted = false,
                };

                await _unitOfWork.GetRepository<Farm>().AddAsync(newFarm);
                await _unitOfWork.SaveChangesAsync();

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

                if (farm == null || farm.IsDeleted)
                {
                    return Result.Failure("Trang trại không tồn tại.", ResultType.NotFound);
                }

                // Soft delete
                farm.IsDeleted = true;
                farm.DeletedAt = DateTime.UtcNow;

                _unitOfWork.GetRepository<Farm>().Update(farm);
                await _unitOfWork.SaveChangesAsync();

                return Result.Success("Xóa trang trại thành công.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xóa trang trại");
                return Result.Failure("Lỗi khi xóa trang trại.", ResultType.Unexpected);
            }
        }

        public async Task<Result<IEnumerable<FarmDto>>> GetAllFarmsAsync()
        {
            try
            {
                var list = await _unitOfWork.GetRepository<Farm>().FindAllAsync(f => !f.IsDeleted);

                return Result<IEnumerable<FarmDto>>.Success(
                    _mapper.Map<IEnumerable<FarmDto>>(list),
                    "Lấy danh sách trang trại thành công."
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi truy xuất danh sách trang trại");
                return Result<IEnumerable<FarmDto>>.Failure(
                    "Lỗi khi truy xuất danh sách trang trại.",
                    ResultType.Unexpected
                );
            }
        }

        public async Task<Result<FarmDto>> GetFarmByIdAsync(Guid id)
        {
            try
            {
                var farm = await _unitOfWork.GetRepository<Farm>().GetByIdAsync(id);

                if (farm == null || farm.IsDeleted)
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

                if (farm == null || farm.IsDeleted)
                    return Result.Failure("Trang trại không tồn tại.", ResultType.NotFound);

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
                            f.Email.ToLower() == emailToUpdate.ToLower()
                            && f.Id != id
                            && !f.IsDeleted
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

                return Result.Success("Cập nhật trang trại thành công.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi cập nhật trang trại");
                return Result.Failure("Lỗi khi cập nhật trang trại.", ResultType.Unexpected);
            }
        }
    }
}

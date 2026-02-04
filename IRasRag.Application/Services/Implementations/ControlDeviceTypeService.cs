using AutoMapper;
using IRasRag.Application.Common.Interfaces.Persistence;
using IRasRag.Application.Common.Models;
using IRasRag.Application.DTOs;
using IRasRag.Application.Services.Interfaces;
using IRasRag.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace IRasRag.Application.Services.Implementations
{
    public class ControlDeviceTypeService : IControlDeviceTypeService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ControlDeviceTypeService> _logger;
        private readonly IMapper _mapper;

        public ControlDeviceTypeService(
            IUnitOfWork unitOfWork,
            ILogger<ControlDeviceTypeService> logger,
            IMapper mapper
        )
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
        }

        #region Get Methods
        public async Task<Result<IEnumerable<ControlDeviceTypeDto>>> GetAllControlDeviceTypesAsync()
        {
            try
            {
                _logger.LogInformation("Bắt đầu lấy danh sách loại thiết bị điều khiển");

                var controlDeviceTypeRepository = _unitOfWork.GetRepository<ControlDeviceType>();
                var controlDeviceTypes = await controlDeviceTypeRepository.GetAllAsync();

                if (!controlDeviceTypes.Any())
                {
                    _logger.LogInformation("Không tìm thấy loại thiết bị điều khiển nào");
                    return Result<IEnumerable<ControlDeviceTypeDto>>.Success(
                        new List<ControlDeviceTypeDto>(),
                        "Không có loại thiết bị điều khiển nào"
                    );
                }

                var controlDeviceTypeDtos = _mapper.Map<IEnumerable<ControlDeviceTypeDto>>(
                    controlDeviceTypes
                );
                _logger.LogInformation(
                    "Lấy danh sách loại thiết bị điều khiển thành công: {Count} loại",
                    controlDeviceTypes.Count()
                );

                return Result<IEnumerable<ControlDeviceTypeDto>>.Success(
                    controlDeviceTypeDtos,
                    "Lấy danh sách loại thiết bị điều khiển thành công"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy danh sách loại thiết bị điều khiển");
                return Result<IEnumerable<ControlDeviceTypeDto>>.Failure(
                    "Đã xảy ra lỗi khi lấy danh sách loại thiết bị điều khiển",
                    ResultType.Unexpected
                );
            }
        }

        public async Task<Result<ControlDeviceTypeDto>> GetControlDeviceTypeByIdAsync(Guid id)
        {
            try
            {
                _logger.LogInformation("Bắt đầu lấy loại thiết bị điều khiển với Id: {Id}", id);

                if (id == Guid.Empty)
                {
                    _logger.LogWarning("Id loại thiết bị điều khiển không hợp lệ");
                    return Result<ControlDeviceTypeDto>.Failure(
                        "Id loại thiết bị điều khiển không hợp lệ",
                        ResultType.BadRequest
                    );
                }

                var controlDeviceTypeRepository = _unitOfWork.GetRepository<ControlDeviceType>();
                var controlDeviceType = await controlDeviceTypeRepository.GetByIdAsync(id);

                if (controlDeviceType == null)
                {
                    _logger.LogWarning("Không tìm thấy loại thiết bị điều khiển với Id: {Id}", id);
                    return Result<ControlDeviceTypeDto>.Failure(
                        "Không tìm thấy loại thiết bị điều khiển",
                        ResultType.NotFound
                    );
                }

                var controlDeviceTypeDto = _mapper.Map<ControlDeviceTypeDto>(controlDeviceType);
                _logger.LogInformation("Lấy loại thiết bị điều khiển thành công: {Id}", id);

                return Result<ControlDeviceTypeDto>.Success(
                    controlDeviceTypeDto,
                    "Lấy loại thiết bị điều khiển thành công"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy loại thiết bị điều khiển với Id: {Id}", id);
                return Result<ControlDeviceTypeDto>.Failure(
                    "Đã xảy ra lỗi khi lấy loại thiết bị điều khiển",
                    ResultType.Unexpected
                );
            }
        }
        #endregion

        #region Create Method
        public async Task<Result<ControlDeviceTypeDto>> CreateControlDeviceTypeAsync(
            CreateControlDeviceTypeDto createDto
        )
        {
            try
            {
                _logger.LogInformation(
                    "Bắt đầu tạo loại thiết bị điều khiển mới: {Name}",
                    createDto.Name
                );

                // Validate input
                if (string.IsNullOrWhiteSpace(createDto.Name))
                {
                    _logger.LogWarning("Tên loại thiết bị điều khiển không được để trống");
                    return Result<ControlDeviceTypeDto>.Failure(
                        "Tên loại thiết bị điều khiển không được để trống",
                        ResultType.BadRequest
                    );
                }

                var controlDeviceTypeRepository = _unitOfWork.GetRepository<ControlDeviceType>();

                // Check duplicate name
                var existingControlDeviceType =
                    await controlDeviceTypeRepository.FirstOrDefaultAsync(cdt =>
                        cdt.Name.ToLower() == createDto.Name.Trim().ToLower()
                    );

                if (existingControlDeviceType != null)
                {
                    _logger.LogWarning(
                        "Loại thiết bị điều khiển với tên {Name} đã tồn tại",
                        createDto.Name
                    );
                    return Result<ControlDeviceTypeDto>.Failure(
                        "Loại thiết bị điều khiển với tên này đã tồn tại",
                        ResultType.Conflict
                    );
                }

                var controlDeviceType = _mapper.Map<ControlDeviceType>(createDto);
                controlDeviceType.Name = createDto.Name.Trim();

                if (!string.IsNullOrWhiteSpace(createDto.Description))
                    controlDeviceType.Description = createDto.Description.Trim();

                await controlDeviceTypeRepository.AddAsync(controlDeviceType);
                await _unitOfWork.SaveChangesAsync();

                var controlDeviceTypeDto = _mapper.Map<ControlDeviceTypeDto>(controlDeviceType);
                _logger.LogInformation(
                    "Tạo loại thiết bị điều khiển thành công: {Id} - {Name}",
                    controlDeviceType.Id,
                    controlDeviceType.Name
                );

                return Result<ControlDeviceTypeDto>.Success(
                    controlDeviceTypeDto,
                    "Tạo loại thiết bị điều khiển thành công"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Lỗi khi tạo loại thiết bị điều khiển: {Name}",
                    createDto.Name
                );
                return Result<ControlDeviceTypeDto>.Failure(
                    "Đã xảy ra lỗi khi tạo loại thiết bị điều khiển",
                    ResultType.Unexpected
                );
            }
        }
        #endregion

        #region Update Method
        public async Task<Result> UpdateControlDeviceTypeAsync(
            Guid id,
            UpdateControlDeviceTypeDto updateDto
        )
        {
            try
            {
                _logger.LogInformation("Bắt đầu cập nhật loại thiết bị điều khiển: {Id}", id);

                if (id == Guid.Empty)
                {
                    _logger.LogWarning("Id loại thiết bị điều khiển không hợp lệ");
                    return Result.Failure(
                        "Id loại thiết bị điều khiển không hợp lệ",
                        ResultType.BadRequest
                    );
                }

                var controlDeviceTypeRepository = _unitOfWork.GetRepository<ControlDeviceType>();
                var controlDeviceType = await controlDeviceTypeRepository.GetByIdAsync(id);

                if (controlDeviceType == null)
                {
                    _logger.LogWarning("Không tìm thấy loại thiết bị điều khiển với Id: {Id}", id);
                    return Result.Failure(
                        "Không tìm thấy loại thiết bị điều khiển",
                        ResultType.NotFound
                    );
                }

                // Check duplicate name if name is being updated
                if (!string.IsNullOrWhiteSpace(updateDto.Name))
                {
                    var existingControlDeviceType =
                        await controlDeviceTypeRepository.FirstOrDefaultAsync(cdt =>
                            cdt.Name.ToLower() == updateDto.Name.Trim().ToLower() && cdt.Id != id
                        );

                    if (existingControlDeviceType != null)
                    {
                        _logger.LogWarning(
                            "Loại thiết bị điều khiển với tên {Name} đã tồn tại",
                            updateDto.Name
                        );
                        return Result.Failure(
                            "Loại thiết bị điều khiển với tên này đã tồn tại",
                            ResultType.Conflict
                        );
                    }
                }

                _mapper.Map(updateDto, controlDeviceType);

                // Trim string values if they were updated
                if (!string.IsNullOrWhiteSpace(updateDto.Name))
                    controlDeviceType.Name = updateDto.Name.Trim();

                if (!string.IsNullOrWhiteSpace(updateDto.Description))
                    controlDeviceType.Description = updateDto.Description.Trim();

                controlDeviceTypeRepository.Update(controlDeviceType);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Cập nhật loại thiết bị điều khiển thành công: {Id}", id);
                return Result.Success("Cập nhật loại thiết bị điều khiển thành công");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi cập nhật loại thiết bị điều khiển: {Id}", id);
                return Result.Failure(
                    "Đã xảy ra lỗi khi cập nhật loại thiết bị điều khiển",
                    ResultType.Unexpected
                );
            }
        }
        #endregion

        #region Delete Method
        public async Task<Result> DeleteControlDeviceTypeAsync(Guid id)
        {
            try
            {
                _logger.LogInformation("Bắt đầu xóa loại thiết bị điều khiển: {Id}", id);

                if (id == Guid.Empty)
                {
                    _logger.LogWarning("Id loại thiết bị điều khiển không hợp lệ");
                    return Result.Failure(
                        "Id loại thiết bị điều khiển không hợp lệ",
                        ResultType.BadRequest
                    );
                }

                var controlDeviceTypeRepository = _unitOfWork.GetRepository<ControlDeviceType>();
                var controlDeviceType = await controlDeviceTypeRepository.GetByIdAsync(id);

                if (controlDeviceType == null)
                {
                    _logger.LogWarning("Không tìm thấy loại thiết bị điều khiển với Id: {Id}", id);
                    return Result.Failure(
                        "Không tìm thấy loại thiết bị điều khiển",
                        ResultType.NotFound
                    );
                }

                controlDeviceTypeRepository.Delete(controlDeviceType);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Xóa loại thiết bị điều khiển thành công: {Id}", id);
                return Result.Success("Xóa loại thiết bị điều khiển thành công");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xóa loại thiết bị điều khiển: {Id}", id);
                return Result.Failure(
                    "Đã xảy ra lỗi khi xóa loại thiết bị điều khiển",
                    ResultType.Unexpected
                );
            }
        }
        #endregion
    }
}

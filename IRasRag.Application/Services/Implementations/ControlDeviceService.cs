using AutoMapper;
using IRasRag.Application.Common.Interfaces.Persistence;
using IRasRag.Application.Common.Models;
using IRasRag.Application.DTOs;
using IRasRag.Application.Services.Interfaces;
using IRasRag.Application.Specifications;
using IRasRag.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace IRasRag.Application.Services.Implementations
{
    public class ControlDeviceService : IControlDeviceService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ControlDeviceService> _logger;
        private readonly IMapper _mapper;

        public ControlDeviceService(
            IUnitOfWork unitOfWork,
            ILogger<ControlDeviceService> logger,
            IMapper mapper
        )
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
        }

        #region Get Methods
        public async Task<Result<IEnumerable<ControlDeviceDto>>> GetAllControlDevicesAsync()
        {
            try
            {
                _logger.LogInformation("Bắt đầu lấy danh sách thiết bị điều khiển");

                var controlDeviceRepository = _unitOfWork.GetRepository<ControlDevice>();
                var spec = new ControlDeviceDtoListSpec();
                var controlDeviceDtos = await controlDeviceRepository.ListAsync(spec);

                _logger.LogInformation(
                    "Lấy danh sách thiết bị điều khiển thành công: {Count} thiết bị",
                    controlDeviceDtos.Count()
                );

                return Result<IEnumerable<ControlDeviceDto>>.Success(
                    controlDeviceDtos,
                    "Lấy danh sách thiết bị điều khiển thành công"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy danh sách thiết bị điều khiển");
                return Result<IEnumerable<ControlDeviceDto>>.Failure(
                    "Đã xảy ra lỗi khi lấy danh sách thiết bị điều khiển",
                    ResultType.Unexpected
                );
            }
        }

        public async Task<Result<ControlDeviceDto>> GetControlDeviceByIdAsync(Guid id)
        {
            try
            {
                _logger.LogInformation("Bắt đầu lấy thiết bị điều khiển với Id: {Id}", id);

                var controlDeviceRepository = _unitOfWork.GetRepository<ControlDevice>();
                var controlDevice = await controlDeviceRepository.GetByIdAsync(id);

                if (controlDevice == null)
                {
                    _logger.LogWarning("Không tìm thấy thiết bị điều khiển với Id: {Id}", id);
                    return Result<ControlDeviceDto>.Failure(
                        $"Không tìm thấy thiết bị điều khiển với Id: {id}",
                        ResultType.NotFound
                    );
                }

                var controlDeviceDto = _mapper.Map<ControlDeviceDto>(controlDevice);
                _logger.LogInformation("Lấy thiết bị điều khiển thành công: {Id}", id);

                return Result<ControlDeviceDto>.Success(
                    controlDeviceDto,
                    "Lấy thiết bị điều khiển thành công"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy thiết bị điều khiển với Id: {Id}", id);
                return Result<ControlDeviceDto>.Failure(
                    "Đã xảy ra lỗi khi lấy thiết bị điều khiển",
                    ResultType.Unexpected
                );
            }
        }
        #endregion

        #region Create Method
        public async Task<Result<ControlDeviceDto>> CreateControlDeviceAsync(
            CreateControlDeviceDto createDto
        )
        {
            try
            {
                _logger.LogInformation(
                    "Bắt đầu tạo thiết bị điều khiển mới: {Name}",
                    createDto.Name
                );

                // Validate Name
                if (string.IsNullOrWhiteSpace(createDto.Name))
                {
                    _logger.LogWarning("Tên thiết bị điều khiển không được để trống");
                    return Result<ControlDeviceDto>.Failure(
                        "Tên thiết bị điều khiển là bắt buộc",
                        ResultType.BadRequest
                    );
                }

                var controlDeviceRepository = _unitOfWork.GetRepository<ControlDevice>();

                // Check if MasterBoard exists
                var masterBoardRepository = _unitOfWork.GetRepository<MasterBoard>();
                var masterBoard = await masterBoardRepository.GetByIdAsync(createDto.MasterBoardId);

                if (masterBoard == null)
                {
                    _logger.LogWarning(
                        "Không tìm thấy bảng mạch với Id: {MasterBoardId}",
                        createDto.MasterBoardId
                    );
                    return Result<ControlDeviceDto>.Failure(
                        $"Không tìm thấy bảng mạch với Id: {createDto.MasterBoardId}",
                        ResultType.NotFound
                    );
                }

                // Check if ControlDeviceType exists
                var controlDeviceTypeRepository = _unitOfWork.GetRepository<ControlDeviceType>();
                var controlDeviceType = await controlDeviceTypeRepository.GetByIdAsync(
                    createDto.ControlDeviceTypeId
                );

                if (controlDeviceType == null)
                {
                    _logger.LogWarning(
                        "Không tìm thấy loại thiết bị điều khiển với Id: {ControlDeviceTypeId}",
                        createDto.ControlDeviceTypeId
                    );
                    return Result<ControlDeviceDto>.Failure(
                        $"Không tìm thấy loại thiết bị điều khiển với Id: {createDto.ControlDeviceTypeId}",
                        ResultType.NotFound
                    );
                }

                // Check duplicate PinCode on the same MasterBoard
                var existingControlDevice = await controlDeviceRepository.FirstOrDefaultAsync(cd =>
                    cd.MasterBoardId == createDto.MasterBoardId && cd.PinCode == createDto.PinCode
                );

                if (existingControlDevice != null)
                {
                    _logger.LogWarning(
                        "Thiết bị điều khiển với mã chân {PinCode} đã tồn tại trên bảng mạch này",
                        createDto.PinCode
                    );
                    return Result<ControlDeviceDto>.Failure(
                        $"Thiết bị điều khiển với mã chân {createDto.PinCode} đã tồn tại trên bảng mạch này",
                        ResultType.Conflict
                    );
                }

                // Create new ControlDevice
                var controlDevice = _mapper.Map<ControlDevice>(createDto);
                await controlDeviceRepository.AddAsync(controlDevice);
                await _unitOfWork.SaveChangesAsync();

                var controlDeviceDto = _mapper.Map<ControlDeviceDto>(controlDevice);
                _logger.LogInformation(
                    "Tạo thiết bị điều khiển thành công: {Id}",
                    controlDevice.Id
                );

                return Result<ControlDeviceDto>.Success(
                    controlDeviceDto,
                    "Tạo thiết bị điều khiển thành công"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tạo thiết bị điều khiển");
                return Result<ControlDeviceDto>.Failure(
                    "Đã xảy ra lỗi khi tạo thiết bị điều khiển",
                    ResultType.Unexpected
                );
            }
        }
        #endregion

        #region Update Method
        public async Task<Result> UpdateControlDeviceAsync(
            Guid id,
            UpdateControlDeviceDto updateDto
        )
        {
            try
            {
                _logger.LogInformation("Bắt đầu cập nhật thiết bị điều khiển: {Id}", id);

                // Check if ControlDevice exists
                var controlDeviceRepository = _unitOfWork.GetRepository<ControlDevice>();
                var controlDevice = await controlDeviceRepository.GetByIdAsync(id);

                if (controlDevice == null)
                {
                    _logger.LogWarning("Không tìm thấy thiết bị điều khiển với Id: {Id}", id);
                    return Result.Failure(
                        $"Không tìm thấy thiết bị điều khiển với Id: {id}",
                        ResultType.NotFound
                    );
                }

                // Validate and update Name if provided
                if (!string.IsNullOrWhiteSpace(updateDto.Name))
                {
                    controlDevice.Name = updateDto.Name.Trim();
                }

                // Validate and update PinCode if provided
                if (updateDto.PinCode.HasValue)
                {
                    // Check duplicate PinCode on the same MasterBoard (excluding current device)
                    var existingControlDevice = await controlDeviceRepository.FirstOrDefaultAsync(
                        cd =>
                            cd.MasterBoardId == controlDevice.MasterBoardId
                            && cd.PinCode == updateDto.PinCode.Value
                            && cd.Id != id
                    );

                    if (existingControlDevice != null)
                    {
                        _logger.LogWarning(
                            "Thiết bị điều khiển với mã chân {PinCode} đã tồn tại trên bảng mạch này",
                            updateDto.PinCode.Value
                        );
                        return Result.Failure(
                            $"Thiết bị điều khiển với mã chân {updateDto.PinCode.Value} đã tồn tại trên bảng mạch này",
                            ResultType.Conflict
                        );
                    }

                    controlDevice.PinCode = updateDto.PinCode.Value;
                }

                // Update State if provided
                if (updateDto.State.HasValue)
                {
                    controlDevice.State = updateDto.State.Value;
                }

                // Update CommandOn if provided
                if (!string.IsNullOrWhiteSpace(updateDto.CommandOn))
                {
                    controlDevice.CommandOn = updateDto.CommandOn.Trim();
                }

                // Update CommandOff if provided
                if (!string.IsNullOrWhiteSpace(updateDto.CommandOff))
                {
                    controlDevice.CommandOff = updateDto.CommandOff.Trim();
                }

                // Validate and update MasterBoardId if provided
                if (updateDto.MasterBoardId.HasValue)
                {
                    var masterBoardRepository = _unitOfWork.GetRepository<MasterBoard>();
                    var masterBoard = await masterBoardRepository.GetByIdAsync(
                        updateDto.MasterBoardId.Value
                    );

                    if (masterBoard == null)
                    {
                        _logger.LogWarning(
                            "Không tìm thấy bảng mạch với Id: {MasterBoardId}",
                            updateDto.MasterBoardId.Value
                        );
                        return Result.Failure(
                            $"Không tìm thấy bảng mạch với Id: {updateDto.MasterBoardId.Value}",
                            ResultType.NotFound
                        );
                    }

                    // Check duplicate PinCode on the new MasterBoard
                    var existingDeviceOnNewBoard =
                        await controlDeviceRepository.FirstOrDefaultAsync(cd =>
                            cd.MasterBoardId == updateDto.MasterBoardId.Value
                            && cd.PinCode == controlDevice.PinCode
                            && cd.Id != id
                        );

                    if (existingDeviceOnNewBoard != null)
                    {
                        _logger.LogWarning(
                            "Thiết bị điều khiển với mã chân {PinCode} đã tồn tại trên bảng mạch mới",
                            controlDevice.PinCode
                        );
                        return Result.Failure(
                            $"Thiết bị điều khiển với mã chân {controlDevice.PinCode} đã tồn tại trên bảng mạch mới",
                            ResultType.Conflict
                        );
                    }

                    controlDevice.MasterBoardId = updateDto.MasterBoardId.Value;
                }

                // Validate and update ControlDeviceTypeId if provided
                if (updateDto.ControlDeviceTypeId.HasValue)
                {
                    var controlDeviceTypeRepository =
                        _unitOfWork.GetRepository<ControlDeviceType>();
                    var controlDeviceType = await controlDeviceTypeRepository.GetByIdAsync(
                        updateDto.ControlDeviceTypeId.Value
                    );

                    if (controlDeviceType == null)
                    {
                        _logger.LogWarning(
                            "Không tìm thấy loại thiết bị điều khiển với Id: {ControlDeviceTypeId}",
                            updateDto.ControlDeviceTypeId.Value
                        );
                        return Result.Failure(
                            $"Không tìm thấy loại thiết bị điều khiển với Id: {updateDto.ControlDeviceTypeId.Value}",
                            ResultType.NotFound
                        );
                    }

                    controlDevice.ControlDeviceTypeId = updateDto.ControlDeviceTypeId.Value;
                }

                controlDeviceRepository.Update(controlDevice);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Cập nhật thiết bị điều khiển thành công: {Id}", id);

                return Result.Success("Cập nhật thiết bị điều khiển thành công");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi cập nhật thiết bị điều khiển với Id: {Id}", id);
                return Result.Failure(
                    "Đã xảy ra lỗi khi cập nhật thiết bị điều khiển",
                    ResultType.Unexpected
                );
            }
        }
        #endregion

        #region Delete Method
        public async Task<Result> DeleteControlDeviceAsync(Guid id)
        {
            try
            {
                _logger.LogInformation("Bắt đầu xóa thiết bị điều khiển: {Id}", id);

                var controlDeviceRepository = _unitOfWork.GetRepository<ControlDevice>();
                var controlDevice = await controlDeviceRepository.GetByIdAsync(id);

                if (controlDevice == null)
                {
                    _logger.LogWarning("Không tìm thấy thiết bị điều khiển với Id: {Id}", id);
                    return Result.Failure(
                        $"Không tìm thấy thiết bị điều khiển với Id: {id}",
                        ResultType.NotFound
                    );
                }

                // Check if ControlDevice has related JobControlMappings
                var hasJobControlMappings = await controlDeviceRepository.AnyAsync(cd =>
                    cd.Id == id && cd.JobControlMappings.Any()
                );

                if (hasJobControlMappings)
                {
                    _logger.LogWarning(
                        "Không thể xóa thiết bị điều khiển {Id} vì đang có liên kết với công việc",
                        id
                    );
                    return Result.Failure(
                        "Không thể xóa thiết bị điều khiển vì đang có liên kết với công việc",
                        ResultType.Conflict
                    );
                }

                controlDeviceRepository.Delete(controlDevice);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Xóa thiết bị điều khiển thành công: {Id}", id);
                return Result.Success("Xóa thiết bị điều khiển thành công");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xóa thiết bị điều khiển với Id: {Id}", id);
                return Result.Failure(
                    "Đã xảy ra lỗi khi xóa thiết bị điều khiển",
                    ResultType.Unexpected
                );
            }
        }
        #endregion
    }
}

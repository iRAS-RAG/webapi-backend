using AutoMapper;
using IRasRag.Application.Common.Constants;
using IRasRag.Application.Common.Interfaces.Auth;
using IRasRag.Application.Common.Interfaces.Mqtt;
using IRasRag.Application.Common.Interfaces.Persistence;
using IRasRag.Application.Common.Models;
using IRasRag.Application.Common.Models.Mqtt;
using IRasRag.Application.Common.Models.Pagination;
using IRasRag.Application.Common.Utils;
using IRasRag.Application.DTOs;
using IRasRag.Application.Services.Interfaces;
using IRasRag.Application.Specifications.ControlDeviceSpecifications;
using IRasRag.Domain.Entities;
using IRasRag.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace IRasRag.Application.Services.Implementations
{
    public class ControlDeviceService : IControlDeviceService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ControlDeviceService> _logger;
        private readonly IMapper _mapper;
        private readonly IMqttPublishService _mqttPublishService;
        private readonly IAuditLogService _auditLogService;
        private readonly ICurrentUserAccessor _currentUserAccessor;

        public ControlDeviceService(
            IUnitOfWork unitOfWork,
            ILogger<ControlDeviceService> logger,
            IMapper mapper,
            IMqttPublishService mqttPublishService,
            IAuditLogService auditLogService,
            ICurrentUserAccessor currentUserAccessor
        )
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
            _mqttPublishService = mqttPublishService;
            _auditLogService = auditLogService;
            _currentUserAccessor = currentUserAccessor;
        }

        #region Get Methods
        public async Task<PaginatedResult<ControlDeviceDto>> GetAllControlDevicesAsync(
            ControlDeviceListRequest request
        )
        {
            try
            {
                _logger.LogInformation(
                    "Bắt đầu lấy danh sách thiết bị điều khiển (Page: {Page}, PageSize: {PageSize})",
                    request.Page,
                    request.PageSize
                );

                var controlDeviceRepository = _unitOfWork.GetRepository<ControlDevice>();
                var pagedResult = await controlDeviceRepository.GetPagedAsync(
                    new ControlDeviceDtoListSpec(request),
                    request.Page,
                    request.PageSize
                );

                _logger.LogInformation(
                    "Lấy danh sách thiết bị điều khiển thành công: {Count} thiết bị",
                    pagedResult.Items.Count
                );

                return new PaginatedResult<ControlDeviceDto>
                {
                    Message =
                        pagedResult.Items.Count == 0
                            ? "Không có thiết bị điều khiển nào"
                            : "Lấy danh sách thiết bị điều khiển thành công",
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
                _logger.LogError(ex, "Lỗi khi lấy danh sách thiết bị điều khiển");

                return new PaginatedResult<ControlDeviceDto>
                {
                    Message = "Đã xảy ra lỗi khi lấy danh sách thiết bị điều khiển",
                    Data = Array.Empty<ControlDeviceDto>(),
                    Meta = null,
                    Links = null,
                };
            }
        }

        public async Task<PaginatedResult<ControlDeviceDto>> GetAllControlDevicesByTankAsync(
            Guid FishTankId,
            ControlDeviceListRequest request
        )
        {
            var fishTank = await _unitOfWork.GetRepository<FishTank>().GetByIdAsync(FishTankId);
            if (fishTank == null)
            {
                _logger.LogWarning("Không tìm thấy bể cá với Id: {FishTankId}", FishTankId);
                return new PaginatedResult<ControlDeviceDto>
                {
                    Message = $"Không tìm thấy bể cá với Id: {FishTankId}",
                    Data = Array.Empty<ControlDeviceDto>(),
                    Meta = null,
                    Links = null,
                };
            }

            var pagedResult = await _unitOfWork
                .GetRepository<ControlDevice>()
                .GetPagedAsync(
                    new ControlDeviceDtoListByTankSpec(request, FishTankId),
                    request.Page,
                    request.PageSize
                );
            var meta = PaginationBuilder.BuildPaginationMetadata(
                request.Page,
                request.PageSize,
                pagedResult.TotalItems
            );
            var links = PaginationBuilder.BuildPaginationLinks(
                request.Page,
                request.PageSize,
                pagedResult.TotalItems
            );
            return new PaginatedResult<ControlDeviceDto>
            {
                Message =
                    pagedResult.Items.Count == 0
                        ? "Không có thiết bị điều khiển nào trong bể cá này"
                        : "Lấy danh sách thiết bị điều khiển thành công",
                Data = pagedResult.Items,
                Meta = meta,
                Links = links,
            };
        }

        public async Task<Result<ControlDeviceDto>> GetControlDeviceByIdAsync(Guid id)
        {
            try
            {
                _logger.LogInformation("Bắt đầu lấy thiết bị điều khiển với Id: {Id}", id);

                var controlDeviceRepository = _unitOfWork.GetRepository<ControlDevice>();
                var controlDeviceDto = await controlDeviceRepository.FirstOrDefaultAsync(
                    new ControlDeviceDtoByIdSpec(id)
                );

                if (controlDeviceDto == null)
                {
                    _logger.LogWarning("Không tìm thấy thiết bị điều khiển với Id: {Id}", id);
                    return Result<ControlDeviceDto>.Failure(
                        $"Không tìm thấy thiết bị điều khiển với Id: {id}",
                        ResultType.NotFound
                    );
                }

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

                await WriteCreateAuditLogAsync(
                    controlDevice,
                    masterBoard.Name,
                    controlDeviceType.Name
                );

                var controlDeviceDto = _mapper.Map<ControlDeviceDto>(controlDevice);
                controlDeviceDto.MasterBoardName = masterBoard.Name;
                controlDeviceDto.ControlDeviceTypeName = controlDeviceType.Name;

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

                var originalMasterBoard = await _unitOfWork
                    .GetRepository<MasterBoard>()
                    .GetByIdAsync(controlDevice.MasterBoardId);
                var originalControlDeviceType = await _unitOfWork
                    .GetRepository<ControlDeviceType>()
                    .GetByIdAsync(controlDevice.ControlDeviceTypeId);

                var oldSnapshot = new
                {
                    controlDevice.Name,
                    controlDevice.PinCode,
                    State = controlDevice.State ? "Bật" : "Tắt",
                    MasterBoardName = originalMasterBoard?.Name ?? "Không xác định",
                    ControlDeviceTypeName = originalControlDeviceType?.Name ?? "Không xác định",
                };

                var updatedMasterBoardName = originalMasterBoard?.Name ?? "Không xác định";
                var updatedControlDeviceTypeName =
                    originalControlDeviceType?.Name ?? "Không xác định";

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
                    updatedMasterBoardName = masterBoard.Name;
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
                    updatedControlDeviceTypeName = controlDeviceType.Name;
                }

                controlDeviceRepository.Update(controlDevice);
                await _unitOfWork.SaveChangesAsync();

                await WriteUpdateAuditLogAsync(
                    controlDevice,
                    oldSnapshot,
                    new
                    {
                        controlDevice.Name,
                        controlDevice.PinCode,
                        State = controlDevice.State ? "Bật" : "Tắt",
                        MasterBoardName = updatedMasterBoardName,
                        ControlDeviceTypeName = updatedControlDeviceTypeName,
                    }
                );

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

                var masterBoard = await _unitOfWork
                    .GetRepository<MasterBoard>()
                    .GetByIdAsync(controlDevice.MasterBoardId);
                var controlDeviceType = await _unitOfWork
                    .GetRepository<ControlDeviceType>()
                    .GetByIdAsync(controlDevice.ControlDeviceTypeId);

                var oldSnapshot = new
                {
                    controlDevice.Name,
                    controlDevice.PinCode,
                    State = controlDevice.State ? "Bật" : "Tắt",
                    MasterBoardName = masterBoard?.Name ?? "Không xác định",
                    ControlDeviceTypeName = controlDeviceType?.Name ?? "Không xác định",
                };

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

                await WriteDeleteAuditLogAsync(controlDevice, oldSnapshot);

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


        #region Toggle Method
        public async Task<Result<ControlDeviceDto>> ToggleControlDeviceAsync(
            Guid id,
            ToggleControlDeviceDto toggleDto
        )
        {
            try
            {
                _logger.LogInformation(
                    "Starting to toggle control device: {Id}, new state: {State}",
                    id,
                    toggleDto.State
                );

                var controlDeviceRepository = _unitOfWork.GetRepository<ControlDevice>();
                var controlDevice = await controlDeviceRepository.GetByIdAsync(id);

                if (controlDevice == null)
                {
                    _logger.LogWarning("Control device not found with Id: {Id}", id);
                    return Result<ControlDeviceDto>.Failure(
                        $"Không tìm thấy thiết bị điều khiển với Id: {id}",
                        ResultType.NotFound
                    );
                }

                var masterBoard = await _unitOfWork
                    .GetRepository<MasterBoard>()
                    .GetByIdAsync(controlDevice.MasterBoardId);

                if (masterBoard == null)
                {
                    _logger.LogWarning("MasterBoard not found for control device: {Id}", id);
                    return Result<ControlDeviceDto>.Failure(
                        "Không tìm thấy bảng mạch của thiết bị điều khiển",
                        ResultType.NotFound
                    );
                }

                var controlDeviceType = await _unitOfWork
                    .GetRepository<ControlDeviceType>()
                    .GetByIdAsync(controlDevice.ControlDeviceTypeId);

                var command = new DeviceCommand
                {
                    Pin = controlDevice.PinCode,
                    Cmd = toggleDto.State ? controlDevice.CommandOn : controlDevice.CommandOff,
                };

                await _mqttPublishService.PublishCommandAsync(masterBoard.MacAddress, command);

                var previousState = controlDevice.State;
                controlDevice.State = toggleDto.State;
                controlDeviceRepository.Update(controlDevice);

                await _auditLogService.WriteSemanticAsync(
                    action: AuditLogActions.ToggleDevice,
                    entityType: AuditLogEntityType.ControlDevice,
                    entityId: controlDevice.Id.ToString(),
                    oldValue: new
                    {
                        State = previousState ? "Bật" : "Tắt",
                        controlDevice.Name,
                        controlDevice.PinCode,
                        MasterBoardName = masterBoard.Name,
                        ControlDeviceTypeName = controlDeviceType?.Name ?? "Không xác định",
                    },
                    newValue: new
                    {
                        State = toggleDto.State ? "Bật" : "Tắt",
                        controlDevice.Name,
                        controlDevice.PinCode,
                        MasterBoardName = masterBoard.Name,
                        ControlDeviceTypeName = controlDeviceType?.Name ?? "Không xác định",
                    }
                );

                await _unitOfWork.SaveChangesAsync();

                // Truy vấn lại để trả về DTO đầy đủ (bao gồm MasterBoardName, ControlDeviceTypeName)
                var dto = await controlDeviceRepository.FirstOrDefaultAsync(
                    new ControlDeviceDtoByIdSpec(id)
                );

                _logger.LogInformation(
                    "Successfully toggled control device: {Id}, state: {State}",
                    id,
                    toggleDto.State
                );

                return Result<ControlDeviceDto>.Success(
                    dto!,
                    toggleDto.State
                        ? "Bật thiết bị điều khiển thành công"
                        : "Tắt thiết bị điều khiển thành công"
                );
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Failed to send MQTT command for device: {Id}", id);
                return Result<ControlDeviceDto>.Failure(
                    "Không thể gửi lệnh: MQTT chưa kết nối",
                    ResultType.Unexpected
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error toggling control device with Id: {Id}", id);
                return Result<ControlDeviceDto>.Failure(
                    "Đã xảy ra lỗi khi bật/tắt thiết bị điều khiển",
                    ResultType.Unexpected
                );
            }
        }
        #endregion
        #region Private Helper Methods for Audit Logging
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
                await _auditLogService.WriteSemanticAsync(
                    action,
                    AuditLogEntityType.ControlDevice,
                    entityId,
                    oldValue,
                    newValue
                );

                await _unitOfWork.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Failed to write {Operation} audit entry for {EntityType} {EntityId}",
                    operation,
                    AuditLogEntityType.ControlDevice,
                    entityId
                );
            }
        }

        private async Task WriteCreateAuditLogAsync(
            ControlDevice controlDevice,
            string masterBoardName,
            string controlDeviceTypeName
        )
        {
            await WriteAuditLogAsync(
                AuditLogActions.Create,
                controlDevice.Id.ToString(),
                oldValue: null,
                newValue: new
                {
                    controlDevice.Name,
                    controlDevice.PinCode,
                    State = controlDevice.State ? "Bật" : "Tắt",
                    MasterBoardName = masterBoardName,
                    ControlDeviceTypeName = controlDeviceTypeName,
                },
                "create-control-device"
            );
        }

        private async Task WriteUpdateAuditLogAsync(
            ControlDevice controlDevice,
            object oldSnapshot,
            object newSnapshot
        )
        {
            await WriteAuditLogAsync(
                AuditLogActions.Update,
                controlDevice.Id.ToString(),
                oldSnapshot,
                newSnapshot,
                "update-control-device"
            );
        }

        private async Task WriteDeleteAuditLogAsync(ControlDevice controlDevice, object oldSnapshot)
        {
            await WriteAuditLogAsync(
                AuditLogActions.Delete,
                controlDevice.Id.ToString(),
                oldSnapshot,
                null,
                "delete-control-device"
            );
        }
        #endregion
    }
}

using AutoMapper;
using IRasRag.Application.Common.Interfaces.Persistence;
using IRasRag.Application.Common.Models;
using IRasRag.Application.Common.Models.Pagination;
using IRasRag.Application.Common.Utils;
using IRasRag.Application.DTOs;
using IRasRag.Application.Services.Interfaces;
using IRasRag.Application.Specifications;
using IRasRag.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace IRasRag.Application.Services.Implementations
{
    public class SensorService : ISensorService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<SensorService> _logger;
        private readonly IMapper _mapper;

        public SensorService(IUnitOfWork unitOfWork, ILogger<SensorService> logger, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
        }

        #region Get Methods
        public async Task<PaginatedResult<SensorDto>> GetAllSensorsAsync(int page, int pageSize)
        {
            try
            {
                _logger.LogInformation(
                    "Bắt đầu lấy danh sách cảm biến (Page: {Page}, PageSize: {PageSize})",
                    page,
                    pageSize
                );

                var repository = _unitOfWork.GetRepository<Sensor>();
                var pagedResult = await repository.GetPagedAsync(page, pageSize);

                var sensorDtos = _mapper.Map<IReadOnlyList<SensorDto>>(pagedResult.Items);

                _logger.LogInformation(
                    "Lấy danh sách cảm biến thành công: {Count} cảm biến",
                    pagedResult.Items.Count
                );

                return new PaginatedResult<SensorDto>
                {
                    Message =
                        sensorDtos.Count == 0
                            ? "Không có cảm biến nào"
                            : "Lấy danh sách cảm biến thành công",
                    Data = sensorDtos,
                    Meta = PaginationBuilder.BuildPaginationMetadata(
                        page,
                        pageSize,
                        pagedResult.TotalItems
                    ),
                    Links = PaginationBuilder.BuildPaginationLinks(
                        page,
                        pageSize,
                        pagedResult.TotalItems
                    ),
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy danh sách cảm biến");

                return new PaginatedResult<SensorDto>
                {
                    Message = "Đã xảy ra lỗi khi lấy danh sách cảm biến",
                    Data = Array.Empty<SensorDto>(),
                    Meta = null,
                    Links = null,
                };
            }
        }

        public async Task<Result<SensorDto>> GetSensorByIdAsync(Guid id)
        {
            try
            {
                _logger.LogInformation("Bắt đầu lấy cảm biến với Id: {Id}", id);

                var sensorRepository = _unitOfWork.GetRepository<Sensor>();
                var sensor = await sensorRepository.GetByIdAsync(id);

                if (sensor == null)
                {
                    _logger.LogWarning("Không tìm thấy cảm biến với Id: {Id}", id);
                    return Result<SensorDto>.Failure(
                        $"Không tìm thấy cảm biến với Id: {id}",
                        ResultType.NotFound
                    );
                }

                var sensorDto = _mapper.Map<SensorDto>(sensor);
                _logger.LogInformation("Lấy cảm biến thành công: {Id}", id);

                return Result<SensorDto>.Success(sensorDto, "Lấy cảm biến thành công");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy cảm biến với Id: {Id}", id);
                return Result<SensorDto>.Failure(
                    "Đã xảy ra lỗi khi lấy cảm biến",
                    ResultType.Unexpected
                );
            }
        }
        #endregion

        #region Create Method
        public async Task<Result<SensorDto>> CreateSensorAsync(CreateSensorDto createDto)
        {
            try
            {
                _logger.LogInformation("Bắt đầu tạo cảm biến mới: {Name}", createDto.Name);

                // Validate Name
                if (string.IsNullOrWhiteSpace(createDto.Name))
                {
                    _logger.LogWarning("Tên cảm biến không được để trống");
                    return Result<SensorDto>.Failure(
                        "Tên cảm biến là bắt buộc",
                        ResultType.BadRequest
                    );
                }

                var sensorRepository = _unitOfWork.GetRepository<Sensor>();

                // Check if SensorType exists
                var sensorTypeRepository = _unitOfWork.GetRepository<SensorType>();
                var sensorType = await sensorTypeRepository.GetByIdAsync(createDto.SensorTypeId);

                if (sensorType == null)
                {
                    _logger.LogWarning(
                        "Không tìm thấy loại cảm biến với Id: {SensorTypeId}",
                        createDto.SensorTypeId
                    );
                    return Result<SensorDto>.Failure(
                        $"Không tìm thấy loại cảm biến với Id: {createDto.SensorTypeId}",
                        ResultType.NotFound
                    );
                }

                // Check if MasterBoard exists
                var masterBoardRepository = _unitOfWork.GetRepository<MasterBoard>();
                var masterBoard = await masterBoardRepository.GetByIdAsync(createDto.MasterBoardId);

                if (masterBoard == null)
                {
                    _logger.LogWarning(
                        "Không tìm thấy bảng mạch với Id: {MasterBoardId}",
                        createDto.MasterBoardId
                    );
                    return Result<SensorDto>.Failure(
                        $"Không tìm thấy bảng mạch với Id: {createDto.MasterBoardId}",
                        ResultType.NotFound
                    );
                }

                // Check duplicate PinCode on the same MasterBoard
                var existingSensor = await sensorRepository.FirstOrDefaultAsync(s =>
                    s.MasterBoardId == createDto.MasterBoardId && s.PinCode == createDto.PinCode
                );

                if (existingSensor != null)
                {
                    _logger.LogWarning(
                        "Cảm biến với mã chân {PinCode} đã tồn tại trên bảng mạch này",
                        createDto.PinCode
                    );
                    return Result<SensorDto>.Failure(
                        $"Cảm biến với mã chân {createDto.PinCode} đã tồn tại trên bảng mạch này",
                        ResultType.Conflict
                    );
                }

                // Create new Sensor
                var sensor = _mapper.Map<Sensor>(createDto);
                await sensorRepository.AddAsync(sensor);
                await _unitOfWork.SaveChangesAsync();

                var sensorDto = _mapper.Map<SensorDto>(sensor);
                _logger.LogInformation("Tạo cảm biến thành công: {Id}", sensor.Id);

                return Result<SensorDto>.Success(sensorDto, "Tạo cảm biến thành công");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tạo cảm biến");
                return Result<SensorDto>.Failure(
                    "Đã xảy ra lỗi khi tạo cảm biến",
                    ResultType.Unexpected
                );
            }
        }
        #endregion

        #region Update Method
        public async Task<Result> UpdateSensorAsync(Guid id, UpdateSensorDto updateDto)
        {
            try
            {
                _logger.LogInformation("Bắt đầu cập nhật cảm biến: {Id}", id);

                // Check if Sensor exists
                var sensorRepository = _unitOfWork.GetRepository<Sensor>();
                var sensor = await sensorRepository.GetByIdAsync(id);

                if (sensor == null)
                {
                    _logger.LogWarning("Không tìm thấy cảm biến với Id: {Id}", id);
                    return Result.Failure(
                        $"Không tìm thấy cảm biến với Id: {id}",
                        ResultType.NotFound
                    );
                }

                // Validate and update Name if provided
                if (!string.IsNullOrWhiteSpace(updateDto.Name))
                {
                    sensor.Name = updateDto.Name.Trim();
                }

                // Validate and update PinCode if provided
                if (updateDto.PinCode.HasValue)
                {
                    // Check duplicate PinCode on the same MasterBoard (excluding current sensor)
                    var existingSensor = await sensorRepository.FirstOrDefaultAsync(s =>
                        s.MasterBoardId == sensor.MasterBoardId
                        && s.PinCode == updateDto.PinCode.Value
                        && s.Id != id
                    );

                    if (existingSensor != null)
                    {
                        _logger.LogWarning(
                            "Cảm biến với mã chân {PinCode} đã tồn tại trên bảng mạch này",
                            updateDto.PinCode.Value
                        );
                        return Result.Failure(
                            $"Cảm biến với mã chân {updateDto.PinCode.Value} đã tồn tại trên bảng mạch này",
                            ResultType.Conflict
                        );
                    }

                    sensor.PinCode = updateDto.PinCode.Value;
                }

                // Validate and update SensorTypeId if provided
                if (updateDto.SensorTypeId.HasValue)
                {
                    var sensorTypeRepository = _unitOfWork.GetRepository<SensorType>();
                    var sensorType = await sensorTypeRepository.GetByIdAsync(
                        updateDto.SensorTypeId.Value
                    );

                    if (sensorType == null)
                    {
                        _logger.LogWarning(
                            "Không tìm thấy loại cảm biến với Id: {SensorTypeId}",
                            updateDto.SensorTypeId.Value
                        );
                        return Result.Failure(
                            $"Không tìm thấy loại cảm biến với Id: {updateDto.SensorTypeId.Value}",
                            ResultType.NotFound
                        );
                    }

                    sensor.SensorTypeId = updateDto.SensorTypeId.Value;
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
                    var existingSensorOnNewBoard = await sensorRepository.FirstOrDefaultAsync(s =>
                        s.MasterBoardId == updateDto.MasterBoardId.Value
                        && s.PinCode == sensor.PinCode
                        && s.Id != id
                    );

                    if (existingSensorOnNewBoard != null)
                    {
                        _logger.LogWarning(
                            "Cảm biến với mã chân {PinCode} đã tồn tại trên bảng mạch mới",
                            sensor.PinCode
                        );
                        return Result.Failure(
                            $"Cảm biến với mã chân {sensor.PinCode} đã tồn tại trên bảng mạch mới",
                            ResultType.Conflict
                        );
                    }

                    sensor.MasterBoardId = updateDto.MasterBoardId.Value;
                }

                sensorRepository.Update(sensor);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Cập nhật cảm biến thành công: {Id}", id);

                return Result.Success("Cập nhật cảm biến thành công");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi cập nhật cảm biến với Id: {Id}", id);
                return Result.Failure("Đã xảy ra lỗi khi cập nhật cảm biến", ResultType.Unexpected);
            }
        }
        #endregion

        #region Delete Method
        public async Task<Result> DeleteSensorAsync(Guid id)
        {
            try
            {
                _logger.LogInformation("Bắt đầu xóa cảm biến: {Id}", id);

                var sensorRepository = _unitOfWork.GetRepository<Sensor>();
                var sensor = await sensorRepository.GetByIdAsync(id);

                if (sensor == null)
                {
                    _logger.LogWarning("Không tìm thấy cảm biến với Id: {Id}", id);
                    return Result.Failure(
                        $"Không tìm thấy cảm biến với Id: {id}",
                        ResultType.NotFound
                    );
                }

                // Check if Sensor has related SensorLogs
                var hasSensorLogs = await sensorRepository.AnyAsync(s =>
                    s.Id == id && s.SensorLogs.Any()
                );

                if (hasSensorLogs)
                {
                    _logger.LogWarning(
                        "Không thể xóa cảm biến {Id} vì đang có nhật ký cảm biến liên kết",
                        id
                    );
                    return Result.Failure(
                        "Không thể xóa cảm biến vì đang có nhật ký cảm biến liên kết",
                        ResultType.Conflict
                    );
                }

                // Check if Sensor has related Jobs
                var hasJobs = await sensorRepository.AnyAsync(s => s.Id == id && s.Jobs.Any());

                if (hasJobs)
                {
                    _logger.LogWarning(
                        "Không thể xóa cảm biến {Id} vì đang có công việc liên kết",
                        id
                    );
                    return Result.Failure(
                        "Không thể xóa cảm biến vì đang có công việc liên kết",
                        ResultType.Conflict
                    );
                }

                sensorRepository.Delete(sensor);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Xóa cảm biến thành công: {Id}", id);
                return Result.Success("Xóa cảm biến thành công");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xóa cảm biến với Id: {Id}", id);
                return Result.Failure("Đã xảy ra lỗi khi xóa cảm biến", ResultType.Unexpected);
            }
        }
        #endregion
    }
}

using AutoMapper;
using IRasRag.Application.Common.Interfaces.Persistence;
using IRasRag.Application.Common.Models;
using IRasRag.Application.Common.Models.Pagination;
using IRasRag.Application.Common.Utils;
using IRasRag.Application.DTOs;
using IRasRag.Application.Services.Interfaces;
using IRasRag.Application.Specifications.SensorSpecifications;
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
        public async Task<PaginatedResult<SensorDto>> GetAllSensorsAsync(SensorListRequest request)
        {
            try
            {
                _logger.LogInformation(
                    "Bắt đầu lấy danh sách cảm biến (Page: {Page}, PageSize: {PageSize})",
                    request.Page,
                    request.PageSize
                );

                var repository = _unitOfWork.GetRepository<Sensor>();
                var pagedResult = await repository.GetPagedAsync(
                    new SensorDtoListSpec(request),
                    request.Page,
                    request.PageSize
                );

                _logger.LogInformation(
                    "Lấy danh sách cảm biến thành công: {Count} cảm biến",
                    pagedResult.Items.Count
                );

                return new PaginatedResult<SensorDto>
                {
                    Message =
                        pagedResult.Items.Count == 0
                            ? "Không có cảm biến nào"
                            : "Lấy danh sách cảm biến thành công",
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
                var existingSensor = await sensorRepository.AnyAsync(s =>
                    s.MasterBoardId == createDto.MasterBoardId && s.PinCode == createDto.PinCode
                );

                if (existingSensor)
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
                sensorDto.SensorTypeName = sensorType.Name;
                sensorDto.MasterBoardName = masterBoard.Name;
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

                sensor.Name = string.IsNullOrWhiteSpace(updateDto.Name)
                    ? sensor.Name
                    : updateDto.Name.Trim();

                var newPinCode = updateDto.PinCode ?? sensor.PinCode;
                var newMasterBoardId = updateDto.MasterBoardId ?? sensor.MasterBoardId;

                if (newPinCode != sensor.PinCode || newMasterBoardId != sensor.MasterBoardId)
                {
                    // Check duplicate PinCode on the (possibly new) MasterBoard
                    var existingSensor = await sensorRepository.AnyAsync(s =>
                        s.MasterBoardId == newMasterBoardId && s.PinCode == newPinCode && s.Id != id
                    );
                    if (existingSensor)
                    {
                        _logger.LogWarning(
                            "Cảm biến với mã chân {PinCode} đã tồn tại trên bảng mạch đã chọn",
                            newPinCode
                        );
                        return Result.Failure(
                            $"Cảm biến với mã chân {newPinCode} đã tồn tại trên bảng mạch đã chọn",
                            ResultType.Conflict
                        );
                    }

                    sensor.PinCode = newPinCode;
                }

                // Validate and update SensorTypeId if provided
                if (updateDto.SensorTypeId.HasValue)
                {
                    var sensorTypeRepository = _unitOfWork.GetRepository<SensorType>();
                    var sensorType = await sensorTypeRepository.AnyAsync(st =>
                        st.Id == updateDto.SensorTypeId.Value
                    );

                    if (!sensorType)
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
                    var existMasterBoard = await _unitOfWork
                        .GetRepository<MasterBoard>()
                        .AnyAsync(mb => mb.Id == updateDto.MasterBoardId.Value);

                    if (!existMasterBoard)
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

                    sensor.MasterBoardId = updateDto.MasterBoardId.Value;
                }

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

        #region SensorLog Methods
        public async Task<Result<SensorLogDto>> CreateSensorLogAsync(Guid sensorId, CreateSensorLogDto dto)
        {
            try
            {
                _logger.LogInformation("Bắt đầu thêm dữ liệu thủ công cho cảm biến: {SensorId}", sensorId);

                var sensorRepository = _unitOfWork.GetRepository<Sensor>();
                var sensor = await sensorRepository.GetByIdAsync(sensorId);

                if (sensor == null)
                {
                    _logger.LogWarning("Không tìm thấy cảm biến với Id: {SensorId}", sensorId);
                    return Result<SensorLogDto>.Failure(
                        $"Không tìm thấy cảm biến với Id: {sensorId}",
                        ResultType.NotFound
                    );
                }

                var logRepository = _unitOfWork.GetRepository<SensorLog>();
                var sensorLog = new SensorLog
                {
                    SensorId = sensorId,
                    Data = dto.Data,
                    IsWarning = false,
                    DataJson = "{}",
                };

                // If a custom timestamp is provided, set it before the initial save
                if (dto.Timestamp.HasValue)
                {
                    sensorLog.CreatedAt = dto.Timestamp.Value;
                }

                await logRepository.AddAsync(sensorLog);
                await _unitOfWork.SaveChangesAsync();

                var logDto = _mapper.Map<SensorLogDto>(sensorLog);
                _logger.LogInformation(
                    "Thêm dữ liệu thủ công thành công: {LogId} cho cảm biến {SensorId}",
                    sensorLog.Id,
                    sensorId
                );

                return Result<SensorLogDto>.Success(logDto, "Thêm dữ liệu cảm biến thành công");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi thêm dữ liệu thủ công cho cảm biến: {SensorId}", sensorId);
                return Result<SensorLogDto>.Failure(
                    "Đã xảy ra lỗi khi thêm dữ liệu cảm biến",
                    ResultType.Unexpected
                );
            }
        }

        public async Task<Result<PaginatedResult<SensorLogDto>>> GetSensorLogsAsync(Guid sensorId, SensorLogListRequest request)
        {
            try
            {
                _logger.LogInformation("Lấy lịch sử dữ liệu cảm biến: {SensorId}", sensorId);

                var sensorRepository = _unitOfWork.GetRepository<Sensor>();
                var sensor = await sensorRepository.GetByIdAsync(sensorId);

                if (sensor == null)
                {
                    _logger.LogWarning("Không tìm thấy cảm biến với Id: {SensorId}", sensorId);
                    return Result<PaginatedResult<SensorLogDto>>.Failure(
                        $"Không tìm thấy cảm biến với Id: {sensorId}",
                        ResultType.NotFound
                    );
                }

                var logRepository = _unitOfWork.GetRepository<SensorLog>();
                PaginatedResult<SensorLogDto> pagedResult;

                if (request.Interval.HasValue && request.Interval.Value > 0)
                {
                    // Kéo toàn bộ dữ liệu (đã lọc theo From/To), gom nhóm trong bộ nhớ, rồi phân trang
                    var allLogs = await logRepository.ListAsync(new SensorLogListSpec(sensorId, request));

                    var intervalTicks = TimeSpan.FromMinutes(request.Interval.Value).Ticks;
                    var buckets = allLogs
                        .Where(l => l.CreatedAt.HasValue)
                        .GroupBy(l =>
                        {
                            var createdAt = l.CreatedAt!.Value;
                            var kind = createdAt.Kind == DateTimeKind.Unspecified
                                ? DateTimeKind.Utc
                                : createdAt.Kind;
                            var roundedTicks = createdAt.Ticks / intervalTicks * intervalTicks;
                            return new DateTime(roundedTicks, kind);
                        })
                        .OrderBy(g => g.Key)
                        .Select(g => new SensorLogDto
                        {
                            Id = g.First().Id,
                            SensorId = sensorId,
                            Data = g.Average(l => l.Data),
                            IsWarning = g.Any(l => l.IsWarning),
                            DataJson = g.First().DataJson,
                            CreatedAt = g.Key,
                        })
                        .ToList();

                    // Phân trang trên danh sách bucket đã gom nhóm
                    var totalBuckets = buckets.Count;
                    var pagedBuckets = buckets
                        .Skip((request.Page - 1) * request.PageSize)
                        .Take(request.PageSize)
                        .ToList();

                    pagedResult = new PaginatedResult<SensorLogDto>
                    {
                        Message = totalBuckets == 0 ? "Không có dữ liệu lịch sử" : "Lấy lịch sử cảm biến thành công",
                        Data = pagedBuckets,
                        Meta = PaginationBuilder.BuildPaginationMetadata(request.Page, request.PageSize, totalBuckets),
                        Links = PaginationBuilder.BuildPaginationLinks(request.Page, request.PageSize, totalBuckets),
                    };
                }
                else
                {
                    // Phân trang trực tiếp từ cơ sở dữ liệu, không cần kéo toàn bộ dữ liệu về bộ nhớ
                    var dbPaged = await logRepository.GetPagedAsync(
                        new SensorLogListSpec(sensorId, request),
                        request.Page,
                        request.PageSize
                    );

                    pagedResult = new PaginatedResult<SensorLogDto>
                    {
                        Message = dbPaged.TotalItems == 0 ? "Không có dữ liệu lịch sử" : "Lấy lịch sử cảm biến thành công",
                        Data = dbPaged.Items,
                        Meta = PaginationBuilder.BuildPaginationMetadata(request.Page, request.PageSize, dbPaged.TotalItems),
                        Links = PaginationBuilder.BuildPaginationLinks(request.Page, request.PageSize, dbPaged.TotalItems),
                    };
                }

                _logger.LogInformation(
                    "Lấy lịch sử cảm biến thành công: {Count} bản ghi (trang {Page}/{TotalPages})",
                    pagedResult.Data?.Count ?? 0,
                    pagedResult.Meta?.Page,
                    pagedResult.Meta?.TotalPages
                );

                return Result<PaginatedResult<SensorLogDto>>.Success(pagedResult, pagedResult.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy lịch sử cảm biến với Id: {SensorId}", sensorId);
                return Result<PaginatedResult<SensorLogDto>>.Failure(
                    "Đã xảy ra lỗi khi lấy lịch sử cảm biến",
                    ResultType.Unexpected
                );
            }
        }
        
        #endregion
    }
}

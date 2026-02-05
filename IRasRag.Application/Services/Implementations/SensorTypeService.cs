using AutoMapper;
using IRasRag.Application.Common.Interfaces.Persistence;
using IRasRag.Application.Common.Models;
using IRasRag.Application.Common.Models.Pagination;
using IRasRag.Application.Common.Utils;
using IRasRag.Application.DTOs;
using IRasRag.Application.Services.Interfaces;
using IRasRag.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace IRasRag.Application.Services.Implementations
{
    public class SensorTypeService : ISensorTypeService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<SensorTypeService> _logger;
        private readonly IMapper _mapper;

        public SensorTypeService(
            IUnitOfWork unitOfWork,
            ILogger<SensorTypeService> logger,
            IMapper mapper
        )
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
        }

        #region Get Methods
        public async Task<PaginatedResult<SensorTypeDto>> GetAllSensorTypesAsync(
            int page,
            int pageSize
        )
        {
            try
            {
                _logger.LogInformation(
                    "Bắt đầu lấy danh sách loại cảm biến (Page: {Page}, PageSize: {PageSize})",
                    page,
                    pageSize
                );

                var repository = _unitOfWork.GetRepository<SensorType>();
                var pagedResult = await repository.GetPagedAsync(page, pageSize);

                var sensorTypeDtos = _mapper.Map<IReadOnlyList<SensorTypeDto>>(pagedResult.Items);

                _logger.LogInformation(
                    "Lấy danh sách loại cảm biến thành công: {Count} loại",
                    pagedResult.Items.Count
                );

                return new PaginatedResult<SensorTypeDto>
                {
                    Message =
                        sensorTypeDtos.Count == 0
                            ? "Không có loại cảm biến nào"
                            : "Lấy danh sách loại cảm biến thành công",
                    Data = sensorTypeDtos,
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
                _logger.LogError(ex, "Lỗi khi lấy danh sách loại cảm biến");

                return new PaginatedResult<SensorTypeDto>
                {
                    Message = "Đã xảy ra lỗi khi lấy danh sách loại cảm biến",
                    Data = Array.Empty<SensorTypeDto>(),
                    Meta = null,
                    Links = null,
                };
            }
        }

        public async Task<Result<SensorTypeDto>> GetSensorTypeByIdAsync(Guid id)
        {
            try
            {
                _logger.LogInformation("Bắt đầu lấy loại cảm biến với Id: {Id}", id);

                if (id == Guid.Empty)
                {
                    _logger.LogWarning("Id loại cảm biến không hợp lệ");
                    return Result<SensorTypeDto>.Failure(
                        "Id loại cảm biến không hợp lệ",
                        ResultType.BadRequest
                    );
                }

                var sensorTypeRepository = _unitOfWork.GetRepository<SensorType>();
                var sensorType = await sensorTypeRepository.GetByIdAsync(id);

                if (sensorType == null)
                {
                    _logger.LogWarning("Không tìm thấy loại cảm biến với Id: {Id}", id);
                    return Result<SensorTypeDto>.Failure(
                        "Không tìm thấy loại cảm biến",
                        ResultType.NotFound
                    );
                }

                var sensorTypeDto = _mapper.Map<SensorTypeDto>(sensorType);
                _logger.LogInformation("Lấy loại cảm biến thành công: {Id}", id);

                return Result<SensorTypeDto>.Success(sensorTypeDto, "Lấy loại cảm biến thành công");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy loại cảm biến với Id: {Id}", id);
                return Result<SensorTypeDto>.Failure(
                    "Đã xảy ra lỗi khi lấy loại cảm biến",
                    ResultType.Unexpected
                );
            }
        }
        #endregion

        #region Create Method
        public async Task<Result<SensorTypeDto>> CreateSensorTypeAsync(
            CreateSensorTypeDto createDto
        )
        {
            try
            {
                _logger.LogInformation("Bắt đầu tạo loại cảm biến mới: {Name}", createDto.Name);

                // Validate input
                if (string.IsNullOrWhiteSpace(createDto.Name))
                {
                    _logger.LogWarning("Tên loại cảm biến không được để trống");
                    return Result<SensorTypeDto>.Failure(
                        "Tên loại cảm biến không được để trống",
                        ResultType.BadRequest
                    );
                }

                if (string.IsNullOrWhiteSpace(createDto.MeasureType))
                {
                    _logger.LogWarning("Loại đo không được để trống");
                    return Result<SensorTypeDto>.Failure(
                        "Loại đo không được để trống",
                        ResultType.BadRequest
                    );
                }

                if (string.IsNullOrWhiteSpace(createDto.UnitOfMeasure))
                {
                    _logger.LogWarning("Đơn vị đo không được để trống");
                    return Result<SensorTypeDto>.Failure(
                        "Đơn vị đo không được để trống",
                        ResultType.BadRequest
                    );
                }

                var sensorTypeRepository = _unitOfWork.GetRepository<SensorType>();

                // Check duplicate name
                var existingSensorType = await sensorTypeRepository.FirstOrDefaultAsync(st =>
                    st.Name.ToLower() == createDto.Name.Trim().ToLower()
                );

                if (existingSensorType != null)
                {
                    _logger.LogWarning("Loại cảm biến với tên {Name} đã tồn tại", createDto.Name);
                    return Result<SensorTypeDto>.Failure(
                        "Loại cảm biến với tên này đã tồn tại",
                        ResultType.Conflict
                    );
                }

                var sensorType = _mapper.Map<SensorType>(createDto);
                sensorType.Name = createDto.Name.Trim();
                sensorType.MeasureType = createDto.MeasureType.Trim();
                sensorType.UnitOfMeasure = createDto.UnitOfMeasure.Trim();

                await sensorTypeRepository.AddAsync(sensorType);
                await _unitOfWork.SaveChangesAsync();

                var sensorTypeDto = _mapper.Map<SensorTypeDto>(sensorType);
                _logger.LogInformation(
                    "Tạo loại cảm biến thành công: {Id} - {Name}",
                    sensorType.Id,
                    sensorType.Name
                );

                return Result<SensorTypeDto>.Success(sensorTypeDto, "Tạo loại cảm biến thành công");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tạo loại cảm biến: {Name}", createDto.Name);
                return Result<SensorTypeDto>.Failure(
                    "Đã xảy ra lỗi khi tạo loại cảm biến",
                    ResultType.Unexpected
                );
            }
        }
        #endregion

        #region Update Method
        public async Task<Result> UpdateSensorTypeAsync(Guid id, UpdateSensorTypeDto updateDto)
        {
            try
            {
                _logger.LogInformation("Bắt đầu cập nhật loại cảm biến: {Id}", id);

                if (id == Guid.Empty)
                {
                    _logger.LogWarning("Id loại cảm biến không hợp lệ");
                    return Result.Failure("Id loại cảm biến không hợp lệ", ResultType.BadRequest);
                }

                var sensorTypeRepository = _unitOfWork.GetRepository<SensorType>();
                var sensorType = await sensorTypeRepository.GetByIdAsync(id);

                if (sensorType == null)
                {
                    _logger.LogWarning("Không tìm thấy loại cảm biến với Id: {Id}", id);
                    return Result.Failure("Không tìm thấy loại cảm biến", ResultType.NotFound);
                }

                // Check duplicate name if name is being updated
                if (!string.IsNullOrWhiteSpace(updateDto.Name))
                {
                    var existingSensorType = await sensorTypeRepository.FirstOrDefaultAsync(st =>
                        st.Name.ToLower() == updateDto.Name.Trim().ToLower() && st.Id != id
                    );

                    if (existingSensorType != null)
                    {
                        _logger.LogWarning(
                            "Loại cảm biến với tên {Name} đã tồn tại",
                            updateDto.Name
                        );
                        return Result.Failure(
                            "Loại cảm biến với tên này đã tồn tại",
                            ResultType.Conflict
                        );
                    }
                }

                _mapper.Map(updateDto, sensorType);

                // Trim string values if they were updated
                if (!string.IsNullOrWhiteSpace(updateDto.Name))
                    sensorType.Name = updateDto.Name.Trim();

                if (!string.IsNullOrWhiteSpace(updateDto.MeasureType))
                    sensorType.MeasureType = updateDto.MeasureType.Trim();

                if (!string.IsNullOrWhiteSpace(updateDto.UnitOfMeasure))
                    sensorType.UnitOfMeasure = updateDto.UnitOfMeasure.Trim();

                sensorTypeRepository.Update(sensorType);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Cập nhật loại cảm biến thành công: {Id}", id);
                return Result.Success("Cập nhật loại cảm biến thành công");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi cập nhật loại cảm biến: {Id}", id);
                return Result.Failure(
                    "Đã xảy ra lỗi khi cập nhật loại cảm biến",
                    ResultType.Unexpected
                );
            }
        }
        #endregion

        #region Delete Method
        public async Task<Result> DeleteSensorTypeAsync(Guid id)
        {
            try
            {
                _logger.LogInformation("Bắt đầu xóa loại cảm biến: {Id}", id);

                if (id == Guid.Empty)
                {
                    _logger.LogWarning("Id loại cảm biến không hợp lệ");
                    return Result.Failure("Id loại cảm biến không hợp lệ", ResultType.BadRequest);
                }

                var sensorTypeRepository = _unitOfWork.GetRepository<SensorType>();
                var sensorType = await sensorTypeRepository.GetByIdAsync(id);

                if (sensorType == null)
                {
                    _logger.LogWarning("Không tìm thấy loại cảm biến với Id: {Id}", id);
                    return Result.Failure("Không tìm thấy loại cảm biến", ResultType.NotFound);
                }

                sensorTypeRepository.Delete(sensorType);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Xóa loại cảm biến thành công: {Id}", id);
                return Result.Success("Xóa loại cảm biến thành công");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xóa loại cảm biến: {Id}", id);
                return Result.Failure("Đã xảy ra lỗi khi xóa loại cảm biến", ResultType.Unexpected);
            }
        }
    }
        #endregion
}

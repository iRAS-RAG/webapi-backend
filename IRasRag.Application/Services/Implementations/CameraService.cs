using AutoMapper;
using IRasRag.Application.Common.Interfaces.Persistence;
using IRasRag.Application.Common.Models;
using IRasRag.Application.DTOs;
using IRasRag.Application.Services.Interfaces;
using IRasRag.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace IRasRag.Application.Services.Implementations
{
    public class CameraService : ICameraService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CameraService> _logger;
        private readonly IMapper _mapper;

        public CameraService(IUnitOfWork unitOfWork, ILogger<CameraService> logger, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
        }

        #region Get Methods
        public async Task<Result<IEnumerable<CameraDto>>> GetAllCamerasAsync()
        {
            try
            {
                _logger.LogInformation("Bắt đầu lấy danh sách camera");

                var cameraRepository = _unitOfWork.GetRepository<Camera>();
                var cameras = await cameraRepository.GetAllAsync();

                if (!cameras.Any())
                {
                    _logger.LogInformation("Không tìm thấy camera nào");
                    return Result<IEnumerable<CameraDto>>.Success(
                        new List<CameraDto>(),
                        "Không có camera nào"
                    );
                }

                var cameraDtos = _mapper.Map<IEnumerable<CameraDto>>(cameras);
                _logger.LogInformation(
                    "Lấy danh sách camera thành công: {Count} camera",
                    cameras.Count()
                );

                return Result<IEnumerable<CameraDto>>.Success(
                    cameraDtos,
                    "Lấy danh sách camera thành công"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy danh sách camera");
                return Result<IEnumerable<CameraDto>>.Failure(
                    "Đã xảy ra lỗi khi lấy danh sách camera",
                    ResultType.Unexpected
                );
            }
        }

        public async Task<Result<CameraDto>> GetCameraByIdAsync(Guid id)
        {
            try
            {
                _logger.LogInformation("Bắt đầu lấy camera với Id: {Id}", id);

                if (id == Guid.Empty)
                {
                    _logger.LogWarning("Id camera không hợp lệ");
                    return Result<CameraDto>.Failure(
                        "Id camera không hợp lệ",
                        ResultType.BadRequest
                    );
                }

                var cameraRepository = _unitOfWork.GetRepository<Camera>();
                var camera = await cameraRepository.GetByIdAsync(id);

                if (camera == null)
                {
                    _logger.LogWarning("Không tìm thấy camera với Id: {Id}", id);
                    return Result<CameraDto>.Failure("Không tìm thấy camera", ResultType.NotFound);
                }

                var cameraDto = _mapper.Map<CameraDto>(camera);
                _logger.LogInformation("Lấy camera thành công: {Id}", id);

                return Result<CameraDto>.Success(cameraDto, "Lấy camera thành công");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy camera với Id: {Id}", id);
                return Result<CameraDto>.Failure(
                    "Đã xảy ra lỗi khi lấy camera",
                    ResultType.Unexpected
                );
            }
        }
        #endregion

        #region Create Method
        public async Task<Result<CameraDto>> CreateCameraAsync(CreateCameraDto createDto)
        {
            try
            {
                _logger.LogInformation("Bắt đầu tạo camera mới: {Name}", createDto.Name);

                // Validate input
                if (string.IsNullOrWhiteSpace(createDto.Name))
                {
                    _logger.LogWarning("Tên camera không được để trống");
                    return Result<CameraDto>.Failure(
                        "Tên camera không được để trống",
                        ResultType.BadRequest
                    );
                }

                if (string.IsNullOrWhiteSpace(createDto.Url))
                {
                    _logger.LogWarning("URL camera không được để trống");
                    return Result<CameraDto>.Failure(
                        "URL camera không được để trống",
                        ResultType.BadRequest
                    );
                }

                if (createDto.FarmId == Guid.Empty)
                {
                    _logger.LogWarning("Mã trang trại không hợp lệ");
                    return Result<CameraDto>.Failure(
                        "Mã trang trại không hợp lệ",
                        ResultType.BadRequest
                    );
                }

                // Verify FarmId exists
                var farmRepository = _unitOfWork.GetRepository<Farm>();
                var farmExists = await farmRepository.AnyAsync(f => f.Id == createDto.FarmId);

                if (!farmExists)
                {
                    _logger.LogWarning(
                        "Không tìm thấy trang trại với Id: {FarmId}",
                        createDto.FarmId
                    );
                    return Result<CameraDto>.Failure(
                        "Không tìm thấy trang trại",
                        ResultType.NotFound
                    );
                }

                var cameraRepository = _unitOfWork.GetRepository<Camera>();

                // Check duplicate name
                var existingCamera = await cameraRepository.FirstOrDefaultAsync(c =>
                    c.Name.ToLower() == createDto.Name.Trim().ToLower()
                    && c.FarmId == createDto.FarmId
                );

                if (existingCamera != null)
                {
                    _logger.LogWarning(
                        "Camera với tên {Name} đã tồn tại trong trang trại",
                        createDto.Name
                    );
                    return Result<CameraDto>.Failure(
                        "Camera với tên này đã tồn tại trong trang trại",
                        ResultType.Conflict
                    );
                }

                var camera = _mapper.Map<Camera>(createDto);
                camera.Name = createDto.Name.Trim();
                camera.Url = createDto.Url.Trim();

                await cameraRepository.AddAsync(camera);
                await _unitOfWork.SaveChangesAsync();

                var cameraDto = _mapper.Map<CameraDto>(camera);
                _logger.LogInformation(
                    "Tạo camera thành công: {Id} - {Name}",
                    camera.Id,
                    camera.Name
                );

                return Result<CameraDto>.Success(cameraDto, "Tạo camera thành công");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tạo camera: {Name}", createDto.Name);
                return Result<CameraDto>.Failure(
                    "Đã xảy ra lỗi khi tạo camera",
                    ResultType.Unexpected
                );
            }
        }
        #endregion

        #region Update Method
        public async Task<Result> UpdateCameraAsync(Guid id, UpdateCameraDto updateDto)
        {
            try
            {
                _logger.LogInformation("Bắt đầu cập nhật camera: {Id}", id);

                if (id == Guid.Empty)
                {
                    _logger.LogWarning("Id camera không hợp lệ");
                    return Result.Failure("Id camera không hợp lệ", ResultType.BadRequest);
                }

                var cameraRepository = _unitOfWork.GetRepository<Camera>();
                var camera = await cameraRepository.GetByIdAsync(id);

                if (camera == null)
                {
                    _logger.LogWarning("Không tìm thấy camera với Id: {Id}", id);
                    return Result.Failure("Không tìm thấy camera", ResultType.NotFound);
                }

                // Verify new FarmId exists if being updated
                if (updateDto.FarmId.HasValue && updateDto.FarmId.Value != Guid.Empty)
                {
                    var farmRepository = _unitOfWork.GetRepository<Farm>();
                    var farmExists = await farmRepository.AnyAsync(f =>
                        f.Id == updateDto.FarmId.Value
                    );

                    if (!farmExists)
                    {
                        _logger.LogWarning(
                            "Không tìm thấy trang trại với Id: {FarmId}",
                            updateDto.FarmId.Value
                        );
                        return Result.Failure("Không tìm thấy trang trại", ResultType.NotFound);
                    }
                }

                // Check duplicate name if name is being updated
                if (!string.IsNullOrWhiteSpace(updateDto.Name))
                {
                    var farmIdToCheck = updateDto.FarmId ?? camera.FarmId;
                    var existingCamera = await cameraRepository.FirstOrDefaultAsync(c =>
                        c.Name.ToLower() == updateDto.Name.Trim().ToLower()
                        && c.FarmId == farmIdToCheck
                        && c.Id != id
                    );

                    if (existingCamera != null)
                    {
                        _logger.LogWarning(
                            "Camera với tên {Name} đã tồn tại trong trang trại",
                            updateDto.Name
                        );
                        return Result.Failure(
                            "Camera với tên này đã tồn tại trong trang trại",
                            ResultType.Conflict
                        );
                    }
                }

                _mapper.Map(updateDto, camera);

                // Trim string values if they were updated
                if (!string.IsNullOrWhiteSpace(updateDto.Name))
                    camera.Name = updateDto.Name.Trim();

                if (!string.IsNullOrWhiteSpace(updateDto.Url))
                    camera.Url = updateDto.Url.Trim();

                cameraRepository.Update(camera);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Cập nhật camera thành công: {Id}", id);
                return Result.Success("Cập nhật camera thành công");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi cập nhật camera: {Id}", id);
                return Result.Failure("Đã xảy ra lỗi khi cập nhật camera", ResultType.Unexpected);
            }
        }
        #endregion

        #region Delete Method
        public async Task<Result> DeleteCameraAsync(Guid id)
        {
            try
            {
                _logger.LogInformation("Bắt đầu xóa camera: {Id}", id);

                if (id == Guid.Empty)
                {
                    _logger.LogWarning("Id camera không hợp lệ");
                    return Result.Failure("Id camera không hợp lệ", ResultType.BadRequest);
                }

                var cameraRepository = _unitOfWork.GetRepository<Camera>();
                var camera = await cameraRepository.GetByIdAsync(id);

                if (camera == null)
                {
                    _logger.LogWarning("Không tìm thấy camera với Id: {Id}", id);
                    return Result.Failure("Không tìm thấy camera", ResultType.NotFound);
                }

                cameraRepository.Delete(camera);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Xóa camera thành công: {Id}", id);
                return Result.Success("Xóa camera thành công");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xóa camera: {Id}", id);
                return Result.Failure("Đã xảy ra lỗi khi xóa camera", ResultType.Unexpected);
            }
        }
        #endregion
    }
}

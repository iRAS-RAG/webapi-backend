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
    public class MasterBoardService : IMasterBoardService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<MasterBoardService> _logger;
        private readonly IMapper _mapper;

        public MasterBoardService(
            IUnitOfWork unitOfWork,
            ILogger<MasterBoardService> logger,
            IMapper mapper
        )
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
        }

        #region Get Methods
        public async Task<PaginatedResult<MasterBoardDto>> GetAllMasterBoardsAsync(
            int page,
            int pageSize
        )
        {
            try
            {
                _logger.LogInformation(
                    "Bắt đầu lấy danh sách bảng mạch (Page: {Page}, PageSize: {PageSize})",
                    page,
                    pageSize
                );

                var repository = _unitOfWork.GetRepository<MasterBoard>();
                var pagedResult = await repository.GetPagedAsync(page, pageSize);

                var masterBoardDtos = _mapper.Map<IReadOnlyList<MasterBoardDto>>(pagedResult.Items);

                _logger.LogInformation(
                    "Lấy danh sách bảng mạch thành công: {Count} bảng mạch",
                    pagedResult.Items.Count
                );

                return new PaginatedResult<MasterBoardDto>
                {
                    Message =
                        masterBoardDtos.Count == 0
                            ? "Không có bảng mạch nào"
                            : "Lấy danh sách bảng mạch thành công",
                    Data = masterBoardDtos,
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
                _logger.LogError(ex, "Lỗi khi lấy danh sách bảng mạch");

                return new PaginatedResult<MasterBoardDto>
                {
                    Message = "Đã xảy ra lỗi khi lấy danh sách bảng mạch",
                    Data = Array.Empty<MasterBoardDto>(),
                    Meta = null,
                    Links = null,
                };
            }
        }

        public async Task<Result<MasterBoardDto>> GetMasterBoardByIdAsync(Guid id)
        {
            try
            {
                _logger.LogInformation("Bắt đầu lấy bảng mạch với Id: {Id}", id);

                var masterBoardRepository = _unitOfWork.GetRepository<MasterBoard>();
                var masterBoard = await masterBoardRepository.GetByIdAsync(id);

                if (masterBoard == null)
                {
                    _logger.LogWarning("Không tìm thấy bảng mạch với Id: {Id}", id);
                    return Result<MasterBoardDto>.Failure(
                        $"Không tìm thấy bảng mạch với Id: {id}",
                        ResultType.NotFound
                    );
                }

                var masterBoardDto = _mapper.Map<MasterBoardDto>(masterBoard);
                _logger.LogInformation("Lấy bảng mạch thành công: {Id}", id);

                return Result<MasterBoardDto>.Success(masterBoardDto, "Lấy bảng mạch thành công");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy bảng mạch với Id: {Id}", id);
                return Result<MasterBoardDto>.Failure(
                    "Đã xảy ra lỗi khi lấy bảng mạch",
                    ResultType.Unexpected
                );
            }
        }
        #endregion

        #region Create Method
        public async Task<Result<MasterBoardDto>> CreateMasterBoardAsync(
            CreateMasterBoardDto createDto
        )
        {
            try
            {
                _logger.LogInformation("Bắt đầu tạo bảng mạch mới: {Name}", createDto.Name);

                // Validate Name
                if (string.IsNullOrWhiteSpace(createDto.Name))
                {
                    _logger.LogWarning("Tên bảng mạch không được để trống");
                    return Result<MasterBoardDto>.Failure(
                        "Tên bảng mạch là bắt buộc",
                        ResultType.BadRequest
                    );
                }

                // Validate MacAddress
                if (string.IsNullOrWhiteSpace(createDto.MacAddress))
                {
                    _logger.LogWarning("Địa chỉ MAC không được để trống");
                    return Result<MasterBoardDto>.Failure(
                        "Địa chỉ MAC là bắt buộc",
                        ResultType.BadRequest
                    );
                }

                // Check duplicate MacAddress
                var masterBoardRepository = _unitOfWork.GetRepository<MasterBoard>();
                var existingMasterBoard = await masterBoardRepository.FirstOrDefaultAsync(mb =>
                    mb.MacAddress.ToLower() == createDto.MacAddress.ToLower()
                );

                if (existingMasterBoard != null)
                {
                    _logger.LogWarning(
                        "Bảng mạch với địa chỉ MAC '{MacAddress}' đã tồn tại",
                        createDto.MacAddress
                    );
                    return Result<MasterBoardDto>.Failure(
                        $"Bảng mạch với địa chỉ MAC '{createDto.MacAddress}' đã tồn tại",
                        ResultType.Conflict
                    );
                }

                // Check if FishTank exists
                var fishTankRepository = _unitOfWork.GetRepository<FishTank>();
                var fishTank = await fishTankRepository.GetByIdAsync(createDto.FishTankId);

                if (fishTank == null)
                {
                    _logger.LogWarning(
                        "Không tìm thấy hồ cá với Id: {FishTankId}",
                        createDto.FishTankId
                    );
                    return Result<MasterBoardDto>.Failure(
                        $"Không tìm thấy hồ cá với Id: {createDto.FishTankId}",
                        ResultType.NotFound
                    );
                }

                // Create new MasterBoard
                var masterBoard = _mapper.Map<MasterBoard>(createDto);
                await masterBoardRepository.AddAsync(masterBoard);
                await _unitOfWork.SaveChangesAsync();

                var masterBoardDto = _mapper.Map<MasterBoardDto>(masterBoard);
                _logger.LogInformation("Tạo bảng mạch thành công: {Id}", masterBoard.Id);

                return Result<MasterBoardDto>.Success(masterBoardDto, "Tạo bảng mạch thành công");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tạo bảng mạch");
                return Result<MasterBoardDto>.Failure(
                    "Đã xảy ra lỗi khi tạo bảng mạch",
                    ResultType.Unexpected
                );
            }
        }
        #endregion

        #region Update Method
        public async Task<Result> UpdateMasterBoardAsync(Guid id, UpdateMasterBoardDto updateDto)
        {
            try
            {
                _logger.LogInformation("Bắt đầu cập nhật bảng mạch: {Id}", id);

                // Check if MasterBoard exists
                var masterBoardRepository = _unitOfWork.GetRepository<MasterBoard>();
                var masterBoard = await masterBoardRepository.GetByIdAsync(id);

                if (masterBoard == null)
                {
                    _logger.LogWarning("Không tìm thấy bảng mạch với Id: {Id}", id);
                    return Result.Failure(
                        $"Không tìm thấy bảng mạch với Id: {id}",
                        ResultType.NotFound
                    );
                }

                // Validate and update Name if provided
                if (!string.IsNullOrWhiteSpace(updateDto.Name))
                {
                    masterBoard.Name = updateDto.Name.Trim();
                }

                // Validate and update MacAddress if provided
                if (!string.IsNullOrWhiteSpace(updateDto.MacAddress))
                {
                    // Check duplicate MacAddress (excluding current record)
                    var existingMasterBoard = await masterBoardRepository.FirstOrDefaultAsync(mb =>
                        mb.MacAddress.ToLower() == updateDto.MacAddress.ToLower() && mb.Id != id
                    );

                    if (existingMasterBoard != null)
                    {
                        _logger.LogWarning(
                            "Bảng mạch với địa chỉ MAC '{MacAddress}' đã tồn tại",
                            updateDto.MacAddress
                        );
                        return Result.Failure(
                            $"Bảng mạch với địa chỉ MAC '{updateDto.MacAddress}' đã tồn tại",
                            ResultType.Conflict
                        );
                    }

                    masterBoard.MacAddress = updateDto.MacAddress.Trim();
                }

                // Validate and update FishTankId if provided
                if (updateDto.FishTankId.HasValue)
                {
                    var fishTankRepository = _unitOfWork.GetRepository<FishTank>();
                    var fishTank = await fishTankRepository.GetByIdAsync(
                        updateDto.FishTankId.Value
                    );

                    if (fishTank == null)
                    {
                        _logger.LogWarning(
                            "Không tìm thấy hồ cá với Id: {FishTankId}",
                            updateDto.FishTankId.Value
                        );
                        return Result.Failure(
                            $"Không tìm thấy hồ cá với Id: {updateDto.FishTankId.Value}",
                            ResultType.NotFound
                        );
                    }

                    masterBoard.FishTankId = updateDto.FishTankId.Value;
                }

                masterBoardRepository.Update(masterBoard);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Cập nhật bảng mạch thành công: {Id}", id);

                return Result.Success("Cập nhật bảng mạch thành công");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi cập nhật bảng mạch với Id: {Id}", id);
                return Result.Failure(
                    "Đã xảy ra lỗi khi cập nhật bảng mạch",
                    ResultType.Unexpected
                );
            }
        }
        #endregion

        #region Delete Method
        public async Task<Result> DeleteMasterBoardAsync(Guid id)
        {
            try
            {
                _logger.LogInformation("Bắt đầu xóa bảng mạch: {Id}", id);

                var masterBoardRepository = _unitOfWork.GetRepository<MasterBoard>();
                var masterBoard = await masterBoardRepository.GetByIdAsync(id);

                if (masterBoard == null)
                {
                    _logger.LogWarning("Không tìm thấy bảng mạch với Id: {Id}", id);
                    return Result.Failure(
                        $"Không tìm thấy bảng mạch với Id: {id}",
                        ResultType.NotFound
                    );
                }

                // Check if MasterBoard has related Sensors
                var hasSensors = await masterBoardRepository.AnyAsync(mb =>
                    mb.Id == id && mb.Sensors.Any()
                );

                if (hasSensors)
                {
                    _logger.LogWarning(
                        "Không thể xóa bảng mạch {Id} vì đang có cảm biến liên kết",
                        id
                    );
                    return Result.Failure(
                        "Không thể xóa bảng mạch vì đang có cảm biến liên kết",
                        ResultType.Conflict
                    );
                }

                // Check if MasterBoard has related ControlDevices
                var hasControlDevices = await masterBoardRepository.AnyAsync(mb =>
                    mb.Id == id && mb.ControlDevices.Any()
                );

                if (hasControlDevices)
                {
                    _logger.LogWarning(
                        "Không thể xóa bảng mạch {Id} vì đang có thiết bị điều khiển liên kết",
                        id
                    );
                    return Result.Failure(
                        "Không thể xóa bảng mạch vì đang có thiết bị điều khiển liên kết",
                        ResultType.Conflict
                    );
                }

                masterBoardRepository.Delete(masterBoard);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Xóa bảng mạch thành công: {Id}", id);
                return Result.Success("Xóa bảng mạch thành công");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xóa bảng mạch với Id: {Id}", id);
                return Result.Failure("Đã xảy ra lỗi khi xóa bảng mạch", ResultType.Unexpected);
            }
        }
        #endregion
    }
}

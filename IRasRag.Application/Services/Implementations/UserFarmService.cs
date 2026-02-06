using AutoMapper;
using IRasRag.Application.Common.Interfaces;
using IRasRag.Application.Common.Interfaces.Persistence;
using IRasRag.Application.Common.Models;
using IRasRag.Application.Common.Models.Pagination;
using IRasRag.Application.Common.Utils;
using IRasRag.Application.DTOs;
using IRasRag.Application.Specifications;
using IRasRag.Domain.Entities;
using IRasRag.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace IRasRag.Application.Services.Implementations
{
    public class UserFarmService : IUserFarmService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<UserFarmService> _logger;

        public UserFarmService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<UserFarmService> logger
        )
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        #region Get Methods
        public async Task<PaginatedResult<UserFarmDto>> GetAllUserFarmsAsync(int page, int pageSize)
        {
            try
            {
                var spec = new UserFarmDtoListSpec();
                var pagedResult = await _unitOfWork
                    .GetRepository<UserFarm>()
                    .GetPagedAsync(spec, page, pageSize);

                var meta = PaginationBuilder.BuildPaginationMetadata(
                    page,
                    pageSize,
                    pagedResult.TotalItems
                );

                var links = PaginationBuilder.BuildPaginationLinks(
                    page,
                    pageSize,
                    pagedResult.TotalItems
                );

                return new PaginatedResult<UserFarmDto>
                {
                    Message = "Lấy danh sách phân quyền người dùng-trang trại thành công",
                    Data = pagedResult.Items.ToList(),
                    Meta = meta,
                    Links = links,
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy danh sách phân quyền người dùng-trang trại");
                return new PaginatedResult<UserFarmDto>
                {
                    Message = "Lỗi khi lấy danh sách phân quyền người dùng-trang trại",
                    Data = new List<UserFarmDto>(),
                    Meta = new PaginationMeta(),
                    Links = new PaginationLinks(),
                };
            }
        }

        public async Task<Result<UserFarmDto>> GetUserFarmByIdAsync(Guid id)
        {
            try
            {
                var userFarmRepo = _unitOfWork.GetRepository<UserFarm>();
                var userFarm = await userFarmRepo.FirstOrDefaultAsync(uf => uf.Id == id);

                if (userFarm == null)
                {
                    return Result<UserFarmDto>.Failure(
                        "Không tìm thấy phân quyền người dùng-trang trại",
                        ResultType.NotFound
                    );
                }

                // Load related entities
                var userRepo = _unitOfWork.GetRepository<User>();
                var user = await userRepo.GetByIdAsync(userFarm.UserId);

                var farmRepo = _unitOfWork.GetRepository<Farm>();
                var farm = await farmRepo.GetByIdAsync(userFarm.FarmId);

                userFarm.User = user!;
                userFarm.Farm = farm!;

                var userFarmDto = _mapper.Map<UserFarmDto>(userFarm);

                return Result<UserFarmDto>.Success(
                    userFarmDto,
                    "Lấy thông tin phân quyền người dùng-trang trại thành công"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Lỗi khi lấy thông tin phân quyền người dùng-trang trại với ID {Id}",
                    id
                );
                return Result<UserFarmDto>.Failure(
                    "Lỗi khi lấy thông tin phân quyền người dùng-trang trại",
                    ResultType.Unexpected
                );
            }
        }
        #endregion

        #region Create Methods
        public async Task<Result<UserFarmDto>> CreateUserFarmAsync(CreateUserFarmDto createDto)
        {
            try
            {
                // Validate User exists
                var userRepo = _unitOfWork.GetRepository<User>();
                var userExists = await userRepo.AnyAsync(u => u.Id == createDto.UserId);
                if (!userExists)
                {
                    return Result<UserFarmDto>.Failure(
                        "Người dùng không tồn tại",
                        ResultType.BadRequest
                    );
                }

                // Validate Farm exists
                var farmRepo = _unitOfWork.GetRepository<Farm>();
                var farmExists = await farmRepo.AnyAsync(f => f.Id == createDto.FarmId);
                if (!farmExists)
                {
                    return Result<UserFarmDto>.Failure(
                        "Trang trại không tồn tại",
                        ResultType.BadRequest
                    );
                }

                // Check if mapping already exists
                var userFarmRepo = _unitOfWork.GetRepository<UserFarm>();
                var existingMapping = await userFarmRepo.AnyAsync(uf =>
                    uf.UserId == createDto.UserId && uf.FarmId == createDto.FarmId
                );
                if (existingMapping)
                {
                    return Result<UserFarmDto>.Failure(
                        "Phân quyền giữa người dùng và trang trại này đã tồn tại",
                        ResultType.Conflict
                    );
                }

                // Map and create
                var userFarm = _mapper.Map<UserFarm>(createDto);

                await userFarmRepo.AddAsync(userFarm);
                await _unitOfWork.SaveChangesAsync();

                // Load related entities for response
                var user = await userRepo.GetByIdAsync(userFarm.UserId);
                var farm = await farmRepo.GetByIdAsync(userFarm.FarmId);
                userFarm.User = user!;
                userFarm.Farm = farm!;

                var userFarmDto = _mapper.Map<UserFarmDto>(userFarm);

                return Result<UserFarmDto>.Success(
                    userFarmDto,
                    "Tạo phân quyền người dùng-trang trại thành công"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tạo phân quyền người dùng-trang trại");
                return Result<UserFarmDto>.Failure(
                    "Lỗi khi tạo phân quyền người dùng-trang trại",
                    ResultType.Unexpected
                );
            }
        }
        #endregion

        #region Update Methods
        public async Task<Result> UpdateUserFarmAsync(Guid id, UpdateUserFarmDto updateDto)
        {
            try
            {
                var userFarmRepo = _unitOfWork.GetRepository<UserFarm>();
                var userFarm = await userFarmRepo.GetByIdAsync(id);

                if (userFarm == null)
                {
                    return Result.Failure(
                        "Không tìm thấy phân quyền người dùng-trang trại",
                        ResultType.NotFound
                    );
                }

                // For junction tables, typically no fields to update
                // This method is included for CRUD completeness
                // If needed in the future, you can add logic here

                _mapper.Map(updateDto, userFarm);
                userFarmRepo.Update(userFarm);
                await _unitOfWork.SaveChangesAsync();

                return Result.Success("Cập nhật phân quyền người dùng-trang trại thành công");
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Lỗi khi cập nhật phân quyền người dùng-trang trại với ID {Id}",
                    id
                );
                return Result.Failure(
                    "Lỗi khi cập nhật phân quyền người dùng-trang trại",
                    ResultType.Unexpected
                );
            }
        }
        #endregion

        #region Delete Methods
        public async Task<Result> DeleteUserFarmAsync(Guid id)
        {
            try
            {
                var userFarmRepo = _unitOfWork.GetRepository<UserFarm>();
                var userFarm = await userFarmRepo.GetByIdAsync(id);

                if (userFarm == null)
                {
                    return Result.Failure(
                        "Không tìm thấy phân quyền người dùng-trang trại",
                        ResultType.NotFound
                    );
                }

                userFarmRepo.Delete(userFarm);
                await _unitOfWork.SaveChangesAsync();

                return Result.Success("Xóa phân quyền người dùng-trang trại thành công");
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Lỗi khi xóa phân quyền người dùng-trang trại với ID {Id}",
                    id
                );
                return Result.Failure(
                    "Lỗi khi xóa phân quyền người dùng-trang trại",
                    ResultType.Unexpected
                );
            }
        }
        #endregion
    }
}

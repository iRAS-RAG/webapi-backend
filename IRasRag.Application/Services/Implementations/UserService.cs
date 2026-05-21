using AutoMapper;
using IRasRag.Application.Common.Interfaces.Auth;
using IRasRag.Application.Common.Interfaces.BackgroundJobs;
using IRasRag.Application.Common.Interfaces.Email;
using IRasRag.Application.Common.Interfaces.Persistence;
using IRasRag.Application.Common.Models;
using IRasRag.Application.Common.Models.Pagination;
using IRasRag.Application.Common.Utils;
using IRasRag.Application.DTOs;
using IRasRag.Application.Services.Interfaces;
using IRasRag.Application.Specifications.UserSpecifications;
using IRasRag.Domain.Entities;
using IRasRag.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace IRasRag.Application.Services.Implementations
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<UserService> _logger;
        private readonly IMapper _mapper;
        private readonly IHashingService _hasher;
        private readonly IBackgroundJobService _backgroundJobService;
        private readonly IEmailService _emailService;

        public UserService(
            IUnitOfWork unitOfWork,
            ILogger<UserService> logger,
            IMapper mapper,
            IHashingService hasher,
            IBackgroundJobService backgroundJobService,
            IEmailService emailService
        )
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
            _hasher = hasher;
            _backgroundJobService = backgroundJobService;
            _emailService = emailService;
        }

        public async Task<Result<UserDto>> CreateOperatorAsync(CreateOperatorUserDto createDto)
        {
            try
            {
                var normalizedEmail = createDto.Email.Trim().ToLower();

                var existingUserByEmail = await _unitOfWork
                    .GetRepository<User>()
                    .FirstOrDefaultAsync(u => u.Email.ToLower() == normalizedEmail && !u.IsDeleted);

                if (existingUserByEmail != null)
                    return Result<UserDto>.Failure("Email đã tồn tại.", ResultType.Conflict);

                var userRole = await _unitOfWork
                    .GetRepository<Role>()
                    .FirstOrDefaultAsync(r => r.Name.ToLower() == "operator");

                if (userRole == null)
                {
                    return Result<UserDto>.Failure("Vai trò không tồn tại.", ResultType.BadRequest);
                }

                var roleName = userRole?.ToSystemRole().ToRoleName() ?? "Kỹ thuật viên";

                var newUser = new User
                {
                    RoleId = userRole.Id,
                    Email = normalizedEmail,
                    FirstName = createDto.FirstName?.Trim(),
                    LastName = createDto.LastName?.Trim(),
                    PasswordHash = _hasher.HashPassword(createDto.Password),
                    IsDeleted = false,
                    //Temporary assign new operator to the seeded farm
                    UserFarms = new List<UserFarm>
                    {
                        new UserFarm
                        {
                            FarmId = Guid.Parse("aaaaaaaa-0000-0000-0000-000000000001"),
                        },
                    },
                };

                var emailBody = await _emailService.GenerateAccountCreatedEmailBodyAsync(
                    roleName,
                    newUser.Email,
                    createDto.Password
                );

                await _unitOfWork.GetRepository<User>().AddAsync(newUser);
                await _unitOfWork.SaveChangesAsync();

                _backgroundJobService.Enqueue<IEmailService>(service =>
                    service.SendEmailAsync(newUser.Email, "Tài khoản IRAS-RAG mới", emailBody)
                );

                return Result<UserDto>.Success(
                    _mapper.Map<UserDto>(newUser),
                    "Tạo người dùng thành công."
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tạo người dùng");
                return Result<UserDto>.Failure("Lỗi khi tạo người dùng.", ResultType.Unexpected);
            }
        }

        public async Task<Result<UserDto>> CreateUserAsync(CreateUserDto createDto)
        {
            try
            {
                var normalizedEmail = createDto.Email.Trim().ToLower();

                // Kiểm tra trùng email
                var existingUserByEmail = await _unitOfWork
                    .GetRepository<User>()
                    .FirstOrDefaultAsync(u => u.Email == normalizedEmail && !u.IsDeleted);

                if (existingUserByEmail != null)
                    return Result<UserDto>.Failure("Email đã tồn tại.", ResultType.Conflict);

                var userRole = await _unitOfWork
                    .GetRepository<Role>()
                    .FirstOrDefaultAsync(r =>
                        r.Name.ToLower() == createDto.RoleName.Trim().ToLower()
                    );

                if (userRole == null)
                {
                    return Result<UserDto>.Failure(
                        "Vai trò người dùng không hợp lệ.",
                        ResultType.BadRequest
                    );
                }

                var systemRole = userRole.ToSystemRole();

                var newUser = new User
                {
                    RoleId = userRole.Id,
                    Email = normalizedEmail,
                    FirstName = createDto.FirstName?.Trim(),
                    LastName = createDto.LastName?.Trim(),
                    PasswordHash = _hasher.HashPassword(createDto.Password),
                    IsDeleted = false,
                };

                if (systemRole == SystemRole.Operator || systemRole == SystemRole.Supervisor)
                {
                    // Temporary assign new non admin user to the seeded farm
                    newUser.UserFarms = new List<UserFarm>
                    {
                        new UserFarm
                        {
                            FarmId = Guid.Parse("aaaaaaaa-0000-0000-0000-000000000001"),
                        },
                    };
                }

                await _unitOfWork.GetRepository<User>().AddAsync(newUser);
                await _unitOfWork.SaveChangesAsync();

                var emailBody = await _emailService.GenerateAccountCreatedEmailBodyAsync(
                    systemRole.ToRoleName(),
                    newUser.Email,
                    createDto.Password
                );

                _backgroundJobService.Enqueue<IEmailService>(service =>
                    service.SendEmailAsync(newUser.Email, "Tài khoản IRAS-RAG mới", emailBody)
                );

                return Result<UserDto>.Success(
                    _mapper.Map<UserDto>(newUser),
                    "Tạo người dùng thành công."
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tạo người dùng");
                return Result<UserDto>.Failure("Lỗi khi tạo người dùng.", ResultType.Unexpected);
            }
        }

        public async Task<Result> DeleteUserAsync(Guid id)
        {
            try
            {
                var user = await _unitOfWork.GetRepository<User>().GetByIdAsync(id);

                if (user == null || user.IsDeleted)
                {
                    return Result.Failure(
                        "Người dùng không tồn tại hoặc đã xóa.",
                        ResultType.NotFound
                    );
                }

                _unitOfWork.GetRepository<User>().Delete(user);
                await _unitOfWork.SaveChangesAsync();

                return Result.Success("Xóa người dùng thành công.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xóa người dùng");
                return Result.Failure("Lỗi khi xóa người dùng.", ResultType.Unexpected);
            }
        }

        public async Task<Result> HardDeleteUserAsync(Guid id)
        {
            try
            {
                var user = await _unitOfWork
                    .GetRepository<User>()
                    .GetByIdAsync(id, QueryType.IncludeDeleted);

                if (user == null)
                {
                    return Result.Failure("Người dùng không tồn tại.", ResultType.NotFound);
                }

                _unitOfWork.GetRepository<User>().HardDelete(user);
                await _unitOfWork.SaveChangesAsync();

                return Result.Success("Xóa vĩnh viễn người dùng thành công.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xóa vĩnh viễn người dùng");
                return Result.Failure("Lỗi khi xóa vĩnh viễn người dùng.", ResultType.Unexpected);
            }
        }

        public async Task<PaginatedResult<UserDto>> GetAllUsersAsync(UserListRequest request)
        {
            try
            {
                _logger.LogInformation(
                    "Bắt đầu lấy danh sách người dùng (Page: {Page}, PageSize: {PageSize})",
                    request.Page,
                    request.PageSize
                );

                var repository = _unitOfWork.GetRepository<User>();
                var pagedResult = await repository.GetPagedAsync(
                    new UserDtoListSpec(request),
                    request.Page,
                    request.PageSize,
                    QueryType.IncludeDeleted
                );

                var userDtos = pagedResult.Items;

                _logger.LogInformation(
                    "Lấy danh sách người dùng thành công: {Count} người dùng",
                    userDtos.Count
                );

                return new PaginatedResult<UserDto>
                {
                    Message =
                        userDtos.Count == 0
                            ? "Không có người dùng nào"
                            : "Lấy danh sách người dùng thành công",
                    Data = userDtos,
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
                _logger.LogError(ex, "Lỗi khi truy xuất danh sách người dùng");

                return new PaginatedResult<UserDto>
                {
                    Message = "Lỗi khi truy xuất danh sách người dùng",
                    Data = Array.Empty<UserDto>(),
                    Meta = null,
                    Links = null,
                };
            }
        }

        public async Task<Result<UserDto>> GetUserByIdAsync(Guid id)
        {
            try
            {
                var user = await _unitOfWork.GetRepository<User>().GetByIdAsync(id);

                if (user == null || user.IsDeleted)
                    return Result<UserDto>.Failure(
                        "Người dùng không tồn tại.",
                        ResultType.NotFound
                    );

                var role = await _unitOfWork.GetRepository<Role>().GetByIdAsync(user.RoleId);
                var roleName = role?.ToSystemRole().ToRoleName() ?? "Không xác định";

                var dto = new UserDto
                {
                    Id = user.Id,
                    RoleId = user.RoleId,
                    RoleName = roleName,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    IsDeleted = user.IsDeleted,
                };

                return Result<UserDto>.Success(dto, "Lấy thông tin người dùng thành công.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi truy xuất thông tin người dùng");
                return Result<UserDto>.Failure(
                    "Lỗi khi truy xuất thông tin người dùng.",
                    ResultType.Unexpected
                );
            }
        }

        public async Task<Result<UserProfileDto>> GetUserProfileAsync(Guid id)
        {
            try
            {
                var user = await _unitOfWork.GetRepository<User>().GetByIdAsync(id);

                if (user == null || user.IsDeleted)
                    return Result<UserProfileDto>.Failure(
                        "Người dùng không tồn tại.",
                        ResultType.NotFound
                    );

                var role = await _unitOfWork.GetRepository<Role>().GetByIdAsync(user.RoleId);
                var roleName = role?.ToSystemRole().ToRoleName() ?? "Không xác định";
                var dto = new UserProfileDto
                {
                    Id = user.Id,
                    RoleName = roleName,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                };

                return Result<UserProfileDto>.Success(dto, "Lấy hồ sơ người dùng thành công.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi truy xuất hồ sơ người dùng");
                return Result<UserProfileDto>.Failure(
                    "Lỗi khi truy xuất hồ sơ người dùng.",
                    ResultType.Unexpected
                );
            }
        }

        public async Task<Result<UserDto>> UpdateUserAsync(Guid id, UpdateUserDto dto)
        {
            try
            {
                var user = await _unitOfWork
                    .GetRepository<User>()
                    .GetByIdAsync(id, QueryType.IncludeDeleted);
                Role? updatedRole = null;

                if (user == null)
                    return Result<UserDto>.Failure(
                        "Người dùng không tồn tại.",
                        ResultType.NotFound
                    );

                if (!string.IsNullOrWhiteSpace(dto.Email))
                {
                    var emailToUpdate = dto.Email.Trim().ToLower();

                    // Kiểm tra trùng email với người dùng khác
                    var existingUser = await _unitOfWork
                        .GetRepository<User>()
                        .FirstOrDefaultAsync(u =>
                            u.Email.ToLower() == emailToUpdate.ToLower()
                            && u.Id != id
                            && !u.IsDeleted
                        );

                    if (existingUser != null)
                        return Result<UserDto>.Failure("Email đã tồn tại.", ResultType.Conflict);

                    user.Email = emailToUpdate;
                }

                if (!string.IsNullOrWhiteSpace(dto.FirstName))
                {
                    user.FirstName = dto.FirstName.Trim();
                }

                if (!string.IsNullOrWhiteSpace(dto.LastName))
                {
                    user.LastName = dto.LastName.Trim();
                }

                if (!string.IsNullOrWhiteSpace(dto.Password))
                {
                    if (dto.Password.Length < 6)
                        return Result<UserDto>.Failure(
                            "Mật khẩu phải có ít nhất 6 ký tự.",
                            ResultType.BadRequest
                        );

                    user.PasswordHash = _hasher.HashPassword(dto.Password);
                }

                if (!string.IsNullOrWhiteSpace(dto.RoleName))
                {
                    var normalizedRoleName = dto.RoleName.Trim().ToLower();
                    updatedRole = await _unitOfWork
                        .GetRepository<Role>()
                        .FirstOrDefaultAsync(r => r.Name.ToLower() == normalizedRoleName);
                    if (updatedRole == null)
                    {
                        return Result<UserDto>.Failure(
                            "Vai trò người dùng không hợp lệ.",
                            ResultType.BadRequest
                        );
                    }
                    user.RoleId = updatedRole.Id;
                }

                if (dto.IsDeleted.HasValue)
                {
                    user.IsDeleted = dto.IsDeleted.Value;
                    if (dto.IsDeleted.Value)
                    {
                        user.DeletedAt = DateTime.UtcNow;
                    }
                    else
                        user.DeletedAt = null;
                }

                _unitOfWork.GetRepository<User>().Update(user);
                await _unitOfWork.SaveChangesAsync();

                updatedRole ??= await _unitOfWork.GetRepository<Role>().GetByIdAsync(user.RoleId);

                return Result<UserDto>.Success(
                    new UserDto
                    {
                        Id = user.Id,
                        RoleId = user.RoleId,
                        RoleName = updatedRole?.Name ?? string.Empty,
                        Email = user.Email,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        IsDeleted = user.IsDeleted,
                    },
                    "Cập nhật người dùng thành công."
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi cập nhật người dùng");
                return Result<UserDto>.Failure(
                    "Lỗi khi cập nhật người dùng.",
                    ResultType.Unexpected
                );
            }
        }

        public async Task<Result> UpdateUserPasswordAsync(Guid id, UpdateUserPasswordDto dto)
        {
            try
            {
                var user = await _unitOfWork.GetRepository<User>().GetByIdAsync(id);

                if (user == null || user.IsDeleted)
                    return Result.Failure("Người dùng không tồn tại.", ResultType.NotFound);

                if (!_hasher.VerifyPassword(dto.OldPassword, user.PasswordHash))
                    return Result.Failure("Mật khẩu cũ không đúng.", ResultType.BadRequest);

                if (dto.NewPassword != dto.ConfirmNewPassword)
                    return Result.Failure(
                        "Mật khẩu mới và xác nhận mật khẩu mới không khớp.",
                        ResultType.BadRequest
                    );

                user.PasswordHash = _hasher.HashPassword(dto.NewPassword);
                _unitOfWork.GetRepository<User>().Update(user);
                await _unitOfWork.SaveChangesAsync();
                return Result.Success("Cập nhật mật khẩu người dùng thành công.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi cập nhật mật khẩu người dùng");
                return Result.Failure(
                    "Lỗi khi cập nhật mật khẩu người dùng.",
                    ResultType.Unexpected
                );
            }
        }

        public async Task<Result<UserDto>> UpdateUserProfileAsync(Guid id, UpdateUserProfileDto dto)
        {
            try
            {
                var user = await _unitOfWork.GetRepository<User>().GetByIdAsync(id);

                if (user == null || user.IsDeleted)
                    return Result<UserDto>.Failure(
                        "Người dùng không tồn tại.",
                        ResultType.NotFound
                    );

                if (!string.IsNullOrWhiteSpace(dto.Email))
                {
                    var emailToUpdate = dto.Email.Trim().ToLower();

                    // Kiểm tra trùng email với người dùng khác
                    var existingUser = await _unitOfWork
                        .GetRepository<User>()
                        .FirstOrDefaultAsync(u =>
                            u.Email.ToLower() == emailToUpdate.ToLower() && u.Id != id
                        );

                    if (existingUser != null)
                        return Result<UserDto>.Failure("Email đã tồn tại.", ResultType.Conflict);

                    user.Email = emailToUpdate;
                }

                if (!string.IsNullOrWhiteSpace(dto.FirstName))
                {
                    user.FirstName = dto.FirstName.Trim();
                }

                if (!string.IsNullOrWhiteSpace(dto.LastName))
                {
                    user.LastName = dto.LastName.Trim();
                }

                _unitOfWork.GetRepository<User>().Update(user);
                await _unitOfWork.SaveChangesAsync();

                return Result<UserDto>.Success(
                    _mapper.Map<UserDto>(user),
                    "Cập nhật người dùng thành công."
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi cập nhật hồ sơ người dùng");
                return Result<UserDto>.Failure(
                    "Lỗi khi cập nhật hồ sơ người dùng.",
                    ResultType.Unexpected
                );
            }
        }
    }
}

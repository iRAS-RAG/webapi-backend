using AutoMapper;
using IRasRag.Application.Common.Interfaces;
using IRasRag.Application.Common.Models;
using IRasRag.Application.DTOs;
using IRasRag.Application.Services.Interfaces;
using IRasRag.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace IRasRag.Application.Services.Implementations
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<UserService> _logger;
        private readonly IMapper _mapper;
        private readonly IPasswordHasher _passwordHasher;

        public UserService(
            IUnitOfWork unitOfWork,
            ILogger<UserService> logger,
            IMapper mapper,
            IPasswordHasher passwordHasher
        )
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
            _passwordHasher = passwordHasher;
        }

        public async Task<Result<UserDto>> CreateUserAsync(CreateUserDto createDto)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(createDto.UserName))
                    return Result<UserDto>.Failure(
                        "Tên đăng nhập không được để trống.",
                        ResultType.BadRequest
                    );

                if (string.IsNullOrWhiteSpace(createDto.Email))
                    return Result<UserDto>.Failure(
                        "Email không được để trống.",
                        ResultType.BadRequest
                    );

                if (string.IsNullOrWhiteSpace(createDto.Password))
                    return Result<UserDto>.Failure(
                        "Mật khẩu không được để trống.",
                        ResultType.BadRequest
                    );

                if (createDto.Password.Length < 6)
                    return Result<UserDto>.Failure(
                        "Mật khẩu phải có ít nhất 6 ký tự.",
                        ResultType.BadRequest
                    );

                // Kiểm tra trùng username
                var normalizedUserName = createDto.UserName.Trim().ToLower();
                var existingUserByUsername = await _unitOfWork
                    .GetRepository<User>()
                    .FirstOrDefaultAsync(u =>
                        u.UserName.ToLower() == normalizedUserName && !u.IsDeleted
                    );

                if (existingUserByUsername != null)
                    return Result<UserDto>.Failure(
                        "Tên đăng nhập đã tồn tại.",
                        ResultType.Conflict
                    );

                // Kiểm tra trùng email
                var existingUserByEmail = await _unitOfWork
                    .GetRepository<User>()
                    .FirstOrDefaultAsync(u =>
                        u.Email.ToLower() == createDto.Email.Trim().ToLower() && !u.IsDeleted
                    );

                if (existingUserByEmail != null)
                    return Result<UserDto>.Failure("Email đã tồn tại.", ResultType.Conflict);

                // Tự động gán role "User" như trong Register
                var userRole = await _unitOfWork
                    .GetRepository<Role>()
                    .FirstOrDefaultAsync(r => r.Name == "User");

                if (userRole == null)
                    return Result<UserDto>.Failure(
                        "Không tìm thấy vai trò mặc định.",
                        ResultType.Unexpected
                    );

                var newUser = new User
                {
                    RoleId = userRole.Id,
                    UserName = normalizedUserName,
                    Email = createDto.Email.Trim(),
                    FirstName = createDto.FirstName?.Trim(),
                    LastName = createDto.LastName?.Trim(),
                    PasswordHash = _passwordHasher.HashPassword(createDto.Password),
                    IsVerified = false,
                    IsDeleted = false,
                };

                await _unitOfWork.GetRepository<User>().AddAsync(newUser);
                await _unitOfWork.SaveChangesAsync();

                var resultDto = new UserDto
                {
                    Id = newUser.Id,
                    RoleName = userRole.Name,
                    UserName = newUser.UserName,
                    Email = newUser.Email,
                    FirstName = newUser.FirstName,
                    LastName = newUser.LastName,
                    IsVerified = newUser.IsVerified,
                };

                return Result<UserDto>.Success(resultDto, "Tạo người dùng thành công.");
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
                    return Result.Failure("Người dùng không tồn tại.", ResultType.NotFound);
                }

                // Soft delete
                user.IsDeleted = true;
                user.DeletedAt = DateTime.UtcNow;

                _unitOfWork.GetRepository<User>().Update(user);
                await _unitOfWork.SaveChangesAsync();

                return Result.Success("Xóa người dùng thành công.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xóa người dùng");
                return Result.Failure("Lỗi khi xóa người dùng.", ResultType.Unexpected);
            }
        }

        public async Task<Result<IEnumerable<UserDto>>> GetAllUsersAsync()
        {
            try
            {
                var users = await _unitOfWork.GetRepository<User>().FindAllAsync(u => !u.IsDeleted);
                var roles = await _unitOfWork.GetRepository<Role>().GetAllAsync();

                var userDtos = users
                    .Select(user => new UserDto
                    {
                        Id = user.Id,
                        RoleName =
                            roles.FirstOrDefault(r => r.Id == user.RoleId)?.Name ?? "Unknown",
                        UserName = user.UserName,
                        Email = user.Email,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        IsVerified = user.IsVerified,
                    })
                    .ToList();

                return Result<IEnumerable<UserDto>>.Success(
                    userDtos,
                    "Lấy danh sách người dùng thành công."
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi truy xuất danh sách người dùng");
                return Result<IEnumerable<UserDto>>.Failure(
                    "Lỗi khi truy xuất danh sách người dùng.",
                    ResultType.Unexpected
                );
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

                var dto = new UserDto
                {
                    Id = user.Id,
                    RoleName = role?.Name ?? "Unknown",
                    UserName = user.UserName,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    IsVerified = user.IsVerified,
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

        public async Task<Result> UpdateUserAsync(Guid id, UpdateUserDto dto)
        {
            try
            {
                var user = await _unitOfWork.GetRepository<User>().GetByIdAsync(id);

                if (user == null || user.IsDeleted)
                    return Result.Failure("Người dùng không tồn tại.", ResultType.NotFound);

                if (!string.IsNullOrWhiteSpace(dto.UserName))
                {
                    var usernameToUpdate = dto.UserName.Trim();

                    // Kiểm tra trùng username với người dùng khác
                    var existingUser = await _unitOfWork
                        .GetRepository<User>()
                        .FirstOrDefaultAsync(u =>
                            u.UserName.ToLower() == usernameToUpdate.ToLower()
                            && u.Id != id
                            && !u.IsDeleted
                        );

                    if (existingUser != null)
                        return Result.Failure("Tên đăng nhập đã tồn tại.", ResultType.Conflict);

                    user.UserName = usernameToUpdate;
                }

                if (!string.IsNullOrWhiteSpace(dto.Email))
                {
                    var emailToUpdate = dto.Email.Trim();

                    // Kiểm tra trùng email với người dùng khác
                    var existingUser = await _unitOfWork
                        .GetRepository<User>()
                        .FirstOrDefaultAsync(u =>
                            u.Email.ToLower() == emailToUpdate.ToLower()
                            && u.Id != id
                            && !u.IsDeleted
                        );

                    if (existingUser != null)
                        return Result.Failure("Email đã tồn tại.", ResultType.Conflict);

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
                        return Result.Failure(
                            "Mật khẩu phải có ít nhất 6 ký tự.",
                            ResultType.BadRequest
                        );

                    user.PasswordHash = _passwordHasher.HashPassword(dto.Password);
                }

                // if (dto.IsVerified.HasValue)
                // {
                //     user.IsVerified = dto.IsVerified.Value;
                // }

                _unitOfWork.GetRepository<User>().Update(user);
                await _unitOfWork.SaveChangesAsync();

                return Result.Success("Cập nhật người dùng thành công.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi cập nhật người dùng");
                return Result.Failure("Lỗi khi cập nhật người dùng.", ResultType.Unexpected);
            }
        }
    }
}

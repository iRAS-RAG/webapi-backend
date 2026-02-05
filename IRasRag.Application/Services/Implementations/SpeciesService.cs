using AutoMapper;
using IRasRag.Application.Common.Interfaces;
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
    public class SpeciesService : ISpeciesService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<SpeciesService> _logger;
        private readonly IMapper _mapper;

        public SpeciesService(
            IUnitOfWork unitOfWork,
            ILogger<SpeciesService> logger,
            IMapper mapper
        )
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<Result<SpeciesDto>> CreateSpeciesAsync(CreateSpeciesDto createDto)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(createDto.Name))
                    return Result<SpeciesDto>.Failure(
                        "Tên loài không được để trống.",
                        ResultType.BadRequest
                    );

                // Kiểm tra trùng tên
                var existingSpecies = await _unitOfWork
                    .GetRepository<Species>()
                    .FirstOrDefaultAsync(s => s.Name.ToLower() == createDto.Name.Trim().ToLower());

                if (existingSpecies != null)
                    return Result<SpeciesDto>.Failure(
                        "Loài với tên này đã tồn tại.",
                        ResultType.Conflict
                    );

                var newSpecies = new Species { Name = createDto.Name.Trim() };

                await _unitOfWork.GetRepository<Species>().AddAsync(newSpecies);
                await _unitOfWork.SaveChangesAsync();

                return Result<SpeciesDto>.Success(
                    _mapper.Map<SpeciesDto>(newSpecies),
                    "Tạo loài thành công."
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tạo loài");
                return Result<SpeciesDto>.Failure("Lỗi khi tạo loài.", ResultType.Unexpected);
            }
        }

        public async Task<Result> DeleteSpeciesAsync(Guid id)
        {
            try
            {
                var species = await _unitOfWork.GetRepository<Species>().GetByIdAsync(id);

                if (species == null)
                {
                    return Result.Failure("Loài không tồn tại.", ResultType.NotFound);
                }

                _unitOfWork.GetRepository<Species>().Delete(species);
                await _unitOfWork.SaveChangesAsync();

                return Result.Success("Xóa loài thành công.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xóa loài");
                return Result.Failure("Lỗi khi xóa loài.", ResultType.Unexpected);
            }
        }

        public async Task<PaginatedResult<SpeciesDto>> GetAllSpeciesAsync(int page, int pageSize)
        {
            try
            {
                _logger.LogInformation(
                    "Bắt đầu lấy danh sách loài (Page: {Page}, PageSize: {PageSize})",
                    page,
                    pageSize
                );

                var repository = _unitOfWork.GetRepository<Species>();
                var pagedResult = await repository.GetPagedAsync(page, pageSize);

                var speciesDtos = _mapper.Map<IReadOnlyList<SpeciesDto>>(pagedResult.Items);

                _logger.LogInformation(
                    "Lấy danh sách loài thành công: {Count} loài",
                    speciesDtos.Count
                );

                return new PaginatedResult<SpeciesDto>
                {
                    Message =
                        speciesDtos.Count == 0
                            ? "Không có loài nào"
                            : "Lấy danh sách loài thành công",
                    Data = speciesDtos,
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
                _logger.LogError(ex, "Lỗi khi truy xuất danh sách loài");

                return new PaginatedResult<SpeciesDto>
                {
                    Message = "Lỗi khi truy xuất danh sách loài",
                    Data = Array.Empty<SpeciesDto>(),
                    Meta = null,
                    Links = null,
                };
            }
        }

        public async Task<Result<SpeciesDto>> GetSpeciesByIdAsync(Guid id)
        {
            try
            {
                var species = await _unitOfWork.GetRepository<Species>().GetByIdAsync(id);

                if (species == null)
                    return Result<SpeciesDto>.Failure("Loài không tồn tại.", ResultType.NotFound);

                var dto = new SpeciesDto { Id = species.Id, Name = species.Name };

                return Result<SpeciesDto>.Success(dto, "Lấy thông tin loài thành công.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi truy xuất thông tin loài");
                return Result<SpeciesDto>.Failure(
                    "Lỗi khi truy xuất thông tin loài.",
                    ResultType.Unexpected
                );
            }
        }

        public async Task<Result> UpdateSpeciesAsync(Guid id, UpdateSpeciesDto dto)
        {
            try
            {
                var species = await _unitOfWork.GetRepository<Species>().GetByIdAsync(id);

                if (species == null)
                    return Result.Failure("Loài không tồn tại.", ResultType.NotFound);

                if (!string.IsNullOrWhiteSpace(dto.Name))
                {
                    var nameToUpdate = dto.Name.Trim();

                    // Kiểm tra trùng tên với loài khác
                    var existingSpecies = await _unitOfWork
                        .GetRepository<Species>()
                        .FirstOrDefaultAsync(s =>
                            s.Name.ToLower() == nameToUpdate.ToLower() && s.Id != id
                        );

                    if (existingSpecies != null)
                        return Result.Failure("Loài với tên này đã tồn tại.", ResultType.Conflict);

                    species.Name = nameToUpdate;
                }

                _unitOfWork.GetRepository<Species>().Update(species);
                await _unitOfWork.SaveChangesAsync();

                return Result.Success("Cập nhật loài thành công.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi cập nhật loài");
                return Result.Failure("Lỗi khi cập nhật loài.", ResultType.Unexpected);
            }
        }
    }
}

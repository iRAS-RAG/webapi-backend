using AutoMapper;
using IRasRag.Application.Common.Interfaces;
using IRasRag.Application.Common.Interfaces.Persistence;
using IRasRag.Application.Common.Models;
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

        public async Task<Result<IEnumerable<SpeciesDto>>> GetAllSpeciesAsync()
        {
            try
            {
                var list = await _unitOfWork.GetRepository<Species>().GetAllAsync();

                return Result<IEnumerable<SpeciesDto>>.Success(
                    _mapper.Map<IEnumerable<SpeciesDto>>(list),
                    "Lấy danh sách loài thành công."
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi truy xuất danh sách loài");
                return Result<IEnumerable<SpeciesDto>>.Failure(
                    "Lỗi khi truy xuất danh sách loài.",
                    ResultType.Unexpected
                );
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

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
    public class FeedTypeService : IFeedTypeService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<FeedTypeService> _logger;
        private readonly IMapper _mapper;

        public FeedTypeService(
            IUnitOfWork unitOfWork,
            ILogger<FeedTypeService> logger,
            IMapper mapper
        )
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<Result<FeedTypeDto>> CreateFeedTypeAsync(CreateFeedTypeDto createDto)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(createDto.Name))
                    return Result<FeedTypeDto>.Failure(
                        "Tên loại thức ăn không được để trống.",
                        ResultType.BadRequest
                    );

                if (createDto.WeightPerUnit <= 0)
                    return Result<FeedTypeDto>.Failure(
                        "Trọng lượng mỗi đơn vị phải lớn hơn 0.",
                        ResultType.BadRequest
                    );

                if (createDto.ProteinPercentage < 0 || createDto.ProteinPercentage > 100)
                    return Result<FeedTypeDto>.Failure(
                        "Tỷ lệ protein phải từ 0 đến 100.",
                        ResultType.BadRequest
                    );

                // Kiểm tra trùng tên
                var existingFeedType = await _unitOfWork
                    .GetRepository<FeedType>()
                    .FirstOrDefaultAsync(f => f.Name.ToLower() == createDto.Name.Trim().ToLower());

                if (existingFeedType != null)
                    return Result<FeedTypeDto>.Failure(
                        "Loại thức ăn với tên này đã tồn tại.",
                        ResultType.Conflict
                    );

                var newFeedType = new FeedType
                {
                    Name = createDto.Name.Trim(),
                    Description = createDto.Description?.Trim(),
                    WeightPerUnit = createDto.WeightPerUnit,
                    ProteinPercentage = createDto.ProteinPercentage,
                    Manufacturer = createDto.Manufacturer?.Trim(),
                };

                await _unitOfWork.GetRepository<FeedType>().AddAsync(newFeedType);
                await _unitOfWork.SaveChangesAsync();

                return Result<FeedTypeDto>.Success(
                    _mapper.Map<FeedTypeDto>(newFeedType),
                    "Tạo loại thức ăn thành công."
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tạo loại thức ăn");
                return Result<FeedTypeDto>.Failure(
                    "Lỗi khi tạo loại thức ăn.",
                    ResultType.Unexpected
                );
            }
        }

        public async Task<Result> DeleteFeedTypeAsync(Guid id)
        {
            try
            {
                var feedType = await _unitOfWork.GetRepository<FeedType>().GetByIdAsync(id);

                if (feedType == null)
                {
                    return Result.Failure("Loại thức ăn không tồn tại.", ResultType.NotFound);
                }

                _unitOfWork.GetRepository<FeedType>().Delete(feedType);
                await _unitOfWork.SaveChangesAsync();

                return Result.Success("Xóa loại thức ăn thành công.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xóa loại thức ăn");
                return Result.Failure("Lỗi khi xóa loại thức ăn.", ResultType.Unexpected);
            }
        }

        public async Task<Result<IEnumerable<FeedTypeDto>>> GetAllFeedTypesAsync()
        {
            try
            {
                var list = await _unitOfWork.GetRepository<FeedType>().GetAllAsync();

                return Result<IEnumerable<FeedTypeDto>>.Success(
                    _mapper.Map<IEnumerable<FeedTypeDto>>(list),
                    "Lấy danh sách loại thức ăn thành công."
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi truy xuất danh sách loại thức ăn");
                return Result<IEnumerable<FeedTypeDto>>.Failure(
                    "Lỗi khi truy xuất danh sách loại thức ăn.",
                    ResultType.Unexpected
                );
            }
        }

        public async Task<Result<FeedTypeDto>> GetFeedTypeByIdAsync(Guid id)
        {
            try
            {
                var feedType = await _unitOfWork.GetRepository<FeedType>().GetByIdAsync(id);

                if (feedType == null)
                    return Result<FeedTypeDto>.Failure(
                        "Loại thức ăn không tồn tại.",
                        ResultType.NotFound
                    );

                var dto = new FeedTypeDto
                {
                    Id = feedType.Id,
                    Name = feedType.Name,
                    Description = feedType.Description,
                    WeightPerUnit = feedType.WeightPerUnit,
                    ProteinPercentage = feedType.ProteinPercentage,
                    Manufacturer = feedType.Manufacturer,
                };

                return Result<FeedTypeDto>.Success(dto, "Lấy thông tin loại thức ăn thành công.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi truy xuất thông tin loại thức ăn");
                return Result<FeedTypeDto>.Failure(
                    "Lỗi khi truy xuất thông tin loại thức ăn.",
                    ResultType.Unexpected
                );
            }
        }

        public async Task<Result> UpdateFeedTypeAsync(Guid id, UpdateFeedTypeDto dto)
        {
            try
            {
                var feedType = await _unitOfWork.GetRepository<FeedType>().GetByIdAsync(id);

                if (feedType == null)
                    return Result.Failure("Loại thức ăn không tồn tại.", ResultType.NotFound);

                if (!string.IsNullOrWhiteSpace(dto.Name))
                {
                    var nameToUpdate = dto.Name.Trim();

                    // Kiểm tra trùng tên với loại thức ăn khác
                    var existingFeedType = await _unitOfWork
                        .GetRepository<FeedType>()
                        .FirstOrDefaultAsync(f =>
                            f.Name.ToLower() == nameToUpdate.ToLower() && f.Id != id
                        );

                    if (existingFeedType != null)
                        return Result.Failure(
                            "Loại thức ăn với tên này đã tồn tại.",
                            ResultType.Conflict
                        );

                    feedType.Name = nameToUpdate;
                }

                if (!string.IsNullOrWhiteSpace(dto.Description))
                {
                    feedType.Description = dto.Description.Trim();
                }

                if (dto.WeightPerUnit.HasValue)
                {
                    if (dto.WeightPerUnit.Value <= 0)
                        return Result.Failure(
                            "Trọng lượng mỗi đơn vị phải lớn hơn 0.",
                            ResultType.BadRequest
                        );

                    feedType.WeightPerUnit = dto.WeightPerUnit.Value;
                }

                if (dto.ProteinPercentage.HasValue)
                {
                    if (dto.ProteinPercentage.Value < 0 || dto.ProteinPercentage.Value > 100)
                        return Result.Failure(
                            "Tỷ lệ protein phải từ 0 đến 100.",
                            ResultType.BadRequest
                        );

                    feedType.ProteinPercentage = dto.ProteinPercentage.Value;
                }

                if (!string.IsNullOrWhiteSpace(dto.Manufacturer))
                {
                    feedType.Manufacturer = dto.Manufacturer.Trim();
                }

                _unitOfWork.GetRepository<FeedType>().Update(feedType);
                await _unitOfWork.SaveChangesAsync();

                return Result.Success("Cập nhật loại thức ăn thành công.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi cập nhật loại thức ăn");
                return Result.Failure("Lỗi khi cập nhật loại thức ăn.", ResultType.Unexpected);
            }
        }
    }
}

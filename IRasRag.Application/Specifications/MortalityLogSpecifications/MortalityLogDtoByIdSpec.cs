using Ardalis.Specification;
using IRasRag.Application.DTOs;
using IRasRag.Domain.Entities;

namespace IRasRag.Application.Specifications.MortalityLogSpecifications
{
    /// <summary>
    /// Specification chiếu một MortalityLog theo Id thành MortalityLogDto,
    /// bao gồm cả thông tin FarmingBatch.
    /// </summary>
    public class MortalityLogDtoByIdSpec : Specification<MortalityLog, MortalityLogDto>
    {
        public MortalityLogDtoByIdSpec(Guid id)
        {
            Query
                .AsNoTracking()
                .Where(ml => ml.Id == id)
                .Select(ml => new MortalityLogDto
                {
                    Id = ml.Id,
                    BatchId = ml.BatchId,
                    BatchName = ml.Batch.Name,
                    UserId = ml.UserId,
                    UserEmail = ml.User.Email,
                    Quantity = ml.Quantity,
                    LostWeightKg = ml.LostWeightKg,
                    Date = ml.Date,
                    CreatedAt = ml.CreatedAt,
                    ModifiedAt = ml.ModifiedAt,
                });
        }
    }
}

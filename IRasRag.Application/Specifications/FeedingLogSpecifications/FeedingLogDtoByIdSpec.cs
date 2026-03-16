using Ardalis.Specification;
using IRasRag.Application.DTOs;
using IRasRag.Domain.Entities;

namespace IRasRag.Application.Specifications.FeedingLogSpecifications
{
    /// <summary>
    /// Specification chiếu một FeedingLog theo Id thành FeedingLogDto,
    /// bao gồm cả thông tin FarmingBatch.
    /// </summary>
    public class FeedingLogDtoByIdSpec : Specification<FeedingLog, FeedingLogDto>
    {
        public FeedingLogDtoByIdSpec(Guid id)
        {
            Query
                .AsNoTracking()
                .Where(fl => fl.Id == id)
                .Select(fl => new FeedingLogDto
                {
                    Id = fl.Id,
                    FarmingBatchId = fl.FarmingBatchId,
                    FarmingBatchName = fl.FarmingBatch.Name,
                    UserId = fl.UserId,
                    UserEmail = fl.User.Email,
                    Amount = fl.Amount,
                    CreatedDate = fl.CreatedDate,
                    CreatedAt = fl.CreatedAt,
                    ModifiedAt = fl.ModifiedAt,
                });
        }
    }
}

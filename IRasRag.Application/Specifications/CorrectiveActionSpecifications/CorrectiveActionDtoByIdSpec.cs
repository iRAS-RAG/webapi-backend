using Ardalis.Specification;
using IRasRag.Application.DTOs;
using IRasRag.Domain.Entities;

namespace IRasRag.Application.Specifications.CorrectiveActionSpecifications
{
    /// <summary>
    /// Specification chiếu một CorrectiveAction theo Id thành CorrectiveActionDto,
    /// bao gồm cả thông tin User.
    /// </summary>
    public class CorrectiveActionDtoByIdSpec : Specification<CorrectiveAction, CorrectiveActionDto>
    {
        public CorrectiveActionDtoByIdSpec(Guid id)
        {
            Query
                .AsNoTracking()
                .Where(ca => ca.Id == id)
                .Select(ca => new CorrectiveActionDto
                {
                    Id = ca.Id,
                    AlertId = ca.AlertId,
                    UserId = ca.UserId,
                    UserEmail = ca.User.Email,
                    PerformedBy = ca.User.FirstName + " " + ca.User.LastName,
                    ActionTaken = ca.ActionTaken,
                    Notes = ca.Notes,
                    Timestamp = ca.Timestamp,
                });
        }
    }
}

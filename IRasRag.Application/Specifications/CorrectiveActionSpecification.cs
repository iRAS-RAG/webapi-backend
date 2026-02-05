using Ardalis.Specification;
using IRasRag.Application.DTOs;
using IRasRag.Domain.Entities;

namespace IRasRag.Application.Specifications
{
    public class CorrectiveActionDtoListSpec : Specification<CorrectiveAction, CorrectiveActionDto>
    {
        public CorrectiveActionDtoListSpec()
        {
            Query
                .AsNoTracking()
                .Include(ca => ca.User)
                .Select(ca => new CorrectiveActionDto
                {
                    Id = ca.Id,
                    AlertId = ca.AlertId,
                    UserId = ca.UserId,
                    UserEmail = ca.User.Email,
                    ActionTaken = ca.ActionTaken,
                    Notes = ca.Notes,
                    Timestamp = ca.Timestamp,
                });
        }
    }
}

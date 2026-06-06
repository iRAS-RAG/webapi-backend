using System.Linq.Expressions;
using Ardalis.Specification;
using IRasRag.Application.DTOs;
using IRasRag.Application.Specifications.Base;
using IRasRag.Domain.Entities;

namespace IRasRag.Application.Specifications.CorrectiveActionSpecifications
{
    public class CorrectiveActionDtoListSpec : BaseListSpec<CorrectiveAction, CorrectiveActionDto>
    {
        public CorrectiveActionDtoListSpec(CorrectiveActionListRequest request)
        {
            Query.AsNoTracking();

            var sortKeyMap = new Dictionary<string, Expression<Func<CorrectiveAction, object?>>>
            {
                ["timestamp"] = ca => ca.Timestamp,
            };

            ApplySearch(request.SearchTerm, [ca => ca.User.FirstName, ca => ca.User.LastName]);

            ApplySort(request.SortBy, request.SortDir, sortKeyMap, defaultSortKey: "timestamp");

            Query.Select(ca => new CorrectiveActionDto
            {
                Id = ca.Id,
                AlertId = ca.AlertId,
                UserId = ca.UserId,
                UserEmail = ca.User.Email,
                PerformedBy = ca.User.LastName + " " + ca.User.FirstName,
                ActionTaken = ca.ActionTaken,
                Notes = ca.Notes,
                Timestamp = ca.Timestamp,
                SensorTypeName =
                    ca.Alert != null && ca.Alert.SensorType != null
                        ? ca.Alert.SensorType.Name
                        : string.Empty,
                FishTankName =
                    ca.Alert != null && ca.Alert.FishTank != null
                        ? ca.Alert.FishTank.Name
                        : string.Empty,
            });
        }
    }
}

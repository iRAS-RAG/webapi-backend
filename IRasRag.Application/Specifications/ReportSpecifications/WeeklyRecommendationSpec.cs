using Ardalis.Specification;
using IRasRag.Application.DTOs;
using IRasRag.Domain.Entities;

namespace IRasRag.Application.Specifications.ReportSpecifications
{
    /// <summary>
    /// Projects Recommendation entities whose linked Alert was raised within the given period
    /// and belongs to the given fish tanks.
    /// Includes the Document title for reporting purposes.
    /// </summary>
    public class WeeklyRecommendationSpec : Specification<Recommendation, WeeklyRecommendationItem>
    {
        public WeeklyRecommendationSpec(DateTime from, DateTime to, IEnumerable<Guid> tankIds)
        {
            Query
                .AsNoTracking()
                .Where(r => r.Alert.RaisedAt >= from && r.Alert.RaisedAt <= to
                            && tankIds.Contains(r.Alert.FishTankId))
                .OrderByDescending(r => r.Alert.RaisedAt)
                .Select(r => new WeeklyRecommendationItem
                {
                    Id = r.Id,
                    AlertId = r.AlertId,
                    SuggestionText = r.SuggestionText,
                    DocumentTitle = r.Document != null ? r.Document.Title : string.Empty,
                });
        }
    }
}

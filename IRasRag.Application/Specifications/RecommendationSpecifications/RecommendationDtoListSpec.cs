using System.Linq.Expressions;
using Ardalis.Specification;
using IRasRag.Application.DTOs;
using IRasRag.Application.Specifications.Base;
using IRasRag.Domain.Entities;

namespace IRasRag.Application.Specifications.RecommendationSpecifications
{
    public class RecommendationDtoListSpec : BaseListSpec<Recommendation, RecommendationDto>
    {
        public RecommendationDtoListSpec(RecommendationListRequest request)
        {
            Query.AsNoTracking();

            var sortMap = new Dictionary<string, Expression<Func<Recommendation, object?>>>
            {
                ["documenttitle"] = r => r.Document.Title,
            };

            ApplySearch(request.SearchTerm, [r => r.SuggestionText, r => r.Document.Title]);

            ApplySort(request.SortBy, request.SortDir, sortMap, defaultSortKey: "documenttitle");

            Query.Select(r => new RecommendationDto
            {
                Id = r.Id,
                AlertId = r.AlertId,
                DocumentId = r.DocumentId,
                DocumentTitle = r.Document != null ? r.Document.Title : string.Empty,
                SuggestionText = r.SuggestionText,
            });
        }
    }
}

using Ardalis.Specification;
using IRasRag.Application.DTOs;
using IRasRag.Domain.Entities;

namespace IRasRag.Application.Specifications.RecommendationSpecifications
{
    /// <summary>
    /// Specification chiếu một Recommendation theo Id thành RecommendationDto,
    /// bao gồm cả thông tin Document (DocumentTitle).
    /// </summary>
    public class RecommendationDtoByIdSpec : Specification<Recommendation, RecommendationDto>
    {
        public RecommendationDtoByIdSpec(Guid id)
        {
            Query
                .AsNoTracking()
                .Where(r => r.Id == id)
                .Select(r => new RecommendationDto
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

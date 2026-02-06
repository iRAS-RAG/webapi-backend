using Ardalis.Specification;
using IRasRag.Application.DTOs;
using IRasRag.Domain.Entities;

namespace IRasRag.Application.Specifications
{
    public class RecommendationDtoListSpec : Specification<Recommendation, RecommendationDto>
    {
        public RecommendationDtoListSpec()
        {
            Query
                .AsNoTracking()
                .Include(r => r.Document)
                .Select(r => new RecommendationDto
                {
                    Id = r.Id,
                    AlertId = r.AlertId,
                    DocumentId = r.DocumentId,
                    DocumentTitle = r.Document.Title,
                    SuggestionText = r.SuggestionText,
                });
        }
    }
}

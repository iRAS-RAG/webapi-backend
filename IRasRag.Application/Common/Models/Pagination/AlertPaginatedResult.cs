using IRasRag.Application.DTOs;

namespace IRasRag.Application.Common.Models.Pagination
{
    public class AlertPaginatedResult : PaginatedResult<AlertDto>
    {
        public AlertStatusCounts StatusCounts { get; set; } = new();
    }
}

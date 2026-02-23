using System.ComponentModel.DataAnnotations;

namespace IRasRag.Application.DTOs
{
    public class BasePaginatedListRequest
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? SearchTerm { get; set; }
        public string? SortBy { get; set; }

        [RegularExpression("^(asc|desc)$", ErrorMessage = "SortDir must be 'asc' or 'desc'.")]
        public string SortDir { get; set; } = "asc";
    }
}

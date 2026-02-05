namespace IRasRag.Application.Common.Models.Pagination
{
    public class PaginatedResult<T>
    {
        public string Message { get; set; } = string.Empty;

        public IReadOnlyList<T>? Data { get; set; }

        public PaginationMeta? Meta { get; set; }

        public PaginationLinks? Links { get; set; }
    }
}

namespace IRasRag.Application.Common.Models.Pagination
{
    public sealed class PagedResult<T>
    {
        public IReadOnlyList<T> Items { get; init; } = [];
        public int TotalItems { get; init; }
    }
}

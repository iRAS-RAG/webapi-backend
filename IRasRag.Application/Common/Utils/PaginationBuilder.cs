using IRasRag.Application.Common.Models.Pagination;

namespace IRasRag.Application.Common.Utils
{
    public class PaginationBuilder
    {
        public static PaginationLinks BuildPaginationLinks(
            int currentPage,
            int pageSize,
            int totalItems
        )
        {
            var totalPages = CalculateTotalPages(totalItems, pageSize);
            string BuildUrl(int page) => $"/items?page={page}&pageSize={pageSize}";
            return new PaginationLinks
            {
                Self = BuildUrl(currentPage),
                First = BuildUrl(1),
                Prev = currentPage > 1 ? BuildUrl(currentPage - 1) : null,
                Next = currentPage < totalPages ? BuildUrl(currentPage + 1) : null,
                Last = BuildUrl(totalPages),
            };
        }

        public static PaginationMeta BuildPaginationMetadata(
            int currentPage,
            int pageSize,
            int totalItems
        )
        {
            var totalPages = CalculateTotalPages(totalItems, pageSize);
            return new PaginationMeta
            {
                Page = currentPage,
                PageSize = pageSize,
                TotalItems = totalItems,
                TotalPages = totalPages,
            };
        }

        private static int CalculateTotalPages(int totalItems, int pageSize)
        {
            return (int)Math.Ceiling((double)totalItems / pageSize);
        }
    }
}

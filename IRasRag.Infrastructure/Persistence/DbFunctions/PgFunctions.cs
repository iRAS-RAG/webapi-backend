using Microsoft.EntityFrameworkCore;

namespace IRasRag.Infrastructure.Persistence.DbFunctions
{
    public static class PgFunctions
    {
        /// <summary>
        /// Maps to PostgreSQL date_bin(stride, source, origin).
        /// Buckets <paramref name="source"/> into fixed-width intervals of
        /// <paramref name="stride"/> aligned to <paramref name="origin"/>.
        /// Never call directly — EF Core translates this to SQL.
        /// </summary>
        public static DateTime DateBin(TimeSpan stride, DateTime source, DateTime origin) =>
            throw new InvalidOperationException("Only callable in EF Core LINQ queries.");
    }
}

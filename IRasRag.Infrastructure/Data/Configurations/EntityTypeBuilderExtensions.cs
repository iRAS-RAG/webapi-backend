using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IRasRag.Infrastructure.Data.Configurations
{
    public static class EntityTypeBuilderExtensions
    {
        public static void ConfigureTimestamps<T>(this EntityTypeBuilder<T> builder)
            where T : class
        {
            builder.Property("CreatedAt").HasDefaultValueSql("CURRENT_TIMESTAMP");
            builder.Property("ModifiedAt").HasDefaultValueSql("CURRENT_TIMESTAMP");
        }
    }
}

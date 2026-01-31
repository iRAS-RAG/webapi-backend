using IRasRag.Domain.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IRasRag.Infrastructure.Data.Configurations
{
    public static class EntityTypeBuilderExtensions
    {
        public static void ConfigureTimestamps<T>(this EntityTypeBuilder<T> builder)
            where T : BaseEntity
        {
            builder.Property(e => e.CreatedAt).IsRequired(false);

            builder.Property(e => e.ModifiedAt).IsRequired(false);

            builder.Property(e => e.DeletedAt).IsRequired(false);
        }
    }
}

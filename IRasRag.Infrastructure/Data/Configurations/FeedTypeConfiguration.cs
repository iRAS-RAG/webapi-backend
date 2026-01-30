using IRasRag.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace IRasRag.Infrastructure.Data.Configurations
{
    public class FeedTypeConfiguration : IEntityTypeConfiguration<FeedType>
    {
        public void Configure(EntityTypeBuilder<FeedType> builder)
        {
            builder.ConfigureTimestamps();

            builder.HasData(FeedTypeSeed.FeedTypes);

        }
    }
}

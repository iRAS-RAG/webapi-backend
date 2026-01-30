using IRasRag.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using IRasRag.Infrastructure.Data.Seeds;

namespace IRasRag.Infrastructure.Data.Configurations
{
    public class FarmConfiguration : IEntityTypeConfiguration<Farm>
    {
        public void Configure(EntityTypeBuilder<Farm> builder)
        {
            builder.ConfigureTimestamps();

            builder.HasQueryFilter(f => !f.IsDeleted);

            builder.HasData(FarmSeed.Farms);
        }
    }
}

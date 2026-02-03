using IRasRag.Domain.Entities;
using IRasRag.Infrastructure.Data.Seeds;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IRasRag.Infrastructure.Data.Configurations
{
    public class FarmConfiguration : IEntityTypeConfiguration<Farm>
    {
        public void Configure(EntityTypeBuilder<Farm> builder)
        {
            builder.ConfigureTimestamps();
            builder.ConfigureSoftDelete();

            builder.HasQueryFilter(f => !f.IsDeleted);

            builder.HasData(FarmSeed.Farms);
        }
    }
}

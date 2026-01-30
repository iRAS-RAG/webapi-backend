using IRasRag.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using IRasRag.Infrastructure.Data.Seeds;

namespace IRasRag.Infrastructure.Data.Configurations
{
    public class SpeciesConfiguration : IEntityTypeConfiguration<Species>
    {
        public void Configure(EntityTypeBuilder<Species> builder)
        {
            builder.ConfigureTimestamps();

            builder.HasData(SpeciesSeed.Species);
        }
    }
}

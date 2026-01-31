using IRasRag.Domain.Entities;
using IRasRag.Infrastructure.Data.Seeds;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

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

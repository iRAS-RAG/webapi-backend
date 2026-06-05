using IRasRag.Domain.Entities;
using IRasRag.Infrastructure.Data.Seeds;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IRasRag.Infrastructure.Data.Configurations
{
    public class GrowthStageConfiguration : IEntityTypeConfiguration<GrowthStage>
    {
        public void Configure(EntityTypeBuilder<GrowthStage> builder)
        {
            builder.ConfigureTimestamps();
            // Ensure stage names are unique per species
            builder.HasIndex(gs => new { gs.SpeciesId, gs.Name }).IsUnique();

            builder
                .HasOne(gs => gs.Species)
                .WithMany(s => s.GrowthStages)
                .HasForeignKey(gs => gs.SpeciesId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasData(GrowthStageSeed.GrowthStages);
        }
    }
}

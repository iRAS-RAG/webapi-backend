using IRasRag.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using IRasRag.Infrastructure.Data.Seeds;

namespace IRasRag.Infrastructure.Data.Configurations
{
    public class SpeciesStageConfigConfiguration : IEntityTypeConfiguration<SpeciesStageConfig>
    {
        public void Configure(EntityTypeBuilder<SpeciesStageConfig> builder)
        {
            builder.ConfigureTimestamps();

            builder
                .HasOne(ssc => ssc.Species)
                .WithMany(s => s.SpeciesStageConfigs)
                .HasForeignKey(ssc => ssc.SpeciesId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .HasOne(ssc => ssc.GrowthStage)
                .WithMany(gs => gs.SpeciesStageConfigs)
                .HasForeignKey(ssc => ssc.GrowthStageId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .HasOne(ssc => ssc.FeedType)
                .WithMany(ft => ft.SpeciesStageConfigs)
                .HasForeignKey(ssc => ssc.FeedTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            // One config per species/stage combination
            builder.HasIndex(ssc => new { ssc.SpeciesId, ssc.GrowthStageId })
                .IsUnique();

            builder.HasData(SpeciesStageConfigSeed.SpeciesStageConfigs);
        }
    }
}

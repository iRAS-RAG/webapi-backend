using IRasRag.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace IRasRag.Infrastructure.Data.Configurations
{
    public class SpeciesThresholdConfiguration : IEntityTypeConfiguration<SpeciesThreshold>
    {
        public void Configure(EntityTypeBuilder<SpeciesThreshold> builder)
        {
            builder.ConfigureTimestamps();

            builder
                .HasOne(st => st.Species)
                .WithMany(s => s.SpeciesThresholds)
                .HasForeignKey(st => st.SpeciesId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .HasOne(st => st.GrowthStage)
                .WithMany(gs => gs.SpeciesThresholds)
                .HasForeignKey(st => st.GrowthStageId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .HasOne(st => st.SensorType)
                .WithMany(s => s.SpeciesThresholds)
                .HasForeignKey(st => st.SensorTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            // Unique constraint: one threshold config per species/stage/sensor combo
            builder.HasIndex(st => new { st.SpeciesId, st.GrowthStageId, st.SensorTypeId })
                .IsUnique();
        }
    }
}

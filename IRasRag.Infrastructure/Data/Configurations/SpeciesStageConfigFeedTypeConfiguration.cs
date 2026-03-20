using IRasRag.Domain.Entities;
using IRasRag.Infrastructure.Data.Seeds;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IRasRag.Infrastructure.Data.Configurations
{
    public class SpeciesStageConfigFeedTypeConfiguration
        : IEntityTypeConfiguration<SpeciesStageConfigFeedType>
    {
        public void Configure(EntityTypeBuilder<SpeciesStageConfigFeedType> builder)
        {
            builder.ToTable("species_stage_config_feed_types");

            builder.HasKey(x => new { x.SpeciesStageConfigId, x.FeedTypeId });

            builder.Property(x => x.SpeciesStageConfigId).HasColumnName("species_stage_config_id");
            builder.Property(x => x.FeedTypeId).HasColumnName("feed_type_id");

            builder
                .HasOne(x => x.SpeciesStageConfig)
                .WithMany(x => x.SpeciesStageConfigFeedTypes)
                .HasForeignKey(x => x.SpeciesStageConfigId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .HasOne(x => x.FeedType)
                .WithMany(x => x.SpeciesStageConfigFeedTypes)
                .HasForeignKey(x => x.FeedTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(x => x.FeedTypeId);

            builder.HasData(SpeciesStageConfigFeedTypeSeed.SpeciesStageConfigFeedTypes);
        }
    }
}
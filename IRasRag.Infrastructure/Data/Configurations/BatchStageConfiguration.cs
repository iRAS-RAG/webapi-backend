using IRasRag.Domain.Entities;
using IRasRag.Infrastructure.Data.Seeds;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IRasRag.Infrastructure.Data.Configurations
{
    public class BatchStageConfiguration : IEntityTypeConfiguration<BatchStage>
    {
        public void Configure(EntityTypeBuilder<BatchStage> builder)
        {
            builder.ConfigureTimestamps();

            builder
                .HasOne(bs => bs.FarmingBatch)
                .WithMany(fb => fb.BatchStages)
                .HasForeignKey(bs => bs.FarmingBatchId)
                .OnDelete(DeleteBehavior.Cascade);

            builder
                .HasOne(bs => bs.SpeciesStageConfig)
                .WithMany(ssc => ssc.BatchStages)
                .HasForeignKey(bs => bs.SpeciesStageConfigId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(bs => bs.Sequence).IsRequired();
            builder.HasIndex(bs => new { bs.FarmingBatchId, bs.Sequence }).IsUnique();

            builder.HasData(BatchStageSeed.BatchStages);
        }
    }
}

using IRasRag.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IRasRag.Infrastructure.Data.Configurations
{
    public class FeedingLogConfiguration : IEntityTypeConfiguration<FeedingLog>
    {
        public void Configure(EntityTypeBuilder<FeedingLog> builder)
        {
            builder.ConfigureTimestamps();

            builder
                .HasOne(fl => fl.FarmingBatch)
                .WithMany(fb => fb.FeedingLogs)
                .HasForeignKey(fl => fl.FarmingBatchId)
                .OnDelete(DeleteBehavior.Restrict);

            // Time-series query: feeding history per batch
            builder.HasIndex(fl => new { fl.FarmingBatchId, fl.CreatedDate });
        }
    }
}

using IRasRag.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IRasRag.Infrastructure.Data.Configurations
{
    public class AlertConfiguration : IEntityTypeConfiguration<Alert>
    {
        public void Configure(EntityTypeBuilder<Alert> builder)
        {
            builder.ConfigureTimestamps();

            builder
                .HasOne(a => a.SensorLog)
                .WithMany(sl => sl.Alerts)
                .HasForeignKey(a => a.SensorLogId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .HasOne(a => a.SpeciesThreshold)
                .WithMany(st => st.Alerts)
                .HasForeignKey(a => a.SpeciesThresholdId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .HasOne(a => a.FarmingBatch)
                .WithMany(fb => fb.Alerts)
                .HasForeignKey(a => a.FarmingBatchId)
                .OnDelete(DeleteBehavior.SetNull);

            builder
                .HasOne(a => a.FishTank)
                .WithMany(ft => ft.Alerts)
                .HasForeignKey(a => a.FishTankId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .HasOne(a => a.SensorType)
                .WithMany(st => st.Alerts)
                .HasForeignKey(a => a.SensorTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(a => a.Status).HasConversion<string>();

            // Filter alerts by tank
            builder.HasIndex(a => new { a.FishTankId, a.Status });
            builder.HasIndex(a => new { a.FarmingBatchId, a.Status });
            builder.HasIndex(a => new { a.SensorLogId, a.Status });
        }
    }
}

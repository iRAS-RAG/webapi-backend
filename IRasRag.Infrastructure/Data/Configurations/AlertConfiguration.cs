using IRasRag.Domain.Entities;
using IRasRag.Infrastructure.Data.Seeds;
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
                .HasOne(a => a.Sensor)
                .WithMany(s => s.Alerts)
                .HasForeignKey(a => a.SensorId)
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

            builder.Property(a => a.TriggerValue).HasColumnType("double precision").IsRequired();
            builder.Property(a => a.RaisedAt).IsRequired();
            builder.Property(a => a.Status).HasConversion<string>();

            builder.HasIndex(a => new { a.FishTankId, a.Status });
            builder.HasIndex(a => new { a.FarmingBatchId, a.Status });
            builder.HasIndex(a => new { a.SensorId, a.Status });

            // Unique index to prevent multiple open alerts of same type for same tank/batch
            builder
                .HasIndex(a => new
                {
                    a.FishTankId,
                    a.FarmingBatchId,
                    a.SensorTypeId,
                })
                .HasFilter(
                    "\"status\" IN ('OPEN', 'ACKNOWLEDGED') AND \"farming_batch_id\" IS NOT NULL"
                )
                .IsUnique();

            // Also allow one open alert per sensor type at tank level if no batch is active
            builder
                .HasIndex(a => new { a.FishTankId, a.SensorTypeId })
                .HasFilter(
                    "\"status\" IN ('OPEN', 'ACKNOWLEDGED') AND \"farming_batch_id\" IS NULL"
                )
                .IsUnique();

            builder.HasData(AlertSeed.Alerts);
        }
    }
}

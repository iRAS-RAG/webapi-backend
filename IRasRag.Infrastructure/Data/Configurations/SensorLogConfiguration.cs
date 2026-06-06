using IRasRag.Domain.Entities;
using IRasRag.Infrastructure.Data.Seeds;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IRasRag.Infrastructure.Data.Configurations
{
    public class SensorLogConfiguration : IEntityTypeConfiguration<SensorLog>
    {
        public void Configure(EntityTypeBuilder<SensorLog> builder)
        {
            builder.ConfigureTimestamps();

            builder
                .HasOne(sl => sl.Sensor)
                .WithMany(s => s.SensorLogs)
                .HasForeignKey(sl => sl.SensorId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Property(sl => sl.Average).HasColumnType("double precision").IsRequired();
            builder.Property(sl => sl.Min).HasColumnType("double precision").IsRequired();
            builder.Property(sl => sl.Max).HasColumnType("double precision").IsRequired();
            builder.Property(sl => sl.SampleCount).IsRequired();
            builder.Property(sl => sl.HasWarning).IsRequired();
            builder.Property(sl => sl.PeriodStart).IsRequired();

            // One aggregate row per sensor per window — also the primary query index
            builder.HasIndex(sl => new { sl.SensorId, sl.PeriodStart }).IsUnique();
        }
    }
}

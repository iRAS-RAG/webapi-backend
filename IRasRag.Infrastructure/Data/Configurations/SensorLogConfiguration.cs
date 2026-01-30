using IRasRag.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

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


            builder.Property(sl => sl.DataJson)
                .IsRequired()
                .HasColumnType("jsonb")
                .HasDefaultValue("{}");

            builder.Property(sl => sl.Data)
                .HasColumnType("double precision");

            // Get sensor readings over time
            builder.HasIndex(sl => new { sl.SensorId, sl.CreatedAt });
        }
    }
}

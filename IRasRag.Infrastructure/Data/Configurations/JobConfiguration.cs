using IRasRag.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace IRasRag.Infrastructure.Data.Configurations
{
    public class JobConfiguration : IEntityTypeConfiguration<Job>
    {
        public void Configure(EntityTypeBuilder<Job> builder)
        {
            builder.ConfigureTimestamps();

            builder.HasQueryFilter(j => !j.IsDeleted);

            builder
                .HasOne(j => j.JobType)
                .WithMany(jt => jt.Jobs)
                .HasForeignKey(j => j.JobTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .HasOne(j => j.Sensor)
                .WithMany(s => s.Jobs)
                .HasForeignKey(j => j.SensorId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.Property(j => j.StartTime)
                .HasColumnType("time");

            builder.Property(j => j.EndTime)
                .HasColumnType("time");

            builder.HasIndex(j => j.SensorId);
        }
    }
}

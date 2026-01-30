using IRasRag.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace IRasRag.Infrastructure.Data.Configurations
{
    public class JobControlMappingConfiguration : IEntityTypeConfiguration<JobControlMapping>
    {
        public void Configure(EntityTypeBuilder<JobControlMapping> builder)
        {
            builder.ConfigureTimestamps();

            builder
                .HasOne(jcm => jcm.Job)
                .WithMany(j => j.JobControlMappings)
                .HasForeignKey(jcm => jcm.JobId)
                .OnDelete(DeleteBehavior.Cascade);

            builder
                .HasOne(jcm => jcm.ControlDevice)
                .WithMany(cd => cd.JobControlMappings)
                .HasForeignKey(jcm => jcm.ControlDeviceId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Property(jcm => jcm.TriggerCondition)
                .HasConversion<string>();

            builder.HasIndex(jcm => new { jcm.JobId, jcm.ControlDeviceId }).IsUnique();
        }
    }
}

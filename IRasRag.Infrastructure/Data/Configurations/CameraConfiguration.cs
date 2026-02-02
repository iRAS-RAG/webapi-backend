using IRasRag.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IRasRag.Infrastructure.Data.Configurations
{
    public class CameraConfiguration : IEntityTypeConfiguration<Camera>
    {
        public void Configure(EntityTypeBuilder<Camera> builder)
        {
            builder.ConfigureTimestamps();
            builder.ConfigureSoftDelete();

            builder.HasQueryFilter(c => !c.IsDeleted);

            builder
                .HasOne(c => c.Farm)
                .WithMany(f => f.Cameras)
                .HasForeignKey(c => c.FarmId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}

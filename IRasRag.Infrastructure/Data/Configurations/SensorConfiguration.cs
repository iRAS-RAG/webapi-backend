using IRasRag.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace IRasRag.Infrastructure.Data.Configurations
{
    public class SensorConfiguration : IEntityTypeConfiguration<Sensor>
    {
        public void Configure(EntityTypeBuilder<Sensor> builder)
        {
            builder.ConfigureTimestamps();

            builder.HasQueryFilter(s => !s.IsDeleted);

            builder
                .HasOne(s => s.SensorType)
                .WithMany(st => st.Sensors)
                .HasForeignKey(s => s.SensorTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .HasOne(s => s.MasterBoard)
                .WithMany(mb => mb.Sensors)
                .HasForeignKey(s => s.MasterBoardId)
                .OnDelete(DeleteBehavior.Restrict);

            // List sensors by board
            builder.HasIndex(s => new { s.MasterBoardId, s.IsDeleted });
            // Unique hardware constraint
            builder.HasIndex(s => new { s.MasterBoardId, s.PinCode }).IsUnique();
        }
    }
}

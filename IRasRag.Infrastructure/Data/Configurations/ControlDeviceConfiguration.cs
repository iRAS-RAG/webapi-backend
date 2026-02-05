using IRasRag.Domain.Entities;
using IRasRag.Infrastructure.Data.Seeds;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IRasRag.Infrastructure.Data.Configurations
{
    public class ControlDeviceConfiguration : IEntityTypeConfiguration<ControlDevice>
    {
        public void Configure(EntityTypeBuilder<ControlDevice> builder)
        {
            builder.ConfigureTimestamps();
            builder.ConfigureSoftDelete();

            builder.HasQueryFilter(cd => !cd.IsDeleted);

            builder
                .HasOne(cd => cd.MasterBoard)
                .WithMany(mb => mb.ControlDevices)
                .HasForeignKey(cd => cd.MasterBoardId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .HasOne(cd => cd.ControlDeviceType)
                .WithMany(cdt => cdt.ControlDevices)
                .HasForeignKey(cd => cd.ControlDeviceTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            // List control devices by board
            builder.HasIndex(cd => new { cd.MasterBoardId, cd.IsDeleted });
            // Unique hardware constraint
            builder.HasIndex(cd => new { cd.MasterBoardId, cd.PinCode }).IsUnique();

            builder.HasData(ControlDeviceSeed.ControlDevices);
        }
    }
}

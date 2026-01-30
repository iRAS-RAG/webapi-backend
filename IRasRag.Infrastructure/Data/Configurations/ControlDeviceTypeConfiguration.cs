using IRasRag.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace IRasRag.Infrastructure.Data.Configurations
{
    public class ControlDeviceTypeConfiguration : IEntityTypeConfiguration<ControlDeviceType>
    {
        public void Configure(EntityTypeBuilder<ControlDeviceType> builder)
        {
            builder.ConfigureTimestamps();

        }
    }
}

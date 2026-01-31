using IRasRag.Domain.Entities;
using IRasRag.Infrastructure.Data.Seeds;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IRasRag.Infrastructure.Data.Configurations
{
    public class SensorTypeConfiguration : IEntityTypeConfiguration<SensorType>
    {
        public void Configure(EntityTypeBuilder<SensorType> builder)
        {
            builder.ConfigureTimestamps();

            builder.HasData(SensorTypeSeed.SensorTypes);
        }
    }
}

using IRasRag.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using IRasRag.Infrastructure.Data.Seeds;

namespace IRasRag.Infrastructure.Data.Configurations
{
    public class GrowthStageConfiguration : IEntityTypeConfiguration<GrowthStage>
    {
        public void Configure(EntityTypeBuilder<GrowthStage> builder)
        {
            builder.ConfigureTimestamps();

            builder.HasData(GrowthStageSeed.GrowthStages);
        }
    }
}

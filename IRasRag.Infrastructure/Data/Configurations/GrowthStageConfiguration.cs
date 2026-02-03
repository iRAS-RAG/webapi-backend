using IRasRag.Domain.Entities;
using IRasRag.Infrastructure.Data.Seeds;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IRasRag.Infrastructure.Data.Configurations
{
    public class GrowthStageConfiguration : IEntityTypeConfiguration<GrowthStage>
    {
        public void Configure(EntityTypeBuilder<GrowthStage> builder)
        {
            builder.ConfigureTimestamps();
            builder.HasIndex(gs => gs.Name).IsUnique();

            builder.HasData(GrowthStageSeed.GrowthStages);
        }
    }
}

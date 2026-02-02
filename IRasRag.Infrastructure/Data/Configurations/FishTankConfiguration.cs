using IRasRag.Domain.Entities;
using IRasRag.Infrastructure.Data.Seeds;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IRasRag.Infrastructure.Data.Configurations
{
    public class FishTankConfiguration : IEntityTypeConfiguration<FishTank>
    {
        public void Configure(EntityTypeBuilder<FishTank> builder)
        {
            builder.ConfigureTimestamps();
            builder.ConfigureSoftDelete();

            builder.HasQueryFilter(ft => !ft.IsDeleted);

            builder
                .HasOne(ft => ft.Farm)
                .WithMany(f => f.FishTanks)
                .HasForeignKey(ft => ft.FarmId)
                .OnDelete(DeleteBehavior.Restrict);

            // Most common query: list tanks by farm
            builder.HasIndex(ft => new { ft.FarmId, ft.IsDeleted });

            builder.HasData(FishTankSeed.FishTanks);
        }
    }
}

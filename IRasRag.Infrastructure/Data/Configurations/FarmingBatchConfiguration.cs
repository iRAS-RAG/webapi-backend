using IRasRag.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace IRasRag.Infrastructure.Data.Configurations
{
    public class FarmingBatchConfiguration : IEntityTypeConfiguration<FarmingBatch>
    {
        public void Configure(EntityTypeBuilder<FarmingBatch> builder)
        {
            builder.ConfigureTimestamps();

            builder.Property(fb => fb.Status)
                .HasConversion<string>()
                .HasMaxLength(20);

            builder
                .HasOne(fb => fb.FishTank)
                .WithMany(ft => ft.FarmingBatches)
                .HasForeignKey(fb => fb.FishTankId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .HasOne(fb => fb.Species)
                .WithMany(s => s.FarmingBatches)
                .HasForeignKey(fb => fb.SpeciesId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(fb => fb.FishTankId);
        }
    }
}

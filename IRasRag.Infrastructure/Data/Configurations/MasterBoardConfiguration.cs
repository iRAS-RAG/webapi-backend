using IRasRag.Domain.Entities;
using IRasRag.Infrastructure.Data.Seeds;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IRasRag.Infrastructure.Data.Configurations
{
    public class MasterBoardConfiguration : IEntityTypeConfiguration<MasterBoard>
    {
        public void Configure(EntityTypeBuilder<MasterBoard> builder)
        {
            builder.ConfigureTimestamps();
            builder.ConfigureSoftDelete();

            builder.HasQueryFilter(mb => !mb.IsDeleted);

            builder
                .HasOne(mb => mb.FishTank)
                .WithMany(ft => ft.MasterBoards)
                .HasForeignKey(mb => mb.FishTankId)
                .OnDelete(DeleteBehavior.Restrict);

            // MacAddress for IoT device registration
            builder.HasIndex(mb => mb.MacAddress).IsUnique();
            builder.HasIndex(mb => new { mb.FishTankId, mb.IsDeleted });

            builder.HasData(MasterBoardSeed.MasterBoards);
        }
    }
}

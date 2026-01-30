using IRasRag.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using IRasRag.Infrastructure.Data.Seeds;

namespace IRasRag.Infrastructure.Data.Configurations
{
    public class UserFarmConfiguration : IEntityTypeConfiguration<UserFarm>
    {
        public void Configure(EntityTypeBuilder<UserFarm> builder)
        {
            builder.ConfigureTimestamps();

            builder
                .HasOne(uf => uf.Farm)
                .WithMany(f => f.UserFarms)
                .HasForeignKey(uf => uf.FarmId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(uf => uf.User)
                .WithMany(u => u.UserFarms)
                .HasForeignKey(uf => uf.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(uf => new { uf.UserId, uf.FarmId }).IsUnique();

            builder.HasData(UserFarmSeed.UserFarms);
        }
    }
}

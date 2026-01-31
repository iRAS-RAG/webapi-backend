using IRasRag.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace IRasRag.Infrastructure.Data.Configurations
{
    public class VerificationConfiguration : IEntityTypeConfiguration<Verification>
    {
        public void Configure(EntityTypeBuilder<Verification> builder)
        {
            builder.ConfigureTimestamps();

            builder.HasOne(v => v.User)
                .WithMany()
                .HasForeignKey(v => v.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(v => v.UserId);
            // Cleanup expired codes
            builder.HasIndex(v => new { v.IsConsumed, v.ExpireDate });
        }
    }
}

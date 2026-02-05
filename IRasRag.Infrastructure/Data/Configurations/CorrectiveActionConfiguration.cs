using IRasRag.Domain.Entities;
using IRasRag.Infrastructure.Data.Seeds;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IRasRag.Infrastructure.Data.Configurations
{
    public class CorrectiveActionConfiguration : IEntityTypeConfiguration<CorrectiveAction>
    {
        public void Configure(EntityTypeBuilder<CorrectiveAction> builder)
        {
            builder.ConfigureTimestamps();

            builder
                .HasOne(ca => ca.Alert)
                .WithMany(a => a.CorrectiveActions)
                .HasForeignKey(ca => ca.AlertId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasData(CorrectiveActionSeed.CorrectiveActions);
        }
    }
}

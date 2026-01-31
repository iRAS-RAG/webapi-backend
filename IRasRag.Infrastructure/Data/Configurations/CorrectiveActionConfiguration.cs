using IRasRag.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

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

        }
    }
}

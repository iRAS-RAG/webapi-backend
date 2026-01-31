using IRasRag.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IRasRag.Infrastructure.Data.Configurations
{
    public class MortalityLogConfiguration : IEntityTypeConfiguration<MortalityLog>
    {
        public void Configure(EntityTypeBuilder<MortalityLog> builder)
        {
            builder.ConfigureTimestamps();

            builder
                .HasOne(ml => ml.Batch)
                .WithMany(fb => fb.MortalityLogs)
                .HasForeignKey(ml => ml.BatchId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}

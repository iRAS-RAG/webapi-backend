using IRasRag.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IRasRag.Infrastructure.Data.Configurations
{
    public class RecommendationConfiguration : IEntityTypeConfiguration<Recommendation>
    {
        public void Configure(EntityTypeBuilder<Recommendation> builder)
        {
            builder.ConfigureTimestamps();

            builder
                .HasOne(r => r.Alert)
                .WithMany(a => a.Recommendations)
                .HasForeignKey(r => r.AlertId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .HasOne(r => r.Document)
                .WithMany(d => d.Recommendations)
                .HasForeignKey(r => r.DocumentId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}

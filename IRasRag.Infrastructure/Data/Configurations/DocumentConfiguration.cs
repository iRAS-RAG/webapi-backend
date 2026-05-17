using IRasRag.Domain.Entities;
using IRasRag.Domain.Enums;
using IRasRag.Infrastructure.Data.Seeds;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IRasRag.Infrastructure.Data.Configurations
{
    public class DocumentConfiguration : IEntityTypeConfiguration<Document>
    {
        public void Configure(EntityTypeBuilder<Document> builder)
        {
            builder.ConfigureTimestamps();

            builder.Property(d => d.RagStatus)
                .HasConversion<string>()
                .HasMaxLength(20)
                .HasDefaultValue(DocumentRagStatus.Pending);

            builder
                .HasOne(d => d.UploadedByUser)
                .WithMany()
                .HasForeignKey(d => d.UploadedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasData(DocumentSeed.Documents);
        }
    }
}

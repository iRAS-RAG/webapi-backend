using IRasRag.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IRasRag.Infrastructure.Data.Configurations
{
    public class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
    {
        public void Configure(EntityTypeBuilder<AuditLog> builder)
        {
            builder.ConfigureTimestamps();

            builder.Property(a => a.Email).IsRequired().HasMaxLength(255);
            builder.Property(a => a.Action).IsRequired().HasMaxLength(50);
            builder.Property(a => a.EntityType).IsRequired().HasMaxLength(100);
            builder.Property(a => a.EntityId).IsRequired().HasMaxLength(100);
            builder.Property(a => a.FirstName).HasMaxLength(100);
            builder.Property(a => a.LastName).HasMaxLength(100);
            builder.Property(a => a.Timestamp).IsRequired();

            builder.HasIndex(a => a.UserId);
            builder.HasIndex(a => a.Action);
            builder.HasIndex(a => a.EntityType);
            builder.HasIndex(a => a.Timestamp);
        }
    }
}

using CareLoop.Infrastructure.Auditing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CareLoop.Infrastructure.Persistence.Configurations;

internal sealed class AuditRecordConfiguration
    : IEntityTypeConfiguration<AuditRecord>
{
    public void Configure(
        EntityTypeBuilder<AuditRecord> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        builder.Property(x => x.Action)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.EntityType)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.Source)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.CorrelationId)
            .HasMaxLength(100);

        builder.Property(x => x.OccurredAt)
            .IsRequired();

        builder.HasIndex(x => new
        {
            x.EntityType,
            x.EntityId,
            x.OccurredAt
        });
    }
}
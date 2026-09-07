using CareLoop.Domain.Escalations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CareLoop.Infrastructure.Persistence.Configurations;

internal sealed class EscalationConfiguration
    : IEntityTypeConfiguration<Escalation>
{
    public void Configure(EntityTypeBuilder<Escalation> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id).ValueGeneratedNever();

        builder.Property(x => x.Level)
            .HasConversion<int>();

        builder.Property(x => x.Reason)
            .HasMaxLength(2000)
            .IsRequired();

        builder.Property(x => x.EscalatedAt).IsRequired();

        builder.Property<Guid>("CareCaseId").IsRequired();
    }
}
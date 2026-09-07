using CareLoop.Domain.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CareLoop.Infrastructure.Persistence.Configurations;

internal sealed class DiagnosticOrderConfiguration
    : IEntityTypeConfiguration<DiagnosticOrder>
{
    public void Configure(EntityTypeBuilder<DiagnosticOrder> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        builder.Property(x => x.TestName)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.OrderedAt)
            .IsRequired();

        builder.Property(x => x.OrderedBy)
            .IsRequired();

        builder.HasIndex(x => x.OrderedAt);
    }
}
using CareLoop.Domain.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CareLoop.Infrastructure.Persistence.Configurations;

internal sealed class DiagnosticResultConfiguration
    : IEntityTypeConfiguration<DiagnosticResult>
{
    public void Configure(EntityTypeBuilder<DiagnosticResult> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        builder.Property(x => x.Priority)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(x => x.Summary)
            .HasMaxLength(2000)
            .IsRequired();

        builder.Property(x => x.ReceivedAt)
            .IsRequired();

        builder.Property<Guid>("CareCaseId")
            .IsRequired();

        builder.HasIndex(x => x.DiagnosticOrderId)
            .IsUnique();

        builder.HasOne<DiagnosticOrder>()
            .WithOne()
            .HasForeignKey<DiagnosticResult>(
                x => x.DiagnosticOrderId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
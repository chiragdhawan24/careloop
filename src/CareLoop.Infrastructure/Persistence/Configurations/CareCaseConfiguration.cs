using CareLoop.Domain.CareCases;
using CareLoop.Domain.Diagnostics;
using CareLoop.Domain.Reviews;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CareLoop.Infrastructure.Persistence.Configurations;

internal sealed class CareCaseConfiguration
    : IEntityTypeConfiguration<CareCase>
{
    public void Configure(EntityTypeBuilder<CareCase> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id).ValueGeneratedNever();

        builder.Property(x => x.Status)
            .HasConversion<int>();

        builder.Property(x => x.CreatedAt).IsRequired();

        builder.Property<Guid>("PatientId").IsRequired();
        builder.Property<Guid>("DiagnosticOrderId").IsRequired();

        builder.HasIndex(x => new { x.Status, x.CreatedAt });

        builder.HasOne(x => x.Patient)
            .WithMany()
            .HasForeignKey("PatientId")
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.DiagnosticOrder)
            .WithOne()
            .HasForeignKey<CareCase>("DiagnosticOrderId")
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.DiagnosticResult)
            .WithOne()
            .HasForeignKey<DiagnosticResult>("CareCaseId")
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.ClinicalReview)
            .WithOne()
            .HasForeignKey<ClinicalReview>("CareCaseId")
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.ContactAttempts)
            .WithOne()
            .HasForeignKey("CareCaseId")
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.FollowUpActions)
            .WithOne()
            .HasForeignKey("CareCaseId")
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.Escalations)
            .WithOne()
            .HasForeignKey("CareCaseId")
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.Events)
            .WithOne()
            .HasForeignKey("CareCaseId")
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(x => x.ContactAttempts)
            .HasField("_contactAttempts");

        builder.Navigation(x => x.FollowUpActions)
            .HasField("_followUpActions");

        builder.Navigation(x => x.Escalations)
            .HasField("_escalations");

        builder.Navigation(x => x.Events)
            .HasField("_events");
    }
}
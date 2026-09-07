using CareLoop.Domain.CareCases;
using CareLoop.Domain.Communications;
using CareLoop.Domain.Diagnostics;
using CareLoop.Domain.Escalations;
using CareLoop.Domain.FollowUps;
using CareLoop.Domain.Patients;
using CareLoop.Domain.Reviews;
using Microsoft.EntityFrameworkCore;

namespace CareLoop.Infrastructure.Persistence;

public sealed class CareLoopDbContext(
    DbContextOptions<CareLoopDbContext> options)
    : DbContext(options)
{
    public DbSet<Patient> Patients => Set<Patient>();

    public DbSet<CareCase> CareCases => Set<CareCase>();

    public DbSet<DiagnosticOrder> DiagnosticOrders =>
        Set<DiagnosticOrder>();

    public DbSet<DiagnosticResult> DiagnosticResults =>
        Set<DiagnosticResult>();

    public DbSet<ClinicalReview> ClinicalReviews =>
        Set<ClinicalReview>();

    public DbSet<ContactAttempt> ContactAttempts =>
        Set<ContactAttempt>();

    public DbSet<FollowUpAction> FollowUpActions =>
        Set<FollowUpAction>();

    public DbSet<Escalation> Escalations =>
        Set<Escalation>();

    public DbSet<CareCaseEvent> CareCaseEvents =>
        Set<CareCaseEvent>();

    protected override void OnConfiguring(
        DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSnakeCaseNamingConvention();
    }

    protected override void OnModelCreating(
        ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(CareLoopDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}
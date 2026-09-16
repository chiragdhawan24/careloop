using CareLoop.Domain.CareCases;
using CareLoop.Domain.Communications;
using CareLoop.Domain.Diagnostics;
using CareLoop.Domain.Escalations;
using CareLoop.Domain.FollowUps;
using CareLoop.Domain.Patients;
using CareLoop.Domain.Reviews;
using Microsoft.EntityFrameworkCore;

using CareLoop.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
// using Microsoft.Extensions.Options; -- no longer needed

namespace CareLoop.Infrastructure.Persistence;

public sealed class CareLoopDbContext(
    DbContextOptions<CareLoopDbContext> options)
    : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>(options)
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

    // REMOVED because CareLoop.Api gave error 134
    // protected override void OnConfiguring(
    //     DbContextOptionsBuilder optionsBuilder)
    // {
    //     optionsBuilder.UseSnakeCaseNamingConvention();
    // }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);


        // Here we are telling the EF Core that - "whenever you persist or query an `ApplicationUser` object, 
        // then make sure to map it with the `asp_net_users` table correspondingly in PostgreSQL DB (and so on for others)
        modelBuilder.Entity<ApplicationUser>()
            .ToTable("asp_net_users");

        modelBuilder.Entity<IdentityRole<Guid>>()
            .ToTable("asp_net_roles");

        modelBuilder.Entity<IdentityUserRole<Guid>>()
            .ToTable("asp_net_user_roles");

        modelBuilder.Entity<IdentityUserClaim<Guid>>()
            .ToTable("asp_net_user_claims");

        modelBuilder.Entity<IdentityRoleClaim<Guid>>()
            .ToTable("asp_net_role_claims");

        modelBuilder.Entity<IdentityUserLogin<Guid>>()
            .ToTable("asp_net_user_logins");

        modelBuilder.Entity<IdentityUserToken<Guid>>()
            .ToTable("asp_net_user_tokens");

        modelBuilder.Entity<IdentityRole<Guid>>()
            .HasIndex(x => x.NormalizedName)
            .HasDatabaseName("role_name_index")
            .IsUnique();

        modelBuilder.Entity<ApplicationUser>()
            .HasIndex(x => x.NormalizedEmail)
            .HasDatabaseName("email_index");

        modelBuilder.Entity<ApplicationUser>()
            .HasIndex(x => x.NormalizedUserName)
            .HasDatabaseName("user_name_index")
            .IsUnique();

        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(CareLoopDbContext).Assembly);
    }
    
}
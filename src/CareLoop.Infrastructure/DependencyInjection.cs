using CareLoop.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

using CareLoop.Application.Security;
using CareLoop.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace CareLoop.Infrastructure;

public static class DependencyInjection
{
    public static IHostApplicationBuilder AddInfrastructure(
        this IHostApplicationBuilder builder)
    {
        // builder.AddNpgsqlDbContext<CareLoopDbContext>(
        //     "careloopdb");
        
        builder.AddNpgsqlDbContext<CareLoopDbContext>(
            "careloopdb",
            configureDbContextOptions: options =>
                options.UseSnakeCaseNamingConvention());

        builder.Services
            .AddIdentityApiEndpoints<ApplicationUser>(options =>
            {
                options.User.RequireUniqueEmail = true;
                options.Password.RequiredLength = 12;
            })
            .AddRoles<IdentityRole<Guid>>()
            .AddEntityFrameworkStores<CareLoopDbContext>();

        builder.Services.AddAuthorizationBuilder()
            .AddPolicy(
                AuthorizationPolicies.CanReviewResults,
                policy => policy.RequireRole(
                    HealthcareRoles.Physician,
                    HealthcareRoles.Nurse))
            .AddPolicy(
                AuthorizationPolicies.CanContactPatients,
                policy => policy.RequireRole(
                    HealthcareRoles.Physician,
                    HealthcareRoles.Nurse,
                    HealthcareRoles.CareCoordinator))
            .AddPolicy(
                AuthorizationPolicies.CanManageFollowUp,
                policy => policy.RequireRole(
                    HealthcareRoles.Physician,
                    HealthcareRoles.Nurse,
                    HealthcareRoles.CareCoordinator))
            .AddPolicy(
                AuthorizationPolicies.CanEscalateCases,
                policy => policy.RequireRole(
                    HealthcareRoles.Physician,
                    HealthcareRoles.Nurse,
                    HealthcareRoles.CareCoordinator,
                    HealthcareRoles.Supervisor))
            .AddPolicy(
                AuthorizationPolicies.CanViewAuditTrail,
                policy => policy.RequireRole(
                    HealthcareRoles.Supervisor,
                    HealthcareRoles.Administrator));
                return builder;
    }
}
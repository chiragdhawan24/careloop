using CareLoop.Application.Security;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace CareLoop.Infrastructure.Identity;

public static class HealthcareIdentitySeeder
{
    public static async Task SeedAsync(IServiceProvider services)
    {
        var roleManager =
            services.GetRequiredService<RoleManager<IdentityRole<Guid>>>();

        var userManager =
            services.GetRequiredService<UserManager<ApplicationUser>>();

        string[] roles =
        [
            HealthcareRoles.Physician,
            HealthcareRoles.Nurse,
            HealthcareRoles.CareCoordinator,
            HealthcareRoles.LabStaff,
            HealthcareRoles.Supervisor,
            HealthcareRoles.Administrator
        ];

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(
                    new IdentityRole<Guid>(role));
            }
        }

        const string clinicianEmail =
            "clinician@careloop.local";

        var clinician =
            await userManager.FindByEmailAsync(clinicianEmail);

        if (clinician is not null &&
            !await userManager.IsInRoleAsync(
                clinician,
                HealthcareRoles.Physician))
        {
            await userManager.AddToRoleAsync(
                clinician,
                HealthcareRoles.Physician);
        }
    }
}
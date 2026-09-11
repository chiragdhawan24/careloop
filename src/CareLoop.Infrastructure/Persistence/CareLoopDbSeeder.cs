using CareLoop.Domain.CareCases;
using CareLoop.Domain.Diagnostics;
using CareLoop.Domain.Patients;
using Microsoft.EntityFrameworkCore;

namespace CareLoop.Infrastructure.Persistence;

public static class CareLoopDbSeeder
{
    private const string DemoMedicalRecordNumber = "MRN-DEMO-1001";

    public static async Task<bool> SeedAsync(
        CareLoopDbContext context,
        CancellationToken cancellationToken = default)
    {
        var alreadyExists = await context.Patients
            .AnyAsync(
                patient =>
                    patient.MedicalRecordNumber == DemoMedicalRecordNumber,
                cancellationToken);

        if (alreadyExists)
        {
            return false;
        }

        var patient = new Patient(
            Guid.Parse("11111111-1111-1111-1111-111111111111"),
            DemoMedicalRecordNumber,
            "Synthetic",
            "Patient",
            new DateOnly(1985, 4, 12));

        var order = new DiagnosticOrder(
            Guid.Parse("22222222-2222-2222-2222-222222222222"),
            "Complete Blood Count",
            DateTimeOffset.UtcNow,
            Guid.Parse("33333333-3333-3333-3333-333333333333"));

        var careCase = new CareCase(
            Guid.Parse("44444444-4444-4444-4444-444444444444"),
            patient,
            order,
            DateTimeOffset.UtcNow);

        var result = new DiagnosticResult(
            Guid.Parse("55555555-5555-5555-5555-555555555555"),
            order.Id,
            ResultPriority.Abnormal,
            "Synthetic abnormal laboratory result.",
            DateTimeOffset.UtcNow);

        careCase.ReceiveResult(result);

        context.CareCases.Add(careCase);

        await context.SaveChangesAsync(cancellationToken);

        return true;
    }
}
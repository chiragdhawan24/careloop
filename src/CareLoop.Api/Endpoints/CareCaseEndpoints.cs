using System.Security.Claims;
using CareLoop.Api.Contracts;
using CareLoop.Application.Security;
using CareLoop.Domain.CareCases;
using CareLoop.Domain.Diagnostics;
using CareLoop.Domain.Patients;
using CareLoop.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CareLoop.Api.Endpoints;

public static class CareCaseEndpoints
{
    public static IEndpointRouteBuilder MapCareCaseEndpoints(
        this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/care-cases");

        group.MapGet("/", GetCareCases)
            .RequireAuthorization(
                AuthorizationPolicies.CanReviewResults);

        group.MapGet("/{id:guid}", GetCareCase)
            .RequireAuthorization(
                AuthorizationPolicies.CanReviewResults);

        group.MapPost("/", CreateCareCase)
            .RequireAuthorization(
                AuthorizationPolicies.CanCreateDiagnosticOrders);

        return endpoints;
    }

    private static async Task<IResult> GetCareCases(
        CareLoopDbContext dbContext,
        CancellationToken cancellationToken)
    {
        var careCases = await dbContext.CareCases
            .AsNoTracking()
            .Select(careCase =>
                new CareCaseSummaryResponse(
                    careCase.Id,
                    careCase.Patient.MedicalRecordNumber,
                    careCase.DiagnosticOrder.TestName,
                    careCase.Status,
                    careCase.CreatedAt))
            .ToListAsync(cancellationToken);

        return Results.Ok(careCases);
    }

    private static async Task<IResult> GetCareCase(
        Guid id,
        CareLoopDbContext dbContext,
        CancellationToken cancellationToken)
    {
        var careCase = await dbContext.CareCases
            .AsNoTracking()
            .Where(x => x.Id == id)
            .Select(x =>
                new CareCaseDetailsResponse(
                    x.Id,
                    x.Patient.MedicalRecordNumber,
                    x.Patient.FirstName,
                    x.Patient.LastName,
                    x.Patient.DateOfBirth,
                    x.DiagnosticOrder.Id,
                    x.DiagnosticOrder.TestName,
                    x.DiagnosticOrder.OrderedAt,
                    x.DiagnosticResult == null
                        ? null
                        : x.DiagnosticResult.Priority,
                    x.DiagnosticResult == null
                        ? null
                        : x.DiagnosticResult.Summary,
                    x.Status,
                    x.CreatedAt))
            .SingleOrDefaultAsync(cancellationToken);

        return careCase is null
            ? Results.NotFound()
            : Results.Ok(careCase);
    }

    private static async Task<IResult> CreateCareCase(
        CreateCareCaseRequest request,
        ClaimsPrincipal user,
        CareLoopDbContext dbContext,
        CancellationToken cancellationToken)
    {
        var userId =
            user.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!Guid.TryParse(userId, out var orderedBy))
        {
            return Results.Unauthorized();
        }

        var patient = await dbContext.Patients
            .SingleOrDefaultAsync(
                x => x.MedicalRecordNumber ==
                     request.MedicalRecordNumber,
                cancellationToken);

        if (patient is null)
        {
            patient = new Patient(
                Guid.NewGuid(),
                request.MedicalRecordNumber,
                request.FirstName,
                request.LastName,
                request.DateOfBirth);
        }
        else if (
            patient.FirstName != request.FirstName ||
            patient.LastName != request.LastName ||
            patient.DateOfBirth != request.DateOfBirth)
        {
            return Results.Conflict(
                "A patient with this medical record number already exists with different demographics.");
        }

        var now = DateTimeOffset.UtcNow;

        var order = new DiagnosticOrder(
            Guid.NewGuid(),
            request.TestName,
            now,
            orderedBy);

        var careCase = new CareCase(
            Guid.NewGuid(),
            patient,
            order,
            now);

        dbContext.CareCases.Add(careCase);

        await dbContext.SaveChangesAsync(
            cancellationToken);

        return Results.Created(
            $"/api/care-cases/{careCase.Id}",
            new
            {
                careCase.Id,
                careCase.Status,
                DiagnosticOrderId = order.Id
            });
    }
}
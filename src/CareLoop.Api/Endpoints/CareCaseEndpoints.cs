using System.Security.Claims;
using CareLoop.Api.Contracts;
using CareLoop.Application.Security;
using CareLoop.Domain.CareCases;
using CareLoop.Domain.Diagnostics;
using CareLoop.Domain.Patients;
using CareLoop.Domain.Reviews;
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

        group.MapPost("/{id:guid}/result", RecordDiagnosticResult)
            .RequireAuthorization(
                AuthorizationPolicies.CanRecordDiagnosticResults);
        
        group.MapPost("/{id:guid}/review", ReviewDiagnosticResult)
            .RequireAuthorization(
                AuthorizationPolicies.CanReviewResults);

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

    private static async Task<IResult> RecordDiagnosticResult(
        Guid id,
        RecordDiagnosticResultRequest request,
        CareLoopDbContext dbContext,
        CancellationToken cancellationToken)
    {
        if (!Enum.IsDefined(request.Priority))
        {
            return Results.BadRequest(
                "Invalid diagnostic result priority.");
        }

        if (string.IsNullOrWhiteSpace(request.Summary))
        {
            return Results.BadRequest(
                "Result summary is required.");
        }

        if (request.Summary.Length > 2000)
        {
            return Results.BadRequest(
                "Result summary cannot exceed 2000 characters.");
        }

        var careCase = await dbContext.CareCases
            .Include(x => x.DiagnosticOrder)
            .SingleOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);

        if (careCase is null)
        {
            return Results.NotFound();
        }

        
        Console.WriteLine($"HELLO dbContext: {dbContext}");
        Console.WriteLine($"HELLO careCase: {careCase}");

        var result = new DiagnosticResult(
            Guid.NewGuid(),
            careCase.DiagnosticOrder.Id,
            request.Priority,
            request.Summary.Trim(),
            DateTimeOffset.UtcNow);

        try
        {
            careCase.ReceiveResult(result);
        }
        catch (InvalidOperationException exception)
        {
            return Results.Conflict(exception.Message);
        }
        
        Console.WriteLine($"Diagnostic result recorded for CareCase {careCase.Id}");

        await dbContext.SaveChangesAsync(cancellationToken);

        return Results.Ok(
            new DiagnosticResultResponse(
                result.Id,
                result.DiagnosticOrderId,
                result.Priority,
                result.Summary,
                result.ReceivedAt,
                careCase.Status));
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

    private static async Task<IResult> ReviewDiagnosticResult(
        Guid id,
        ReviewDiagnosticResultRequest request,
        ClaimsPrincipal user,
        CareLoopDbContext dbContext,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Notes))
        {
            return Results.BadRequest(
                "Review notes are required.");
        }

        if (request.Notes.Length > 4000)
        {
            return Results.BadRequest(
                "Review notes cannot exceed 4000 characters.");
        }

        var userId =
            user.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!Guid.TryParse(userId, out var reviewedBy))
        {
            return Results.Unauthorized();
        }

        var careCase = await dbContext.CareCases
            .SingleOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);

        if (careCase is null)
        {
            return Results.NotFound();
        }

        var review = new ClinicalReview(
            Guid.NewGuid(),
            reviewedBy,
            DateTimeOffset.UtcNow,
            request.Notes.Trim());

        try
        {
            careCase.ReviewResult(review);
        }
        catch (InvalidOperationException exception)
        {
            return Results.Conflict(exception.Message);
        }

        await dbContext.SaveChangesAsync(cancellationToken);

        return Results.Ok(
            new ClinicalReviewResponse(
                review.Id,
                review.ReviewedBy,
                review.ReviewedAt,
                review.Notes,
                careCase.Status));
    }
}
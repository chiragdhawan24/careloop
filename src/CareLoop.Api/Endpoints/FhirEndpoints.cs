using CareLoop.Api.Fhir;
using CareLoop.Application.Security;
using CareLoop.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CareLoop.Api.Endpoints;

public static class FhirEndpoints
{
    public static IEndpointRouteBuilder MapFhirEndpoints(
        this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/fhir");

        group.MapGet(
                "/care-cases/{id:guid}",
                GetCareCaseBundle)
            .RequireAuthorization(
                AuthorizationPolicies.CanReviewResults);

        return endpoints;
    }

    private static async Task<IResult> GetCareCaseBundle(
        Guid id,
        CareLoopDbContext dbContext,
        CancellationToken cancellationToken)
    {
        var careCase = await dbContext.CareCases
            .AsNoTracking()
            .Include(x => x.Patient)
            .Include(x => x.DiagnosticOrder)
            .Include(x => x.DiagnosticResult)
            .Include(x => x.ContactAttempts)
            .Include(x => x.FollowUpActions)
            .SingleOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);

        if (careCase is null)
        {
            return Results.NotFound();
        }

        var bundle =
            FhirCareCaseMapper.ToBundle(careCase);

        return Results.Json(
            bundle,
            contentType: "application/fhir+json");
    }
}
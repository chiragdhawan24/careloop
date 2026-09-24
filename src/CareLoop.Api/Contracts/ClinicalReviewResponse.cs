using CareLoop.Domain.CareCases;

namespace CareLoop.Api.Contracts;

public sealed record ClinicalReviewResponse(
    Guid Id,
    Guid ReviewedBy,
    DateTimeOffset ReviewedAt,
    string Notes,
    CareCaseStatus CareCaseStatus);
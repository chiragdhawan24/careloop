using CareLoop.Domain.CareCases;
using CareLoop.Domain.Communications;

namespace CareLoop.Api.Contracts;

public sealed record ContactAttemptResponse(
    Guid Id,
    ContactMethod Method,
    bool Successful,
    string Notes,
    DateTimeOffset AttemptedAt,
    CareCaseStatus CareCaseStatus);
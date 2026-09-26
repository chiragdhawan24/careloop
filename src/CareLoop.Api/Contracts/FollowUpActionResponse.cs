using CareLoop.Domain.CareCases;
using CareLoop.Domain.FollowUps;

namespace CareLoop.Api.Contracts;

public sealed record FollowUpActionResponse(
    Guid Id,
    string Description,
    Guid AssignedTo,
    DateTimeOffset DueAt,
    FollowUpStatus Status,
    DateTimeOffset? CompletedAt,
    CareCaseStatus CareCaseStatus);
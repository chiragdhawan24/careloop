namespace CareLoop.Domain.Events;

public sealed record CareCaseResolvedDomainEvent(
    Guid CareCaseId,
    DateTimeOffset OccurredAt)
    : IDomainEvent;
using CareLoop.Domain.Diagnostics;

namespace CareLoop.Domain.Events;

public sealed record DiagnosticResultReceivedDomainEvent(
    Guid CareCaseId,
    Guid DiagnosticResultId,
    ResultPriority Priority,
    DateTimeOffset OccurredAt)
    : IDomainEvent;
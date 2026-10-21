namespace CareLoop.Domain.Events;

public interface IDomainEvent
{
    DateTimeOffset OccurredAt { get; }
}
namespace CareLoop.Domain.CareCases;

public class CareCaseEvent
{
    public Guid Id { get; private set; }

    public CareCaseEventType Type { get; private set; }

    public DateTimeOffset OccurredAt { get; private set; }

    public string Description { get; private set; }

    public CareCaseEvent(
        Guid id,
        CareCaseEventType type,
        DateTimeOffset occurredAt,
        string description)
    {
        Id = id;
        Type = type;
        OccurredAt = occurredAt;
        Description = description;
    }
}
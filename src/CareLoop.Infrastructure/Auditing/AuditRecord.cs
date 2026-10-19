namespace CareLoop.Infrastructure.Auditing;

public sealed class AuditRecord
{
    private AuditRecord()
    {
    }

    public Guid Id { get; private set; }

    public Guid? ActorUserId { get; private set; }

    public string Action { get; private set; } = null!;

    public string EntityType { get; private set; } = null!;

    public Guid EntityId { get; private set; }

    public string Source { get; private set; } = null!;

    public DateTimeOffset OccurredAt { get; private set; }

    public string? CorrelationId { get; private set; }

    public static AuditRecord UserAction(
        Guid actorUserId,
        string action,
        string entityType,
        Guid entityId,
        string? correlationId = null)
    {
        return new AuditRecord
        {
            Id = Guid.NewGuid(),
            ActorUserId = actorUserId,
            Action = action,
            EntityType = entityType,
            EntityId = entityId,
            Source = "User",
            OccurredAt = DateTimeOffset.UtcNow,
            CorrelationId = correlationId
        };
    }

    public static AuditRecord SystemAction(
        string action,
        string entityType,
        Guid entityId)
    {
        return new AuditRecord
        {
            Id = Guid.NewGuid(),
            ActorUserId = null,
            Action = action,
            EntityType = entityType,
            EntityId = entityId,
            Source = "System",
            OccurredAt = DateTimeOffset.UtcNow
        };
    }
}
namespace CareLoop.Domain.Escalations;

public class Escalation
{
    public Guid Id { get; private set; }

    public EscalationLevel Level { get; private set; }

    public string Reason { get; private set; }

    public DateTimeOffset EscalatedAt { get; private set; }

    public Escalation(
        Guid id,
        EscalationLevel level,
        string reason,
        DateTimeOffset escalatedAt)
    {
        Id = id;
        Level = level;
        Reason = reason;
        EscalatedAt = escalatedAt;
    }
}
namespace CareLoop.Domain.Diagnostics;

public class DiagnosticOrder
{
    public Guid Id { get; private set; }

    public string TestName { get; private set; }

    public DateTimeOffset OrderedAt { get; private set; }

    public Guid OrderedBy { get; private set; }

    public DiagnosticOrder(
        Guid id,
        string testName,
        DateTimeOffset orderedAt,
        Guid orderedBy)
    {
        Id = id;
        TestName = testName;
        OrderedAt = orderedAt;
        OrderedBy = orderedBy;
    }
}
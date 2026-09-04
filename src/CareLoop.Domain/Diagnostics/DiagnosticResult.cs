namespace CareLoop.Domain.Diagnostics;

public class DiagnosticResult
{
    public Guid Id { get; private set; }

    public Guid DiagnosticOrderId { get; private set; }

    public ResultPriority Priority { get; private set; }

    public string Summary { get; private set; }

    public DateTimeOffset ReceivedAt { get; private set; }

    public DiagnosticResult(
        Guid id,
        Guid diagnosticOrderId,
        ResultPriority priority,
        string summary,
        DateTimeOffset receivedAt)
    {
        Id = id;
        DiagnosticOrderId = diagnosticOrderId;
        Priority = priority;
        Summary = summary;
        ReceivedAt = receivedAt;
    }
}
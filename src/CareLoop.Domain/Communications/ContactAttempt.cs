namespace CareLoop.Domain.Communications;

public class ContactAttempt
{
    public Guid Id { get; private set; }

    public ContactMethod Method { get; private set; }

    public DateTimeOffset AttemptedAt { get; private set; }

    public bool Successful { get; private set; }

    public string Notes { get; private set; }

    public ContactAttempt(
        Guid id,
        ContactMethod method,
        DateTimeOffset attemptedAt,
        bool successful,
        string notes)
    {
        Id = id;
        Method = method;
        AttemptedAt = attemptedAt;
        Successful = successful;
        Notes = notes;
    }
}
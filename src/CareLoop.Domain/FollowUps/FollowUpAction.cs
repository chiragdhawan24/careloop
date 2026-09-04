namespace CareLoop.Domain.FollowUps;

public class FollowUpAction
{
    public Guid Id { get; private set; }

    public string Description { get; private set; }

    public Guid AssignedTo { get; private set; }

    public DateTimeOffset DueAt { get; private set; }

    public FollowUpStatus Status { get; private set; }

    public DateTimeOffset? CompletedAt { get; private set; }

    public FollowUpAction(
        Guid id,
        string description,
        Guid assignedTo,
        DateTimeOffset dueAt)
    {
        Id = id;
        Description = description;
        AssignedTo = assignedTo;
        DueAt = dueAt;

        Status = FollowUpStatus.Pending;
    }

    public void Start()
    {
        if (Status != FollowUpStatus.Pending)
        {
            throw new InvalidOperationException(
                $"Cannot start follow-up when status is {Status}.");
        }

        Status = FollowUpStatus.InProgress;
    }

    public void Complete(DateTimeOffset completedAt)
    {
        if (Status != FollowUpStatus.InProgress)
        {
            throw new InvalidOperationException(
                $"Cannot complete follow-up when status is {Status}.");
        }

        Status = FollowUpStatus.Completed;
        CompletedAt = completedAt;
    }
}
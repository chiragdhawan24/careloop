namespace CareLoop.Domain.CareCases;

public enum CareCaseStatus
{
    Ordered = 1,
    AwaitingResult = 2,
    ResultReceived = 3,
    AwaitingReview = 4,
    Reviewed = 5,
    AwaitingPatientContact = 6,
    PatientContacted = 7,
    FollowUpRequired = 8,
    FollowUpInProgress = 9,
    Resolved = 10,
    Escalated = 11,
    Cancelled = 12,
    OnHold = 13
}
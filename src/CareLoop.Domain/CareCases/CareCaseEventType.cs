namespace CareLoop.Domain.CareCases;

public enum CareCaseEventType
{
    CaseCreated = 1,
    ResultReceived = 2,
    ResultReviewed = 3,
    PatientContactRequired = 4,
    PatientContactAttempted = 5,
    PatientContacted = 6,
    FollowUpCreated = 7,
    FollowUpStarted = 8,
    FollowUpCompleted = 9,
    Escalated = 10,
    Resolved = 11
}
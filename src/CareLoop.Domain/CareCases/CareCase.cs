using CareLoop.Domain.Communications;
using CareLoop.Domain.Diagnostics;
using CareLoop.Domain.Escalations;
using CareLoop.Domain.FollowUps;
using CareLoop.Domain.Patients;
using CareLoop.Domain.Reviews;

namespace CareLoop.Domain.CareCases;

public class CareCase
{
    private readonly List<ContactAttempt> _contactAttempts = new();
    private readonly List<FollowUpAction> _followUpActions = new();
    private readonly List<Escalation> _escalations = new();
    private readonly List<CareCaseEvent> _events = new();

    public Guid Id { get; private set; }

    public Patient Patient { get; private set; }

    public DiagnosticOrder DiagnosticOrder { get; private set; }

    public DiagnosticResult? DiagnosticResult { get; private set; }

    public ClinicalReview? ClinicalReview { get; private set; }

    public CareCaseStatus Status { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }

    public IReadOnlyCollection<ContactAttempt> ContactAttempts =>
        _contactAttempts.AsReadOnly();

    public IReadOnlyCollection<FollowUpAction> FollowUpActions =>
        _followUpActions.AsReadOnly();

    public IReadOnlyCollection<Escalation> Escalations =>
        _escalations.AsReadOnly();

    public IReadOnlyCollection<CareCaseEvent> Events =>
        _events.AsReadOnly();

    public CareCase(
        Guid id,
        Patient patient,
        DiagnosticOrder diagnosticOrder,
        DateTimeOffset createdAt)
    {
        Id = id;
        Patient = patient;
        DiagnosticOrder = diagnosticOrder;
        CreatedAt = createdAt;

        Status = CareCaseStatus.AwaitingResult;

        AddEvent(
            CareCaseEventType.CaseCreated,
            createdAt,
            "Care case created.");
    }

    public void ReceiveResult(DiagnosticResult result)
    {
        if (Status != CareCaseStatus.AwaitingResult)
        {
            throw new InvalidOperationException(
                $"Cannot receive a diagnostic result when case status is {Status}.");
        }

        DiagnosticResult = result;
        Status = CareCaseStatus.AwaitingReview;

        AddEvent(
            CareCaseEventType.ResultReceived,
            result.ReceivedAt,
            "Diagnostic result received.");
    }

    public void ReviewResult(ClinicalReview review)
    {
        if (Status != CareCaseStatus.AwaitingReview)
        {
            throw new InvalidOperationException(
                $"Cannot review result when case status is {Status}.");
        }

        ClinicalReview = review;
        Status = CareCaseStatus.Reviewed;

        AddEvent(
            CareCaseEventType.ResultReviewed,
            review.ReviewedAt,
            "Diagnostic result reviewed.");
    }

    public void RequirePatientContact(DateTimeOffset occurredAt)
    {
        if (Status != CareCaseStatus.Reviewed)
        {
            throw new InvalidOperationException(
                $"Cannot require patient contact when case status is {Status}.");
        }

        Status = CareCaseStatus.AwaitingPatientContact;

        AddEvent(
            CareCaseEventType.PatientContactRequired,
            occurredAt,
            "Patient contact required.");
    }

    public void RecordContactAttempt(ContactAttempt attempt)
    {
        if (Status != CareCaseStatus.AwaitingPatientContact)
        {
            throw new InvalidOperationException(
                $"Cannot record patient contact when case status is {Status}.");
        }

        _contactAttempts.Add(attempt);

        AddEvent(
            CareCaseEventType.PatientContactAttempted,
            attempt.AttemptedAt,
            "Patient contact attempted.");

        if (attempt.Successful)
        {
            Status = CareCaseStatus.PatientContacted;

            AddEvent(
                CareCaseEventType.PatientContacted,
                attempt.AttemptedAt,
                "Patient successfully contacted.");
        }
    }

    public void RequireFollowUp(
        FollowUpAction followUpAction,
        DateTimeOffset occurredAt)
    {
        if (Status != CareCaseStatus.PatientContacted)
        {
            throw new InvalidOperationException(
                $"Cannot require follow-up when case status is {Status}.");
        }

        _followUpActions.Add(followUpAction);
        Status = CareCaseStatus.FollowUpRequired;

        AddEvent(
            CareCaseEventType.FollowUpCreated,
            occurredAt,
            "Follow-up action created.");
    }

    public void StartFollowUp(
        Guid followUpActionId,
        DateTimeOffset occurredAt)
    {
        if (Status != CareCaseStatus.FollowUpRequired)
        {
            throw new InvalidOperationException(
                $"Cannot start follow-up when case status is {Status}.");
        }

        var followUp = _followUpActions
            .SingleOrDefault(x => x.Id == followUpActionId);

        if (followUp is null)
        {
            throw new InvalidOperationException(
                "Follow-up action was not found.");
        }

        followUp.Start();

        Status = CareCaseStatus.FollowUpInProgress;

        AddEvent(
            CareCaseEventType.FollowUpStarted,
            occurredAt,
            "Follow-up action started.");
    }

    public void CompleteFollowUp(
        Guid followUpActionId,
        DateTimeOffset completedAt)
    {
        if (Status != CareCaseStatus.FollowUpInProgress)
        {
            throw new InvalidOperationException(
                $"Cannot complete follow-up when case status is {Status}.");
        }

        var followUp = _followUpActions
            .SingleOrDefault(x => x.Id == followUpActionId);

        if (followUp is null)
        {
            throw new InvalidOperationException(
                "Follow-up action was not found.");
        }

        followUp.Complete(completedAt);

        AddEvent(
            CareCaseEventType.FollowUpCompleted,
            completedAt,
            "Follow-up action completed.");
    }

    public void Resolve(DateTimeOffset resolvedAt)
    {
        if (Status != CareCaseStatus.FollowUpInProgress)
        {
            throw new InvalidOperationException(
                $"Cannot resolve case when case status is {Status}.");
        }

        if (_followUpActions.Count == 0 ||
            _followUpActions.Any(x => x.Status != FollowUpStatus.Completed))
        {
            throw new InvalidOperationException(
                "Cannot resolve case until all follow-up actions are completed.");
        }

        Status = CareCaseStatus.Resolved;

        AddEvent(
            CareCaseEventType.Resolved,
            resolvedAt,
            "Care case resolved.");
    }

    public void Escalate(Escalation escalation)
    {
        if (Status == CareCaseStatus.Resolved ||
            Status == CareCaseStatus.Cancelled)
        {
            throw new InvalidOperationException(
                $"Cannot escalate case when case status is {Status}.");
        }

        _escalations.Add(escalation);

        AddEvent(
            CareCaseEventType.Escalated,
            escalation.EscalatedAt,
            $"Case escalated to {escalation.Level}.");
    }

    private void AddEvent(
        CareCaseEventType type,
        DateTimeOffset occurredAt,
        string description)
    {
        _events.Add(
            new CareCaseEvent(
                Guid.NewGuid(),
                type,
                occurredAt,
                description));
    }
}
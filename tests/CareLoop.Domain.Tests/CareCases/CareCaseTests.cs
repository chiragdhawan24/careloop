using CareLoop.Domain.CareCases;
using CareLoop.Domain.Diagnostics;
using CareLoop.Domain.Patients;

using CareLoop.Domain.Communications;
using CareLoop.Domain.Escalations;
using CareLoop.Domain.FollowUps;
using CareLoop.Domain.Reviews;

namespace CareLoop.Domain.Tests.CareCases;

public class CareCaseTests
{
    [Fact]
    public void NewCase_ShouldStartAwaitingResult()
    {
        var patient = new Patient(
            Guid.NewGuid(),
            "MRN-1001",
            "Jane",
            "Doe",
            new DateOnly(1985, 4, 12));

        var order = new DiagnosticOrder(
            Guid.NewGuid(),
            "Complete Blood Count",
            DateTimeOffset.UtcNow,
            Guid.NewGuid());

        var careCase = new CareCase(
            Guid.NewGuid(),
            patient,
            order,
            DateTimeOffset.UtcNow);

        Assert.Equal(
            CareCaseStatus.AwaitingResult,
            careCase.Status);
    }

    [Fact]
    public void ReceiveResult_ShouldMoveCaseToAwaitingReview()
    {
        var patient = new Patient(
            Guid.NewGuid(),
            "MRN-1001",
            "Jane",
            "Doe",
            new DateOnly(1985, 4, 12));

        var order = new DiagnosticOrder(
            Guid.NewGuid(),
            "Complete Blood Count",
            DateTimeOffset.UtcNow,
            Guid.NewGuid());

        var careCase = new CareCase(
            Guid.NewGuid(),
            patient,
            order,
            DateTimeOffset.UtcNow);

        var result = new DiagnosticResult(
            Guid.NewGuid(),
            order.Id,
            ResultPriority.Abnormal,
            "Synthetic abnormal laboratory result",
            DateTimeOffset.UtcNow);

        careCase.ReceiveResult(result);

        Assert.Equal(
            CareCaseStatus.AwaitingReview,
            careCase.Status);

        Assert.Equal(
            result,
            careCase.DiagnosticResult);
    }

    [Fact]
    public void ReceiveResult_WhenResultAlreadyReceived_ShouldThrowException()
    {
        var patient = new Patient(
            Guid.NewGuid(),
            "MRN-1001",
            "Jane",
            "Doe",
            new DateOnly(1985, 4, 12));

        var order = new DiagnosticOrder(
            Guid.NewGuid(),
            "Complete Blood Count",
            DateTimeOffset.UtcNow,
            Guid.NewGuid());

        var careCase = new CareCase(
            Guid.NewGuid(),
            patient,
            order,
            DateTimeOffset.UtcNow);

        var firstResult = new DiagnosticResult(
            Guid.NewGuid(),
            order.Id,
            ResultPriority.Abnormal,
            "First result",
            DateTimeOffset.UtcNow);

        careCase.ReceiveResult(firstResult);

        var secondResult = new DiagnosticResult(
            Guid.NewGuid(),
            order.Id,
            ResultPriority.Critical,
            "Second result",
            DateTimeOffset.UtcNow);

        Assert.Throws<InvalidOperationException>(
            () => careCase.ReceiveResult(secondResult));
    }

    [Fact]
    public void ReviewResult_ShouldMoveCaseToReviewed()
    {
        var patient = new Patient(
            Guid.NewGuid(),
            "MRN-1001",
            "Jane",
            "Doe",
            new DateOnly(1985, 4, 12));

        var order = new DiagnosticOrder(
            Guid.NewGuid(),
            "Complete Blood Count",
            DateTimeOffset.UtcNow,
            Guid.NewGuid());

        var careCase = new CareCase(
            Guid.NewGuid(),
            patient,
            order,
            DateTimeOffset.UtcNow);

        var result = new DiagnosticResult(
            Guid.NewGuid(),
            order.Id,
            ResultPriority.Abnormal,
            "Synthetic abnormal laboratory result",
            DateTimeOffset.UtcNow);

        careCase.ReceiveResult(result);

        var review = new ClinicalReview(
            Guid.NewGuid(),
            Guid.NewGuid(),
            DateTimeOffset.UtcNow,
            "Result reviewed.");

        careCase.ReviewResult(review);

        Assert.Equal(
            CareCaseStatus.Reviewed,
            careCase.Status);

        Assert.Equal(
            review,
            careCase.ClinicalReview);
    }

    [Fact]
    public void FailedContactAttempt_ShouldKeepCaseAwaitingPatientContact()
    {
        var careCase = CreateReviewedCase();

        careCase.RequirePatientContact(DateTimeOffset.UtcNow);

        var attempt = new ContactAttempt(
            Guid.NewGuid(),
            ContactMethod.Phone,
            DateTimeOffset.UtcNow,
            false,
            "No answer.");

        careCase.RecordContactAttempt(attempt);

        Assert.Equal(
            CareCaseStatus.AwaitingPatientContact,
            careCase.Status);

        Assert.Single(careCase.ContactAttempts);
    }

    [Fact]
    public void SuccessfulContactAttempt_ShouldMoveCaseToPatientContacted()
    {
        var careCase = CreateReviewedCase();

        careCase.RequirePatientContact(DateTimeOffset.UtcNow);

        var attempt = new ContactAttempt(
            Guid.NewGuid(),
            ContactMethod.Phone,
            DateTimeOffset.UtcNow,
            true,
            "Patient reached successfully.");

        careCase.RecordContactAttempt(attempt);

        Assert.Equal(
            CareCaseStatus.PatientContacted,
            careCase.Status);

        Assert.Single(careCase.ContactAttempts);
    }

    [Fact]
    public void CompleteWorkflow_ShouldResolveCase()
    {
        var careCase = CreateReviewedCase();

        careCase.RequirePatientContact(DateTimeOffset.UtcNow);

        var contactAttempt = new ContactAttempt(
            Guid.NewGuid(),
            ContactMethod.Phone,
            DateTimeOffset.UtcNow,
            true,
            "Patient reached.");

        careCase.RecordContactAttempt(contactAttempt);

        var followUp = new FollowUpAction(
            Guid.NewGuid(),
            "Repeat laboratory test",
            Guid.NewGuid(),
            DateTimeOffset.UtcNow.AddDays(7));

        careCase.RequireFollowUp(
            followUp,
            DateTimeOffset.UtcNow);

        Assert.Equal(
            CareCaseStatus.FollowUpRequired,
            careCase.Status);

        careCase.StartFollowUp(
            followUp.Id,
            DateTimeOffset.UtcNow);

        Assert.Equal(
            CareCaseStatus.FollowUpInProgress,
            careCase.Status);

        careCase.CompleteFollowUp(
            followUp.Id,
            DateTimeOffset.UtcNow);

        Assert.Equal(
            FollowUpStatus.Completed,
            followUp.Status);

        careCase.Resolve(DateTimeOffset.UtcNow);

        Assert.Equal(
            CareCaseStatus.Resolved,
            careCase.Status);
    }

    [Fact]
    public void Resolve_WhenFollowUpNotCompleted_ShouldThrowException()
    {
        var careCase = CreateReviewedCase();

        careCase.RequirePatientContact(DateTimeOffset.UtcNow);

        var contactAttempt = new ContactAttempt(
            Guid.NewGuid(),
            ContactMethod.Phone,
            DateTimeOffset.UtcNow,
            true,
            "Patient reached.");

        careCase.RecordContactAttempt(contactAttempt);

        var followUp = new FollowUpAction(
            Guid.NewGuid(),
            "Repeat laboratory test",
            Guid.NewGuid(),
            DateTimeOffset.UtcNow.AddDays(7));

        careCase.RequireFollowUp(
            followUp,
            DateTimeOffset.UtcNow);

        careCase.StartFollowUp(
            followUp.Id,
            DateTimeOffset.UtcNow);

        Assert.Throws<InvalidOperationException>(
            () => careCase.Resolve(DateTimeOffset.UtcNow));
    }

    [Fact]
    public void Escalate_ShouldRecordEscalation()
    {
        var careCase = CreateReviewedCase();

        careCase.RequirePatientContact(DateTimeOffset.UtcNow);

        var escalation = new Escalation(
            Guid.NewGuid(),
            EscalationLevel.Supervisor,
            "Patient contact deadline exceeded.",
            DateTimeOffset.UtcNow);

        careCase.Escalate(escalation);

        Assert.Single(careCase.Escalations);

        Assert.Equal(
            EscalationLevel.Supervisor,
            careCase.Escalations.First().Level);

        Assert.Equal(
            CareCaseStatus.AwaitingPatientContact,
            careCase.Status);
    }

    [Fact]
    public void CareCase_ShouldRecordWorkflowEvents()
    {
        var careCase = CreateReviewedCase();

        Assert.Contains(
            careCase.Events,
            x => x.Type == CareCaseEventType.CaseCreated);

        Assert.Contains(
            careCase.Events,
            x => x.Type == CareCaseEventType.ResultReceived);

        Assert.Contains(
            careCase.Events,
            x => x.Type == CareCaseEventType.ResultReviewed);
    }

    private static CareCase CreateReviewedCase()
    {
        var patient = new Patient(
            Guid.NewGuid(),
            "MRN-1001",
            "Jane",
            "Doe",
            new DateOnly(1985, 4, 12));

        var order = new DiagnosticOrder(
            Guid.NewGuid(),
            "Complete Blood Count",
            DateTimeOffset.UtcNow,
            Guid.NewGuid());

        var careCase = new CareCase(
            Guid.NewGuid(),
            patient,
            order,
            DateTimeOffset.UtcNow);

        var result = new DiagnosticResult(
            Guid.NewGuid(),
            order.Id,
            ResultPriority.Abnormal,
            "Synthetic abnormal laboratory result",
            DateTimeOffset.UtcNow);

        careCase.ReceiveResult(result);

        var review = new ClinicalReview(
            Guid.NewGuid(),
            Guid.NewGuid(),
            DateTimeOffset.UtcNow,
            "Result reviewed.");

        careCase.ReviewResult(review);

        return careCase;
    }
}
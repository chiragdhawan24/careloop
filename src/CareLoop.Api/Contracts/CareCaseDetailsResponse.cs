using CareLoop.Domain.CareCases;
using CareLoop.Domain.Diagnostics;

namespace CareLoop.Api.Contracts;

public sealed record CareCaseDetailsResponse(
    Guid Id,
    string MedicalRecordNumber,
    string FirstName,
    string LastName,
    DateOnly DateOfBirth,
    Guid DiagnosticOrderId,
    string TestName,
    DateTimeOffset OrderedAt,
    ResultPriority? ResultPriority,
    string? ResultSummary,
    CareCaseStatus Status,
    DateTimeOffset CreatedAt);
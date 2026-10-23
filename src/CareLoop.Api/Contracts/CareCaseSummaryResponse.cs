using CareLoop.Domain.CareCases;
using CareLoop.Domain.Diagnostics;

namespace CareLoop.Api.Contracts;

public sealed record CareCaseSummaryResponse(
    Guid Id,
    string MedicalRecordNumber,
    string PatientName,
    string TestName,
    ResultPriority? Priority,
    CareCaseStatus Status,
    DateTimeOffset CreatedAt);
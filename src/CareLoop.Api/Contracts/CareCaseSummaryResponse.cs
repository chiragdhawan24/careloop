using CareLoop.Domain.CareCases;

namespace CareLoop.Api.Contracts;

public sealed record CareCaseSummaryResponse(
    Guid Id,
    string MedicalRecordNumber,
    string TestName,
    CareCaseStatus Status,
    DateTimeOffset CreatedAt);
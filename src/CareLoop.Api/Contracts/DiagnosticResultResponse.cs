using CareLoop.Domain.CareCases;
using CareLoop.Domain.Diagnostics;

namespace CareLoop.Api.Contracts;

public sealed record DiagnosticResultResponse(
    Guid Id,
    Guid DiagnosticOrderId,
    ResultPriority Priority,
    string Summary,
    DateTimeOffset ReceivedAt,
    CareCaseStatus CareCaseStatus);
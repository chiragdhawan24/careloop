using CareLoop.Domain.Diagnostics;

namespace CareLoop.Api.Contracts;

public sealed record RecordDiagnosticResultRequest(
    ResultPriority Priority,
    string Summary);
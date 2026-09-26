using CareLoop.Domain.Communications;

namespace CareLoop.Api.Contracts;

public sealed record RecordContactAttemptRequest(
    ContactMethod Method,
    bool Successful,
    string Notes);
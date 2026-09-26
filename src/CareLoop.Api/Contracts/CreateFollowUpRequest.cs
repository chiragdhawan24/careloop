namespace CareLoop.Api.Contracts;

public sealed record CreateFollowUpRequest(
    string Description,
    DateTimeOffset DueAt);
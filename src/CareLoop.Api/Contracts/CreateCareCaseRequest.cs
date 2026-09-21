namespace CareLoop.Api.Contracts;

public sealed record CreateCareCaseRequest(
    string MedicalRecordNumber,
    string FirstName,
    string LastName,
    DateOnly DateOfBirth,
    string TestName);
namespace CareLoop.Domain.Patients;

public class Patient
{
    public Guid Id { get; private set; }

    public string MedicalRecordNumber { get; private set; }

    public string FirstName { get; private set; }

    public string LastName { get; private set; }

    public DateOnly DateOfBirth { get; private set; }

    public Patient(
        Guid id,
        string medicalRecordNumber,
        string firstName,
        string lastName,
        DateOnly dateOfBirth)
    {
        Id = id;
        MedicalRecordNumber = medicalRecordNumber;
        FirstName = firstName;
        LastName = lastName;
        DateOfBirth = dateOfBirth;
    }
}
namespace CareLoop.Domain.Reviews;

public class ClinicalReview
{
    public Guid Id { get; private set; }

    public Guid ReviewedBy { get; private set; }

    public DateTimeOffset ReviewedAt { get; private set; }

    public string Notes { get; private set; }

    public ClinicalReview(
        Guid id,
        Guid reviewedBy,
        DateTimeOffset reviewedAt,
        string notes)
    {
        Id = id;
        ReviewedBy = reviewedBy;
        ReviewedAt = reviewedAt;
        Notes = notes;
    }
}
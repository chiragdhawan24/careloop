namespace CareLoop.Infrastructure.Sla;

public sealed class CareLoopSlaOptions
{
    public const string SectionName = "CareLoop:Sla";

    public int CheckIntervalSeconds { get; init; } = 10;

    public int CriticalReviewSeconds { get; init; } = 30;

    public int UrgentReviewSeconds { get; init; } = 60;

    public int AbnormalReviewSeconds { get; init; } = 120;

    public int NormalReviewSeconds { get; init; } = 300;
}
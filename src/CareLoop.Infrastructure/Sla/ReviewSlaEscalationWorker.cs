using CareLoop.Domain.CareCases;
using CareLoop.Domain.Diagnostics;
using CareLoop.Domain.Escalations;
using CareLoop.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CareLoop.Infrastructure.Sla;

public sealed class ReviewSlaEscalationWorker(
    IServiceScopeFactory scopeFactory,
    IOptions<CareLoopSlaOptions> options,
    ILogger<ReviewSlaEscalationWorker> logger)
    : BackgroundService
{
    private readonly CareLoopSlaOptions _options = options.Value;

    protected override async Task ExecuteAsync(
        CancellationToken stoppingToken)
    {
        using var timer = new PeriodicTimer(
            TimeSpan.FromSeconds(
                _options.CheckIntervalSeconds));

        while (await timer.WaitForNextTickAsync(stoppingToken))
        {
            await CheckCasesAsync(stoppingToken);
        }
    }

    private async Task CheckCasesAsync(
        CancellationToken cancellationToken)
    {
        using var scope = scopeFactory.CreateScope();

        var dbContext =
            scope.ServiceProvider.GetRequiredService<CareLoopDbContext>();

        var cases = await dbContext.CareCases
            .Include(x => x.DiagnosticResult)
            .Include(x => x.Escalations)
            .Where(x =>
                x.Status == CareCaseStatus.AwaitingReview &&
                x.DiagnosticResult != null)
            .ToListAsync(cancellationToken);

        var now = DateTimeOffset.UtcNow;

        foreach (var careCase in cases)
        {
            var result = careCase.DiagnosticResult!;

            var deadline =
                result.ReceivedAt + GetReviewSla(result.Priority);

            if (now < deadline)
            {
                continue;
            }

            var alreadyEscalated =
                careCase.Escalations.Any(x =>
                    x.Level == EscalationLevel.CareCoordinator);

            if (alreadyEscalated)
            {
                continue;
            }

            careCase.Escalate(
                new Escalation(
                    Guid.NewGuid(),
                    EscalationLevel.CareCoordinator,
                    "Diagnostic result review SLA exceeded.",
                    now));

            logger.LogWarning(
                "CareCase {CareCaseId} automatically escalated because its review SLA expired.",
                careCase.Id);
        }

        await dbContext.SaveChangesAsync(
            cancellationToken);
    }

    private TimeSpan GetReviewSla(
        ResultPriority priority)
    {
        return priority switch
        {
            ResultPriority.Critical =>
                TimeSpan.FromSeconds(
                    _options.CriticalReviewSeconds),

            ResultPriority.Urgent =>
                TimeSpan.FromSeconds(
                    _options.UrgentReviewSeconds),

            ResultPriority.Abnormal =>
                TimeSpan.FromSeconds(
                    _options.AbnormalReviewSeconds),

            ResultPriority.Normal =>
                TimeSpan.FromSeconds(
                    _options.NormalReviewSeconds),

            _ => throw new ArgumentOutOfRangeException(
                nameof(priority),
                priority,
                null)
        };
    }
}
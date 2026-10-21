using CareLoop.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace CareLoop.Infrastructure.Outbox;

public sealed class OutboxProcessor(
    IServiceScopeFactory scopeFactory,
    ILogger<OutboxProcessor> logger)
    : BackgroundService
{
    protected override async Task ExecuteAsync(
        CancellationToken stoppingToken)
    {
        using var timer =
            new PeriodicTimer(
                TimeSpan.FromSeconds(5));

        while (
            await timer.WaitForNextTickAsync(
                stoppingToken))
        {
            await ProcessMessagesAsync(
                stoppingToken);
        }
    }

    private async Task ProcessMessagesAsync(
        CancellationToken cancellationToken)
    {
        using var scope =
            scopeFactory.CreateScope();

        var dbContext =
            scope.ServiceProvider
                .GetRequiredService<CareLoopDbContext>();

        var messages =
            await dbContext.OutboxMessages
                .Where(x => x.ProcessedAt == null)
                .OrderBy(x => x.OccurredAt)
                .Take(50)
                .ToListAsync(cancellationToken);

        if (messages.Count == 0)
        {
            return;
        }

        foreach (var message in messages)
        {
            logger.LogInformation(
                "Processing OutboxMessage {OutboxMessageId} of type {OutboxMessageType}",
                message.Id,
                message.Type);

            /*
             * Future:
             *
             * Kafka
             * Azure Service Bus
             * RabbitMQ
             * FHIR integration
             * notification service
             * analytics pipeline
             */

            message.MarkProcessed(
                DateTimeOffset.UtcNow);
        }

        await dbContext.SaveChangesAsync(
            cancellationToken);
    }
}
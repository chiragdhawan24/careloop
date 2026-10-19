using System.Security.Claims;
using CareLoop.Infrastructure.Persistence;

namespace CareLoop.Infrastructure.Auditing;

public static class AuditTrail
{
    public static void RecordUserAction(
        CareLoopDbContext dbContext,
        ClaimsPrincipal user,
        string action,
        string entityType,
        Guid entityId,
        string? correlationId = null)
    {
        var userId =
            user.FindFirstValue(
                ClaimTypes.NameIdentifier);

        if (!Guid.TryParse(userId, out var actorUserId))
        {
            throw new InvalidOperationException(
                "Authenticated user identifier is invalid.");
        }

        dbContext.AuditRecords.Add(
            AuditRecord.UserAction(
                actorUserId,
                action,
                entityType,
                entityId,
                correlationId));
    }

    public static void RecordSystemAction(
        CareLoopDbContext dbContext,
        string action,
        string entityType,
        Guid entityId)
    {
        dbContext.AuditRecords.Add(
            AuditRecord.SystemAction(
                action,
                entityType,
                entityId));
    }
}
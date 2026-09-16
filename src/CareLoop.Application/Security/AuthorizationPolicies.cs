namespace CareLoop.Application.Security;

public static class AuthorizationPolicies
{
    public const string CanReviewResults = "CanReviewResults";
    public const string CanContactPatients = "CanContactPatients";
    public const string CanManageFollowUp = "CanManageFollowUp";
    public const string CanEscalateCases = "CanEscalateCases";
    public const string CanViewAuditTrail = "CanViewAuditTrail";
}
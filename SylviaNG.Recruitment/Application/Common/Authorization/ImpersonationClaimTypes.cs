namespace SylviaNG.Recruitment.Application.Common.Authorization
{
    /// <summary>Custom claim names embedded in an impersonation token (see ImpersonationService,
    /// ImpersonationMiddleware). Shared constants so both stay in sync.</summary>
    public static class ImpersonationClaimTypes
    {
        public const string SessionId = "imp_sid";
        public const string ActorUserAccountId = "imp_by";
    }
}

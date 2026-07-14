namespace SylviaNG.Recruitment.Application.Interfaces.Services
{
    /// <summary>
    /// Resolves the display name of the currently authenticated HR/Admin user, for attribution
    /// on audit trails (e.g. ApplicationStatusHistory.ChangedByUserName). Not usable as a stable
    /// identifier - see remarks on GetCurrentUserName.
    /// </summary>
    public interface ICurrentUserService
    {
        /// <summary>
        /// Best-effort username for the current request: Keycloak's "preferred_username" claim,
        /// falling back to ClaimTypes.Name (the Local hardcoded-auth scheme sets this to the
        /// username itself - see AccountSettingsService.GetUsername for the same fallback).
        /// Returns null outside an authenticated request instead of throwing, since audit
        /// attribution is best-effort, not a hard requirement.
        /// </summary>
        string? GetCurrentUserName();
    }
}

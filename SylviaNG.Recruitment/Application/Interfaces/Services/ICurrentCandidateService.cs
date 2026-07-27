namespace SylviaNG.Recruitment.Application.Interfaces.Services
{
    /// <summary>
    /// Single choke point for resolving the calling candidate's identity. Every self-service
    /// candidate-profile handler goes through this instead of duplicating claim-extraction.
    /// </summary>
    public interface ICurrentCandidateService
    {
        /// <summary>
        /// Reads the current user's Keycloak subject id (ClaimTypes.NameIdentifier, falling
        /// back to the raw "sub" claim). Throws if called outside an authenticated request.
        /// </summary>
        string GetCurrentKeycloakSubjectId();

        /// <summary>
        /// Returns the CandidateProfileId for the current user, auto-provisioning a new
        /// CandidateProfile row (seeded from the token's name/email claims) if none exists yet.
        /// </summary>
        Task<long> GetOrCreateCurrentProfileIdAsync();

        /// <summary>
        /// Returns the current user's CandidateProfile.Email, auto-provisioning a profile row
        /// if none exists yet. Used as the fallback match key for JobApplication rows that
        /// predate the CandidateProfileId FK, or that a guest submitted before registering.
        /// </summary>
        Task<string> GetCurrentEmailAsync();

        /// <summary>
        /// Same as GetOrCreateCurrentProfileIdAsync, but returns null instead of throwing when
        /// there's no authenticated user on the request - safe to call from [AllowAnonymous]
        /// endpoints (e.g. the guest job-application submit flow) to opportunistically link an
        /// authenticated submitter's own profile without requiring auth.
        /// </summary>
        Task<long?> TryGetCurrentCandidateProfileIdAsync();
    }
}

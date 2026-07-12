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
        /// if none exists yet. JobApplication rows are matched to a candidate by this email
        /// (there is no CandidateProfile FK on JobApplication) - see US-040.
        /// </summary>
        Task<string> GetCurrentEmailAsync();
    }
}

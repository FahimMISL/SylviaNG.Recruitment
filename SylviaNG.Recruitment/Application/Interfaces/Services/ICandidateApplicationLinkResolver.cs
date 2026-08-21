namespace SylviaNG.Recruitment.Application.Interfaces.Services
{
    /// <summary>
    /// Resolves which CandidateProfile a job application belongs to. Shared by
    /// JobApplicationCoreService.CreateAsync and JobApplicationSubmissionService.SubmitAsync so the
    /// resolution order (authenticated candidate's own profile, else an existing profile matching
    /// the typed email, else null for a not-yet-linked guest) can't drift between the two paths.
    /// </summary>
    public interface ICandidateApplicationLinkResolver
    {
        Task<long?> ResolveCandidateProfileIdAsync(string? candidateEmail);
    }
}

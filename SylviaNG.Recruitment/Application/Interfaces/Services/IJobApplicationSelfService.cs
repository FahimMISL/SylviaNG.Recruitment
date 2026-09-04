using SylviaNG.Recruitment.Application.Features.JobPostings.Models;

namespace SylviaNG.Recruitment.Application.Interfaces.Services
{
    /// <summary>Candidate self-service over their own applications (US-040) plus the eligibility
    /// pre-check (US-024) - grouped together since all three act only on the current authenticated
    /// candidate's own identity/applications.</summary>
    public interface IJobApplicationSelfService
    {
        /// <summary>Every application the current authenticated candidate has submitted (US-040 AC1/AC2/AC3).</summary>
        Task<List<MyApplicationResponse>> GetMyApplicationsAsync();

        /// <summary>
        /// Candidate withdraws their own active application (US-040 AC4). No-op if already
        /// Withdrawn; throws if the application belongs to a different candidate or is in a
        /// terminal state that can no longer transition to Withdrawn.
        /// </summary>
        Task WithdrawMyApplicationAsync(long jobApplicationId);

        /// <summary>
        /// Checks the current authenticated candidate's profile against a job posting's own
        /// eligibility criteria (age/education/experience/district) and returns which, if any,
        /// requirements are unmet (US-024 AC2/AC3).
        /// </summary>
        Task<JobEligibilityResponse> CheckEligibilityAsync(long jobPostingId);
    }
}

using SylviaNG.Recruitment.Application.Features.JobPostings.Models;
using SylviaNG.Recruitment.SharedKernel.Pagination;

namespace SylviaNG.Recruitment.Application.Interfaces.Services
{
    /// <summary>Plain CRUD for JobApplication - the raw admin/HR create/update/delete/read paths,
    /// with no eligibility/audience/duplicate/payment logic. Real applicant submissions go through
    /// IJobApplicationSubmissionService.SubmitAsync instead.</summary>
    public interface IJobApplicationCoreService
    {
        /// <summary>
        /// candidateProfileId is an internal-only override for callers that already have the
        /// candidate's real profile in hand (e.g. TalentPoolService.FastTrackAsync) - when
        /// omitted, resolves the same way SubmitAsync does (current authenticated candidate, else
        /// an existing profile matching CandidateEmail, else null for a not-yet-linked guest).
        /// </summary>
        Task<long> CreateAsync(JobApplicationCreateRequest request, long? candidateProfileId = null);
        Task UpdateAsync(long jobApplicationId, JobApplicationUpdateRequest request);
        Task DeleteAsync(long jobApplicationId);
        Task<JobApplicationResponse> GetByIdAsync(long jobApplicationId);
        Task<PagedResult<JobApplicationResponse>> GetPaginatedByJobPostingAsync(long jobPostingId, PagedRequest request);
    }
}

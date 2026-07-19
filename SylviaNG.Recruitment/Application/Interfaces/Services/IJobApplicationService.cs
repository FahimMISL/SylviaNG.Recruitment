using SylviaNG.Recruitment.Application.Features.JobPostings.Models;
using SylviaNG.Recruitment.Domain.Enums;
using SylviaNG.Recruitment.SharedKernel.Pagination;

namespace SylviaNG.Recruitment.Application.Interfaces.Services
{
    public interface IJobApplicationService
    {
        Task<long> CreateAsync(JobApplicationCreateRequest request);
        Task UpdateAsync(long jobApplicationId, JobApplicationUpdateRequest request);
        Task DeleteAsync(long jobApplicationId);
        Task<JobApplicationResponse> GetByIdAsync(long jobApplicationId);
        Task<PagedResult<JobApplicationResponse>> GetPaginatedByJobPostingAsync(long jobPostingId, PagedRequest request);

        /// <summary>
        /// Anonymous/authenticated candidate apply flow used by the career portal (source=External)
        /// and internal job board (source=Internal). Validates the posting is Open and audience-matched,
        /// rejects duplicate (email, jobPostingId) applications, optionally stores the uploaded CV.
        /// </summary>
        Task<JobApplicationResponse> SubmitAsync(JobApplicationSubmitRequest request, ApplicationSourceEnum source);

        /// <summary>
        /// ATS dashboard: all applications across every job posting, filterable by status/source/date
        /// (US-035 AC1/AC2/AC3), plus candidate-attribute filters scoped to one vacancy (US-050).
        /// </summary>
        Task<PagedResult<JobApplicationDashboardResponse>> GetDashboardPagedAsync(
            PagedRequest request,
            JobApplicationAttributeFilterRequest filter);

        /// <summary>Full application detail including status-history audit trail (US-035 AC4).</summary>
        Task<JobApplicationDetailResponse> GetDetailAsync(long jobApplicationId);

        /// <summary>IDs of every application matching the ATS dashboard filters, unpaginated - backs
        /// "select all N matching applications" bulk selection across pages (US-047 AC5).</summary>
        Task<List<long>> GetDashboardMatchingIdsAsync(JobApplicationAttributeFilterRequest filter);

        /// <summary>Reject/withdraw reasons for the given status, for the dropdown (US-036 AC3).</summary>
        Task<List<ApplicationStatusReasonResponse>> GetStatusReasonsAsync(ApplicationStatusEnum status);

        /// <summary>Moves a single application to a new status, validating the transition and reason (US-036).</summary>
        Task UpdateStatusAsync(long jobApplicationId, JobApplicationStatusUpdateRequest request);

        /// <summary>Best-effort bulk status move across multiple applications (US-035 AC5).</summary>
        Task<JobApplicationBulkStatusUpdateResponse> BulkUpdateStatusAsync(JobApplicationBulkStatusUpdateRequest request);

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

        /// <summary>
        /// Groups applications to the given vacancy that share an email, national ID, or phone
        /// number across different applications, so HR can spot cross-channel duplicates (US-038 AC1/AC2).
        /// Already-DuplicateDismissed applications are excluded from re-surfacing as open groups.
        /// </summary>
        Task<List<JobApplicationDuplicateGroupResponse>> GetDuplicatesAsync(long jobPostingId);

        /// <summary>
        /// HR keeps one application as primary and dismisses the rest of a detected duplicate
        /// group (US-038 AC3/AC4). Re-validates the group server-side; dismissed applications
        /// move to DuplicateDismissed with an audit trail entry, retained for history.
        /// </summary>
        Task ResolveDuplicatesAsync(JobApplicationDuplicateResolveRequest request);
    }
}

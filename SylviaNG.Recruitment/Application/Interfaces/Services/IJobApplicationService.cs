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

        /// <summary>ATS dashboard: all applications across every job posting, filterable (US-035 AC1/AC2/AC3).</summary>
        Task<PagedResult<JobApplicationDashboardResponse>> GetDashboardPagedAsync(
            PagedRequest request,
            long? jobPostingId,
            ApplicationStatusEnum? status,
            ApplicationSourceEnum? source,
            DateTime? dateFrom,
            DateTime? dateTo);

        /// <summary>Full application detail including status-history audit trail (US-035 AC4).</summary>
        Task<JobApplicationDetailResponse> GetDetailAsync(long jobApplicationId);

        /// <summary>Reject/withdraw reasons for the given status, for the dropdown (US-036 AC3).</summary>
        Task<List<ApplicationStatusReasonResponse>> GetStatusReasonsAsync(ApplicationStatusEnum status);

        /// <summary>Moves a single application to a new status, validating the transition and reason (US-036).</summary>
        Task UpdateStatusAsync(long jobApplicationId, JobApplicationStatusUpdateRequest request);

        /// <summary>Best-effort bulk status move across multiple applications (US-035 AC5).</summary>
        Task<JobApplicationBulkStatusUpdateResponse> BulkUpdateStatusAsync(JobApplicationBulkStatusUpdateRequest request);
    }
}

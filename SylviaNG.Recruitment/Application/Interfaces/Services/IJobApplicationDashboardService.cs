using SylviaNG.Recruitment.Application.Features.JobPostings.Models;
using SylviaNG.Recruitment.SharedKernel.Pagination;

namespace SylviaNG.Recruitment.Application.Interfaces.Services
{
    /// <summary>ATS dashboard: the all-postings paged/filtered application list, candidate-attribute
    /// filtering, and single-application detail (grouped together since GetDetailAsync is tagged
    /// the same US-035 story as the dashboard list, not a plain CRUD read).</summary>
    public interface IJobApplicationDashboardService
    {
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
    }
}

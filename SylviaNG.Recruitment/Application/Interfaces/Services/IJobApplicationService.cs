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
    }
}

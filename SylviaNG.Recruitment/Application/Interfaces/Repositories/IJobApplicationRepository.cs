using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.SharedKernel.Generic;
using SylviaNG.Recruitment.SharedKernel.Pagination;

namespace SylviaNG.Recruitment.Application.Interfaces.Repositories
{
    public interface IJobApplicationRepository : IRepository<JobApplication>
    {
        Task<JobApplication?> GetByEmailAndJobPostingIdAsync(string email, long jobPostingId);
        Task<PagedResult<JobApplication>> GetPaginatedByJobPostingAsync(long jobPostingId, PagedRequest request);

        /// <summary>Total application count across all job postings, for dashboard summary stats.</summary>
        Task<int> CountAllAsync();

        /// <summary>Every application by this candidate's email, across all postings (HR profile view, US-009 AC4).</summary>
        Task<List<JobApplication>> GetByCandidateEmailAsync(string email);
    }
}

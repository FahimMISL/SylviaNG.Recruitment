using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.Domain.Enums;
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

        /// <summary>Cross-job-posting ATS dashboard query with optional filters (US-035 AC1/AC2).</summary>
        Task<PagedResult<JobApplication>> GetPaginatedAllAsync(
            PagedRequest request,
            long? jobPostingId,
            ApplicationStatusEnum? status,
            ApplicationSourceEnum? source,
            DateTime? dateFrom,
            DateTime? dateTo);

        /// <summary>Application detail including its full status-history + reason (US-035 AC4, US-036 AC2).</summary>
        Task<JobApplication?> GetByIdWithHistoryAsync(long jobApplicationId);

        /// <summary>Every application for one job posting, unpaginated - for shortlist filter evaluation (US-043).</summary>
        Task<List<JobApplication>> GetAllByJobPostingIdAsync(long jobPostingId);

        /// <summary>IDs of every application matching the ATS dashboard filters, unpaginated - backs
        /// "select all N matching applications" bulk selection across pages (US-047 AC5).</summary>
        Task<List<long>> GetAllMatchingIdsAsync(
            long? jobPostingId,
            ApplicationStatusEnum? status,
            ApplicationSourceEnum? source,
            DateTime? dateFrom,
            DateTime? dateTo);
    }
}

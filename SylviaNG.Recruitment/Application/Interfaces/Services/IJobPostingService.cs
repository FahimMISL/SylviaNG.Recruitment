using SylviaNG.Recruitment.Application.Features.JobPostings.Models;
using SylviaNG.Recruitment.Domain.Enums;
using SylviaNG.Recruitment.SharedKernel.Pagination;

namespace SylviaNG.Recruitment.Application.Interfaces.Services
{
    public interface IJobPostingService
    {
        Task<long> CreateAsync(JobPostingCreateRequest request);
        Task UpdateAsync(long jobPostingId, JobPostingUpdateRequest request);
        Task DeleteAsync(long jobPostingId);
        Task<JobPostingResponse> GetByIdAsync(long jobPostingId);
        Task<List<JobPostingResponse>> GetAllAsync();
        Task<PagedResult<JobPostingResponse>> GetPaginatedAsync(PagedRequest request);
        Task<List<JobPostingLookupResponse>> GetActiveBySiteIdAsync(long siteId);

        /// <summary>Public career-portal browse: Open postings with CircularType ExternalOnly or Both.</summary>
        Task<PagedResult<JobPostingResponse>> GetPaginatedPublicAsync(PagedRequest request, string? location, long? departmentId, EmploymentTypeEnum? employmentType, int? maxExperienceYears);

        /// <summary>Internal job board browse: Open postings with CircularType InternalOnly or Both.</summary>
        Task<PagedResult<JobPostingResponse>> GetPaginatedInternalAsync(PagedRequest request, string? location, long? departmentId, EmploymentTypeEnum? employmentType, int? maxExperienceYears);

        /// <summary>Public career-portal detail view. Throws NotFoundException if not Open or wrong audience.</summary>
        Task<JobPostingResponse> GetPublicByIdAsync(long jobPostingId);

        /// <summary>Internal job board detail view. Throws NotFoundException if not Open or wrong audience.</summary>
        Task<JobPostingResponse> GetInternalByIdAsync(long jobPostingId);
    }
}

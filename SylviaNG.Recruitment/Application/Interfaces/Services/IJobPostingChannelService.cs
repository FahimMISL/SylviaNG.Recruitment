using SylviaNG.Recruitment.SharedKernel.Pagination;
using SylviaNG.Recruitment.Application.Features.JobPostingChannels.Models;

namespace SylviaNG.Recruitment.Application.Interfaces.Services
{
    public interface IJobPostingChannelService
    {
        Task<long> CreateAsync(JobPostingChannelCreateRequest request);
        Task UpdateAsync(long jobPostingChannelId, JobPostingChannelUpdateRequest request);
        Task DeleteAsync(long jobPostingChannelId);
        Task<List<JobPostingChannelResponse>> GetAllAsync();
        Task<JobPostingChannelResponse> GetByIdAsync(long jobPostingChannelId);
        Task<PagedResult<JobPostingChannelResponse>> GetPaginatedAsync(PagedRequest request);
    }
}

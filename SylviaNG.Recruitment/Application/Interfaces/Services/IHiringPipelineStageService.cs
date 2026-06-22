using SylviaNG.Recruitment.SharedKernel.Pagination;
using SylviaNG.Recruitment.Application.Features.HiringPipelineStages.Models;

namespace SylviaNG.Recruitment.Application.Interfaces.Services
{
    public interface IHiringPipelineStageService
    {
        Task<long> CreateAsync(HiringPipelineStageCreateRequest request);
        Task UpdateAsync(long hiringPipelineStageId, HiringPipelineStageUpdateRequest request);
        Task DeleteAsync(long hiringPipelineStageId);
        Task<List<HiringPipelineStageResponse>> GetAllAsync();
        Task<HiringPipelineStageResponse> GetByIdAsync(long hiringPipelineStageId);
        Task<PagedResult<HiringPipelineStageResponse>> GetPaginatedAsync(PagedRequest request);
        Task<List<HiringPipelineStageResponse>> GetByJobPostingIdAsync(long jobPostingId);
    }
}

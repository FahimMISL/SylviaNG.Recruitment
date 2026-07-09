using SylviaNG.Recruitment.Application.Features.HiringPipelines.Models;

namespace SylviaNG.Recruitment.Application.Interfaces.Services
{
    public interface IHiringPipelineService
    {
        Task<long> CreateAsync(HiringPipelineCreateRequest request);
        Task UpdateAsync(long hiringPipelineId, HiringPipelineUpdateRequest request);
        Task DeleteAsync(long hiringPipelineId);
        Task<long> DuplicateAsync(long hiringPipelineId);
        Task SetActiveAsync(long hiringPipelineId, bool isActive);
        Task<HiringPipelineResponse> GetByIdAsync(long hiringPipelineId);
        Task<List<HiringPipelineResponse>> GetAllAsync();

        /// <summary>Active pipelines only, for the "assign pipeline to job posting" dropdown.</summary>
        Task<List<HiringPipelineLookupResponse>> GetActiveLookupAsync();
    }
}

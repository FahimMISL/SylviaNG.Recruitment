using SylviaNG.Recruitment.Application.Features.EventTemplateMappings.Models;

namespace SylviaNG.Recruitment.Application.Interfaces.Services
{
    public interface IEventTemplateMappingService
    {
        Task<long> CreateAsync(EventTemplateMappingCreateRequest request);
        Task UpdateAsync(long eventTemplateMappingId, EventTemplateMappingUpdateRequest request);
        Task DeleteAsync(long eventTemplateMappingId);
        Task<List<EventTemplateMappingResponse>> GetAllAsync();
    }
}

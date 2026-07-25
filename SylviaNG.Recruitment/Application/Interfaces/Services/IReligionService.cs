using SylviaNG.Recruitment.Application.Features.Religions.Models;

namespace SylviaNG.Recruitment.Application.Interfaces.Services
{
    public interface IReligionService
    {
        Task<long> CreateAsync(ReligionCreateRequest request);
        Task UpdateAsync(long religionId, ReligionUpdateRequest request);
        Task DeleteAsync(long religionId);
        Task<List<ReligionResponse>> GetAllAsync();
    }
}

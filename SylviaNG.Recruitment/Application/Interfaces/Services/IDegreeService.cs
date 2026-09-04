using SylviaNG.Recruitment.Application.Features.Degrees.Models;

namespace SylviaNG.Recruitment.Application.Interfaces.Services
{
    public interface IDegreeService
    {
        Task<long> CreateAsync(DegreeCreateRequest request);
        Task UpdateAsync(long degreeId, DegreeUpdateRequest request);
        Task DeleteAsync(long degreeId);
        Task<List<DegreeResponse>> GetAllAsync();
    }
}

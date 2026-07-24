using SylviaNG.Recruitment.Application.Features.Genders.Models;

namespace SylviaNG.Recruitment.Application.Interfaces.Services
{
    public interface IGenderService
    {
        Task<long> CreateAsync(GenderCreateRequest request);
        Task UpdateAsync(long genderId, GenderUpdateRequest request);
        Task DeleteAsync(long genderId);
        Task<List<GenderResponse>> GetAllAsync();
    }
}

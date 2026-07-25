using SylviaNG.Recruitment.Application.Features.Countries.Models;

namespace SylviaNG.Recruitment.Application.Interfaces.Services
{
    public interface ICountryService
    {
        Task<long> CreateAsync(CountryCreateRequest request);
        Task UpdateAsync(long countryId, CountryUpdateRequest request);
        Task DeleteAsync(long countryId);
        Task<List<CountryResponse>> GetAllAsync();
    }
}

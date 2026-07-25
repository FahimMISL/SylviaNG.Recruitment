using SylviaNG.Recruitment.Application.Features.AddressLookup.Models;

namespace SylviaNG.Recruitment.Application.Interfaces.Services
{
    public interface IAddressLookupService
    {
        Task<List<DivisionResponse>> GetDivisionsAsync();
        Task<List<DistrictResponse>> GetDistrictsAsync(long divisionId);
        Task<List<ThanaResponse>> GetThanasAsync(long districtId);
    }
}

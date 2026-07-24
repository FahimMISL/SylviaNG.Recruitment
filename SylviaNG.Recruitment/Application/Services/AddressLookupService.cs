using SylviaNG.Recruitment.Application.Features.AddressLookup.Models;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Services
{
    public class AddressLookupService : IAddressLookupService
    {
        private readonly IDivisionRepository _divisionRepository;
        private readonly IDistrictRepository _districtRepository;
        private readonly IThanaRepository _thanaRepository;

        public AddressLookupService(IDivisionRepository divisionRepository, IDistrictRepository districtRepository, IThanaRepository thanaRepository)
        {
            _divisionRepository = divisionRepository;
            _districtRepository = districtRepository;
            _thanaRepository = thanaRepository;
        }

        public async Task<List<DivisionResponse>> GetDivisionsAsync()
        {
            var entities = await _divisionRepository.GetAllOrderedAsync();
            return entities.Select(d => new DivisionResponse { DivisionId = d.DivisionId, Name = d.Name }).ToList();
        }

        public async Task<List<DistrictResponse>> GetDistrictsAsync(long divisionId)
        {
            var entities = await _districtRepository.GetByDivisionIdOrderedAsync(divisionId);
            return entities.Select(d => new DistrictResponse { DistrictId = d.DistrictId, Name = d.Name, DivisionId = d.DivisionId }).ToList();
        }

        public async Task<List<ThanaResponse>> GetThanasAsync(long districtId)
        {
            var entities = await _thanaRepository.GetByDistrictIdOrderedAsync(districtId);
            return entities.Select(t => new ThanaResponse { ThanaId = t.ThanaId, Name = t.Name, DistrictId = t.DistrictId }).ToList();
        }
    }
}

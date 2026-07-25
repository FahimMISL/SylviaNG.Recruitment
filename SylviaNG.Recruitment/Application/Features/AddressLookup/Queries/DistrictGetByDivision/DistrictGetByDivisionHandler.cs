using MediatR;
using SylviaNG.Recruitment.Application.Features.AddressLookup.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.AddressLookup.Queries.DistrictGetByDivision
{
    public class DistrictGetByDivisionHandler : IRequestHandler<DistrictGetByDivisionQuery, List<DistrictResponse>>
    {
        private readonly IAddressLookupService _addressLookupService;

        public DistrictGetByDivisionHandler(IAddressLookupService addressLookupService)
        {
            _addressLookupService = addressLookupService;
        }

        public async Task<List<DistrictResponse>> Handle(DistrictGetByDivisionQuery query, CancellationToken cancellationToken)
        {
            return await _addressLookupService.GetDistrictsAsync(query.DivisionId);
        }
    }
}

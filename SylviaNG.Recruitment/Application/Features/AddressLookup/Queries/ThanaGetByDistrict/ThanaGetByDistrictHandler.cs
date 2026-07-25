using MediatR;
using SylviaNG.Recruitment.Application.Features.AddressLookup.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.AddressLookup.Queries.ThanaGetByDistrict
{
    public class ThanaGetByDistrictHandler : IRequestHandler<ThanaGetByDistrictQuery, List<ThanaResponse>>
    {
        private readonly IAddressLookupService _addressLookupService;

        public ThanaGetByDistrictHandler(IAddressLookupService addressLookupService)
        {
            _addressLookupService = addressLookupService;
        }

        public async Task<List<ThanaResponse>> Handle(ThanaGetByDistrictQuery query, CancellationToken cancellationToken)
        {
            return await _addressLookupService.GetThanasAsync(query.DistrictId);
        }
    }
}

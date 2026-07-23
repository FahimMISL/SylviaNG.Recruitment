using MediatR;
using SylviaNG.Recruitment.Application.Features.AddressLookup.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.AddressLookup.Queries.DivisionGetAll
{
    public class DivisionGetAllHandler : IRequestHandler<DivisionGetAllQuery, List<DivisionResponse>>
    {
        private readonly IAddressLookupService _addressLookupService;

        public DivisionGetAllHandler(IAddressLookupService addressLookupService)
        {
            _addressLookupService = addressLookupService;
        }

        public async Task<List<DivisionResponse>> Handle(DivisionGetAllQuery query, CancellationToken cancellationToken)
        {
            return await _addressLookupService.GetDivisionsAsync();
        }
    }
}

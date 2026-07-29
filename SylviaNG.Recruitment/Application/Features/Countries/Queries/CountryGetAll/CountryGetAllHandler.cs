using MediatR;
using SylviaNG.Recruitment.Application.Features.Countries.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.Countries.Queries.CountryGetAll
{
    public class CountryGetAllHandler : IRequestHandler<CountryGetAllQuery, List<CountryResponse>>
    {
        private readonly ICountryService _countryService;

        public CountryGetAllHandler(ICountryService countryService)
        {
            _countryService = countryService;
        }

        public async Task<List<CountryResponse>> Handle(CountryGetAllQuery query, CancellationToken cancellationToken)
        {
            return await _countryService.GetAllAsync();
        }
    }
}

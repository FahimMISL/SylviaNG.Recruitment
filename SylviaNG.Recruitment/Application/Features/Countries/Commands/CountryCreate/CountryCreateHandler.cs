using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.Countries.Commands.CountryCreate
{
    public class CountryCreateHandler : IRequestHandler<CountryCreateCommand, long>
    {
        private readonly ICountryService _countryService;

        public CountryCreateHandler(ICountryService countryService)
        {
            _countryService = countryService;
        }

        public async Task<long> Handle(CountryCreateCommand command, CancellationToken cancellationToken)
        {
            return await _countryService.CreateAsync(command.Request);
        }
    }
}

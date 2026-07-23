using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.Countries.Commands.CountryUpdate
{
    public class CountryUpdateHandler : IRequestHandler<CountryUpdateCommand, Unit>
    {
        private readonly ICountryService _countryService;

        public CountryUpdateHandler(ICountryService countryService)
        {
            _countryService = countryService;
        }

        public async Task<Unit> Handle(CountryUpdateCommand command, CancellationToken cancellationToken)
        {
            await _countryService.UpdateAsync(command.CountryId, command.Request);
            return Unit.Value;
        }
    }
}

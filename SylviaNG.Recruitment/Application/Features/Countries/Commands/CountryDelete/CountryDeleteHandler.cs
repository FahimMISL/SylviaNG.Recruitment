using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.Countries.Commands.CountryDelete
{
    public class CountryDeleteHandler : IRequestHandler<CountryDeleteCommand, Unit>
    {
        private readonly ICountryService _countryService;

        public CountryDeleteHandler(ICountryService countryService)
        {
            _countryService = countryService;
        }

        public async Task<Unit> Handle(CountryDeleteCommand command, CancellationToken cancellationToken)
        {
            await _countryService.DeleteAsync(command.CountryId);
            return Unit.Value;
        }
    }
}

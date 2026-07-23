using MediatR;

namespace SylviaNG.Recruitment.Application.Features.Countries.Commands.CountryDelete
{
    public class CountryDeleteCommand : IRequest<Unit>
    {
        public long CountryId { get; set; }

        public CountryDeleteCommand(long countryId)
        {
            CountryId = countryId;
        }
    }
}

using MediatR;
using SylviaNG.Recruitment.Application.Features.Countries.Models;

namespace SylviaNG.Recruitment.Application.Features.Countries.Commands.CountryUpdate
{
    public class CountryUpdateCommand : IRequest<Unit>
    {
        public long CountryId { get; set; }
        public CountryUpdateRequest Request { get; set; }

        public CountryUpdateCommand(long countryId, CountryUpdateRequest request)
        {
            CountryId = countryId;
            Request = request;
        }
    }
}

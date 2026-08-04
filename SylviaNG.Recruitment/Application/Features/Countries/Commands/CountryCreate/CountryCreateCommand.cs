using MediatR;
using SylviaNG.Recruitment.Application.Features.Countries.Models;

namespace SylviaNG.Recruitment.Application.Features.Countries.Commands.CountryCreate
{
    public class CountryCreateCommand : IRequest<long>
    {
        public CountryCreateRequest Request { get; set; }

        public CountryCreateCommand(CountryCreateRequest request)
        {
            Request = request;
        }
    }
}

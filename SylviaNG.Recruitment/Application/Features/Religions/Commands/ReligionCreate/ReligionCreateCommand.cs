using MediatR;
using SylviaNG.Recruitment.Application.Features.Religions.Models;

namespace SylviaNG.Recruitment.Application.Features.Religions.Commands.ReligionCreate
{
    public class ReligionCreateCommand : IRequest<long>
    {
        public ReligionCreateRequest Request { get; set; }

        public ReligionCreateCommand(ReligionCreateRequest request)
        {
            Request = request;
        }
    }
}

using MediatR;
using SylviaNG.Recruitment.Application.Features.Genders.Models;

namespace SylviaNG.Recruitment.Application.Features.Genders.Commands.GenderCreate
{
    public class GenderCreateCommand : IRequest<long>
    {
        public GenderCreateRequest Request { get; set; }

        public GenderCreateCommand(GenderCreateRequest request)
        {
            Request = request;
        }
    }
}

using MediatR;
using SylviaNG.Recruitment.Application.Features.Nominees.Models;

namespace SylviaNG.Recruitment.Application.Features.Nominees.Commands.NomineeCreate
{
    public class NomineeCreateCommand : IRequest<long>
    {
        public NomineeCreateRequest Request { get; set; }

        public NomineeCreateCommand(NomineeCreateRequest request)
        {
            Request = request;
        }
    }
}

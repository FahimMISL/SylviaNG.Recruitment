using MediatR;
using SylviaNG.Recruitment.Application.Features.VerificationItems.Models;

namespace SylviaNG.Recruitment.Application.Features.VerificationItems.Commands.VerificationItemCreate
{
    public class VerificationItemCreateCommand : IRequest<long>
    {
        public VerificationItemCreateRequest Request { get; set; }

        public VerificationItemCreateCommand(VerificationItemCreateRequest request)
        {
            Request = request;
        }
    }
}

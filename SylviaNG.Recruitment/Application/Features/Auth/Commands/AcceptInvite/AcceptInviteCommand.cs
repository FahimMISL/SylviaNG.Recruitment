using MediatR;
using SylviaNG.Recruitment.Application.Features.Auth.Models;

namespace SylviaNG.Recruitment.Application.Features.Auth.Commands.AcceptInvite
{
    public class AcceptInviteCommand : IRequest
    {
        public AcceptInviteRequest Request { get; set; }

        public AcceptInviteCommand(AcceptInviteRequest request)
        {
            Request = request;
        }
    }
}

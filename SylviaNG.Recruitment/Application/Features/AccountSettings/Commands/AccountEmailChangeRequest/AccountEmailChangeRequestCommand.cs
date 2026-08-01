using MediatR;
using SylviaNG.Recruitment.Application.Features.AccountSettings.Models;

namespace SylviaNG.Recruitment.Application.Features.AccountSettings.Commands.AccountEmailChangeRequest
{
    public class AccountEmailChangeRequestCommand : IRequest<AccountEmailChangeChallengeResponse>
    {
        public Models.AccountEmailChangeRequest Request { get; set; }

        public AccountEmailChangeRequestCommand(Models.AccountEmailChangeRequest request)
        {
            Request = request;
        }
    }
}

using MediatR;
using SylviaNG.Recruitment.Application.Features.AccountSettings.Models;

namespace SylviaNG.Recruitment.Application.Features.AccountSettings.Commands.AccountPasswordChange
{
    public class AccountPasswordChangeCommand : IRequest<Unit>
    {
        public AccountPasswordChangeRequest Request { get; set; }

        public AccountPasswordChangeCommand(AccountPasswordChangeRequest request)
        {
            Request = request;
        }
    }
}

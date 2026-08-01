using MediatR;
using SylviaNG.Recruitment.Application.Features.AccountSettings.Models;

namespace SylviaNG.Recruitment.Application.Features.AccountSettings.Commands.AccountEmailChangeConfirm
{
    public class AccountEmailChangeConfirmCommand : IRequest<string>
    {
        public AccountEmailChangeConfirmRequest Request { get; set; }

        public AccountEmailChangeConfirmCommand(AccountEmailChangeConfirmRequest request)
        {
            Request = request;
        }
    }
}

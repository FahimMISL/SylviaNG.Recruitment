using MediatR;
using SylviaNG.Recruitment.Application.Features.AccountSettings.Models;

namespace SylviaNG.Recruitment.Application.Features.AccountSettings.Commands.AccountEmailUpdate
{
    public class AccountEmailUpdateCommand : IRequest<Unit>
    {
        public AccountEmailUpdateRequest Request { get; set; }

        public AccountEmailUpdateCommand(AccountEmailUpdateRequest request)
        {
            Request = request;
        }
    }
}

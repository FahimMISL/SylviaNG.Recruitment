using MediatR;
using SylviaNG.Recruitment.Application.Features.UserAccounts.Models;

namespace SylviaNG.Recruitment.Application.Features.UserAccounts.Commands.UserAccountCreate
{
    public class UserAccountCreateCommand : IRequest<long>
    {
        public UserAccountCreateRequest Request { get; set; }

        public UserAccountCreateCommand(UserAccountCreateRequest request)
        {
            Request = request;
        }
    }
}

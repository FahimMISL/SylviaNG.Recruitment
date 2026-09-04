using MediatR;
using SylviaNG.Recruitment.Application.Features.UserAccounts.Models;

namespace SylviaNG.Recruitment.Application.Features.UserAccounts.Commands.UserAccountUpdate
{
    public class UserAccountUpdateCommand : IRequest<Unit>
    {
        public long UserAccountId { get; set; }
        public UserAccountUpdateRequest Request { get; set; }

        public UserAccountUpdateCommand(long userAccountId, UserAccountUpdateRequest request)
        {
            UserAccountId = userAccountId;
            Request = request;
        }
    }
}

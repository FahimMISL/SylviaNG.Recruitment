using MediatR;
using SylviaNG.Recruitment.Application.Features.UserAccounts.Models;

namespace SylviaNG.Recruitment.Application.Features.UserAccounts.Queries.UserAccountGetById
{
    public class UserAccountGetByIdQuery : IRequest<UserAccountResponse>
    {
        public long UserAccountId { get; set; }

        public UserAccountGetByIdQuery(long userAccountId)
        {
            UserAccountId = userAccountId;
        }
    }
}

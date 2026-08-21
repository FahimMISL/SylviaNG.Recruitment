using MediatR;
using SylviaNG.Recruitment.Application.Features.UserAccounts.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.UserAccounts.Queries.UserAccountGetById
{
    public class UserAccountGetByIdHandler : IRequestHandler<UserAccountGetByIdQuery, UserAccountResponse>
    {
        private readonly IUserAccountService _userAccountService;

        public UserAccountGetByIdHandler(IUserAccountService userAccountService)
        {
            _userAccountService = userAccountService;
        }

        public async Task<UserAccountResponse> Handle(UserAccountGetByIdQuery query, CancellationToken cancellationToken)
        {
            return await _userAccountService.GetByIdAsync(query.UserAccountId);
        }
    }
}

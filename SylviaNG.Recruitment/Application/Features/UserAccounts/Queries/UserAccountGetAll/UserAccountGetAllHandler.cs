using MediatR;
using SylviaNG.Recruitment.Application.Features.UserAccounts.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.UserAccounts.Queries.UserAccountGetAll
{
    public class UserAccountGetAllHandler : IRequestHandler<UserAccountGetAllQuery, List<UserAccountResponse>>
    {
        private readonly IUserAccountService _userAccountService;

        public UserAccountGetAllHandler(IUserAccountService userAccountService)
        {
            _userAccountService = userAccountService;
        }

        public async Task<List<UserAccountResponse>> Handle(UserAccountGetAllQuery query, CancellationToken cancellationToken)
        {
            return await _userAccountService.GetAllAsync();
        }
    }
}

using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.UserAccounts.Commands.UserAccountCreate
{
    public class UserAccountCreateHandler : IRequestHandler<UserAccountCreateCommand, long>
    {
        private readonly IUserAccountService _userAccountService;

        public UserAccountCreateHandler(IUserAccountService userAccountService)
        {
            _userAccountService = userAccountService;
        }

        public async Task<long> Handle(UserAccountCreateCommand command, CancellationToken cancellationToken)
        {
            return await _userAccountService.CreateAsync(command.Request);
        }
    }
}

using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.UserAccounts.Commands.UserAccountSetActive
{
    public class UserAccountSetActiveHandler : IRequestHandler<UserAccountSetActiveCommand, Unit>
    {
        private readonly IUserAccountService _userAccountService;

        public UserAccountSetActiveHandler(IUserAccountService userAccountService)
        {
            _userAccountService = userAccountService;
        }

        public async Task<Unit> Handle(UserAccountSetActiveCommand command, CancellationToken cancellationToken)
        {
            await _userAccountService.SetActiveAsync(command.UserAccountId, command.IsActive);
            return Unit.Value;
        }
    }
}

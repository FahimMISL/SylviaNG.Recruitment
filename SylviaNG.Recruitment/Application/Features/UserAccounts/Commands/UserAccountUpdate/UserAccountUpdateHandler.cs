using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.UserAccounts.Commands.UserAccountUpdate
{
    public class UserAccountUpdateHandler : IRequestHandler<UserAccountUpdateCommand, Unit>
    {
        private readonly IUserAccountService _userAccountService;

        public UserAccountUpdateHandler(IUserAccountService userAccountService)
        {
            _userAccountService = userAccountService;
        }

        public async Task<Unit> Handle(UserAccountUpdateCommand command, CancellationToken cancellationToken)
        {
            await _userAccountService.UpdateAsync(command.UserAccountId, command.Request);
            return Unit.Value;
        }
    }
}

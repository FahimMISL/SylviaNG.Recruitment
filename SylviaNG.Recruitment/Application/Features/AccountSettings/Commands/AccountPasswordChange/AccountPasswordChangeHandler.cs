using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.AccountSettings.Commands.AccountPasswordChange
{
    public class AccountPasswordChangeHandler : IRequestHandler<AccountPasswordChangeCommand, Unit>
    {
        private readonly IAccountSettingsService _accountSettingsService;

        public AccountPasswordChangeHandler(IAccountSettingsService accountSettingsService)
        {
            _accountSettingsService = accountSettingsService;
        }

        public async Task<Unit> Handle(AccountPasswordChangeCommand command, CancellationToken cancellationToken)
        {
            await _accountSettingsService.ChangePasswordAsync(command.Request);
            return Unit.Value;
        }
    }
}

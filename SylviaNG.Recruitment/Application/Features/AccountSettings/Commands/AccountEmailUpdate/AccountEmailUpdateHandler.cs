using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.AccountSettings.Commands.AccountEmailUpdate
{
    public class AccountEmailUpdateHandler : IRequestHandler<AccountEmailUpdateCommand, Unit>
    {
        private readonly IAccountSettingsService _accountSettingsService;

        public AccountEmailUpdateHandler(IAccountSettingsService accountSettingsService)
        {
            _accountSettingsService = accountSettingsService;
        }

        public async Task<Unit> Handle(AccountEmailUpdateCommand command, CancellationToken cancellationToken)
        {
            await _accountSettingsService.UpdateEmailAsync(command.Request);
            return Unit.Value;
        }
    }
}

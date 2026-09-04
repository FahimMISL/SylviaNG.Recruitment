using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.AccountSettings.Commands.AccountEmailChangeConfirm
{
    public class AccountEmailChangeConfirmHandler : IRequestHandler<AccountEmailChangeConfirmCommand, string>
    {
        private readonly IAccountSettingsService _accountSettingsService;

        public AccountEmailChangeConfirmHandler(IAccountSettingsService accountSettingsService)
        {
            _accountSettingsService = accountSettingsService;
        }

        public async Task<string> Handle(AccountEmailChangeConfirmCommand command, CancellationToken cancellationToken)
        {
            return await _accountSettingsService.ConfirmEmailChangeAsync(command.Request);
        }
    }
}

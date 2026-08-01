using MediatR;
using SylviaNG.Recruitment.Application.Features.AccountSettings.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.AccountSettings.Commands.AccountEmailChangeRequest
{
    public class AccountEmailChangeRequestHandler : IRequestHandler<AccountEmailChangeRequestCommand, AccountEmailChangeChallengeResponse>
    {
        private readonly IAccountSettingsService _accountSettingsService;

        public AccountEmailChangeRequestHandler(IAccountSettingsService accountSettingsService)
        {
            _accountSettingsService = accountSettingsService;
        }

        public async Task<AccountEmailChangeChallengeResponse> Handle(AccountEmailChangeRequestCommand command, CancellationToken cancellationToken)
        {
            return await _accountSettingsService.RequestEmailChangeAsync(command.Request);
        }
    }
}

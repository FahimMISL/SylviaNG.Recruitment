using MediatR;
using SylviaNG.Recruitment.Application.Features.AccountSettings.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.AccountSettings.Queries.AccountSettingsGetMe
{
    public class AccountSettingsGetMeHandler : IRequestHandler<AccountSettingsGetMeQuery, AccountSettingsResponse>
    {
        private readonly IAccountSettingsService _accountSettingsService;

        public AccountSettingsGetMeHandler(IAccountSettingsService accountSettingsService)
        {
            _accountSettingsService = accountSettingsService;
        }

        public async Task<AccountSettingsResponse> Handle(AccountSettingsGetMeQuery query, CancellationToken cancellationToken)
        {
            return await _accountSettingsService.GetMyAccountAsync();
        }
    }
}

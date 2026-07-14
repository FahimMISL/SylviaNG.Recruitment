using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.AccountSettings.Commands.AccountPhotoDelete
{
    public class AccountPhotoDeleteHandler : IRequestHandler<AccountPhotoDeleteCommand, Unit>
    {
        private readonly IAccountSettingsService _accountSettingsService;

        public AccountPhotoDeleteHandler(IAccountSettingsService accountSettingsService)
        {
            _accountSettingsService = accountSettingsService;
        }

        public async Task<Unit> Handle(AccountPhotoDeleteCommand command, CancellationToken cancellationToken)
        {
            await _accountSettingsService.DeletePhotoAsync();
            return Unit.Value;
        }
    }
}

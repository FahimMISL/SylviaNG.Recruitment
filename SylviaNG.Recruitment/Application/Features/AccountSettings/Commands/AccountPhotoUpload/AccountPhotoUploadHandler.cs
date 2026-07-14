using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.AccountSettings.Commands.AccountPhotoUpload
{
    public class AccountPhotoUploadHandler : IRequestHandler<AccountPhotoUploadCommand, string>
    {
        private readonly IAccountSettingsService _accountSettingsService;

        public AccountPhotoUploadHandler(IAccountSettingsService accountSettingsService)
        {
            _accountSettingsService = accountSettingsService;
        }

        public async Task<string> Handle(AccountPhotoUploadCommand command, CancellationToken cancellationToken)
        {
            return await _accountSettingsService.UploadPhotoAsync(command.File);
        }
    }
}

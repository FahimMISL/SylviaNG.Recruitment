using MediatR;
using Microsoft.AspNetCore.Http;

namespace SylviaNG.Recruitment.Application.Features.AccountSettings.Commands.AccountPhotoUpload
{
    public class AccountPhotoUploadCommand : IRequest<string>
    {
        public IFormFile File { get; set; }

        public AccountPhotoUploadCommand(IFormFile file)
        {
            File = file;
        }
    }
}

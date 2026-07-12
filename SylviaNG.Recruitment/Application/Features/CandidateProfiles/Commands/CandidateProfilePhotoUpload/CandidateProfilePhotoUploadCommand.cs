using MediatR;
using Microsoft.AspNetCore.Http;

namespace SylviaNG.Recruitment.Application.Features.CandidateProfiles.Commands.CandidateProfilePhotoUpload
{
    public class CandidateProfilePhotoUploadCommand : IRequest<string>
    {
        public IFormFile File { get; set; }

        public CandidateProfilePhotoUploadCommand(IFormFile file)
        {
            File = file;
        }
    }
}

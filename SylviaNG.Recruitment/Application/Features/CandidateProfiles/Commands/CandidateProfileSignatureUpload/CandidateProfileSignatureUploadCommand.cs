using MediatR;
using Microsoft.AspNetCore.Http;

namespace SylviaNG.Recruitment.Application.Features.CandidateProfiles.Commands.CandidateProfileSignatureUpload
{
    public class CandidateProfileSignatureUploadCommand : IRequest<string>
    {
        public IFormFile File { get; set; }

        public CandidateProfileSignatureUploadCommand(IFormFile file)
        {
            File = file;
        }
    }
}

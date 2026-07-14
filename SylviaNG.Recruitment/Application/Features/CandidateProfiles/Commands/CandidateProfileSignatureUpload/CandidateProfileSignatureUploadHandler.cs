using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.CandidateProfiles.Commands.CandidateProfileSignatureUpload
{
    public class CandidateProfileSignatureUploadHandler : IRequestHandler<CandidateProfileSignatureUploadCommand, string>
    {
        private readonly ICandidateProfileService _candidateProfileService;

        public CandidateProfileSignatureUploadHandler(ICandidateProfileService candidateProfileService)
        {
            _candidateProfileService = candidateProfileService;
        }

        public async Task<string> Handle(CandidateProfileSignatureUploadCommand command, CancellationToken cancellationToken)
        {
            return await _candidateProfileService.UploadSignatureAsync(command.File);
        }
    }
}

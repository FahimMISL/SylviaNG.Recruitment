using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.CandidateProfiles.Commands.CandidateProfilePhotoUpload
{
    public class CandidateProfilePhotoUploadHandler : IRequestHandler<CandidateProfilePhotoUploadCommand, string>
    {
        private readonly ICandidateProfileService _candidateProfileService;

        public CandidateProfilePhotoUploadHandler(ICandidateProfileService candidateProfileService)
        {
            _candidateProfileService = candidateProfileService;
        }

        public async Task<string> Handle(CandidateProfilePhotoUploadCommand command, CancellationToken cancellationToken)
        {
            return await _candidateProfileService.UploadPhotoAsync(command.File);
        }
    }
}

using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.CandidateProfiles.Commands.CandidateProfilePhotoDelete
{
    public class CandidateProfilePhotoDeleteHandler : IRequestHandler<CandidateProfilePhotoDeleteCommand, Unit>
    {
        private readonly ICandidateProfileService _candidateProfileService;

        public CandidateProfilePhotoDeleteHandler(ICandidateProfileService candidateProfileService)
        {
            _candidateProfileService = candidateProfileService;
        }

        public async Task<Unit> Handle(CandidateProfilePhotoDeleteCommand command, CancellationToken cancellationToken)
        {
            await _candidateProfileService.DeletePhotoAsync();
            return Unit.Value;
        }
    }
}

using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.CandidateProfiles.Commands.CandidateProfileSignatureDelete
{
    public class CandidateProfileSignatureDeleteHandler : IRequestHandler<CandidateProfileSignatureDeleteCommand, Unit>
    {
        private readonly ICandidateProfileService _candidateProfileService;

        public CandidateProfileSignatureDeleteHandler(ICandidateProfileService candidateProfileService)
        {
            _candidateProfileService = candidateProfileService;
        }

        public async Task<Unit> Handle(CandidateProfileSignatureDeleteCommand command, CancellationToken cancellationToken)
        {
            await _candidateProfileService.DeleteSignatureAsync();
            return Unit.Value;
        }
    }
}

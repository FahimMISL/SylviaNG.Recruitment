using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.CandidateProfiles.Commands.CandidateProfileMarkInternal
{
    public class CandidateProfileMarkInternalHandler : IRequestHandler<CandidateProfileMarkInternalCommand, Unit>
    {
        private readonly ICandidateProfileService _candidateProfileService;

        public CandidateProfileMarkInternalHandler(ICandidateProfileService candidateProfileService)
        {
            _candidateProfileService = candidateProfileService;
        }

        public async Task<Unit> Handle(CandidateProfileMarkInternalCommand command, CancellationToken cancellationToken)
        {
            await _candidateProfileService.MarkInternalAsync(command.CandidateProfileId);
            return Unit.Value;
        }
    }
}

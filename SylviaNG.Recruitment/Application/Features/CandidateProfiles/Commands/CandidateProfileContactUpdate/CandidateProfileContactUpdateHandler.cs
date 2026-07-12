using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.CandidateProfiles.Commands.CandidateProfileContactUpdate
{
    public class CandidateProfileContactUpdateHandler : IRequestHandler<CandidateProfileContactUpdateCommand, Unit>
    {
        private readonly ICandidateProfileService _candidateProfileService;

        public CandidateProfileContactUpdateHandler(ICandidateProfileService candidateProfileService)
        {
            _candidateProfileService = candidateProfileService;
        }

        public async Task<Unit> Handle(CandidateProfileContactUpdateCommand command, CancellationToken cancellationToken)
        {
            await _candidateProfileService.UpdateContactAsync(command.Request);
            return Unit.Value;
        }
    }
}

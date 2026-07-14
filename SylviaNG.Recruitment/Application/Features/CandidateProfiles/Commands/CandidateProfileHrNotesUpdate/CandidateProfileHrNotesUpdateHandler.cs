using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.CandidateProfiles.Commands.CandidateProfileHrNotesUpdate
{
    public class CandidateProfileHrNotesUpdateHandler : IRequestHandler<CandidateProfileHrNotesUpdateCommand, Unit>
    {
        private readonly ICandidateProfileService _candidateProfileService;

        public CandidateProfileHrNotesUpdateHandler(ICandidateProfileService candidateProfileService)
        {
            _candidateProfileService = candidateProfileService;
        }

        public async Task<Unit> Handle(CandidateProfileHrNotesUpdateCommand command, CancellationToken cancellationToken)
        {
            await _candidateProfileService.UpdateHrNotesAsync(command.CandidateProfileId, command.HrNotes);
            return Unit.Value;
        }
    }
}

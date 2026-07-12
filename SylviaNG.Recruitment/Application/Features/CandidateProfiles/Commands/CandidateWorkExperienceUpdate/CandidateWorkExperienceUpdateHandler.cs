using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.CandidateProfiles.Commands.CandidateWorkExperienceUpdate
{
    public class CandidateWorkExperienceUpdateHandler : IRequestHandler<CandidateWorkExperienceUpdateCommand, Unit>
    {
        private readonly ICandidateWorkExperienceService _candidateWorkExperienceService;

        public CandidateWorkExperienceUpdateHandler(ICandidateWorkExperienceService candidateWorkExperienceService)
        {
            _candidateWorkExperienceService = candidateWorkExperienceService;
        }

        public async Task<Unit> Handle(CandidateWorkExperienceUpdateCommand command, CancellationToken cancellationToken)
        {
            await _candidateWorkExperienceService.UpdateAsync(command.CandidateWorkExperienceId, command.Request);
            return Unit.Value;
        }
    }
}

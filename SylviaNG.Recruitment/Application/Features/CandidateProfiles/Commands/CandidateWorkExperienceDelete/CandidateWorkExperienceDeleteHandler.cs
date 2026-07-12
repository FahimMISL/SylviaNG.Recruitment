using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.CandidateProfiles.Commands.CandidateWorkExperienceDelete
{
    public class CandidateWorkExperienceDeleteHandler : IRequestHandler<CandidateWorkExperienceDeleteCommand, Unit>
    {
        private readonly ICandidateWorkExperienceService _candidateWorkExperienceService;

        public CandidateWorkExperienceDeleteHandler(ICandidateWorkExperienceService candidateWorkExperienceService)
        {
            _candidateWorkExperienceService = candidateWorkExperienceService;
        }

        public async Task<Unit> Handle(CandidateWorkExperienceDeleteCommand command, CancellationToken cancellationToken)
        {
            await _candidateWorkExperienceService.DeleteAsync(command.CandidateWorkExperienceId);
            return Unit.Value;
        }
    }
}

using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.CandidateExperiences.Commands.CandidateExperienceUpdate
{
    public class CandidateExperienceUpdateHandler : IRequestHandler<CandidateExperienceUpdateCommand, Unit>
    {
        private readonly ICandidateExperienceService _service;

        public CandidateExperienceUpdateHandler(ICandidateExperienceService service)
        {
            _service = service;
        }

        public async Task<Unit> Handle(CandidateExperienceUpdateCommand command, CancellationToken cancellationToken)
        {
            await _service.UpdateAsync(command.CandidateExperienceId, command.Request);
            return Unit.Value;
        }
    }
}

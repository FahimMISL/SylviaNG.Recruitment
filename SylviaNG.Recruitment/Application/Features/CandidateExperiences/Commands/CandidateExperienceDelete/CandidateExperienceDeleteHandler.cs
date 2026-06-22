using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.CandidateExperiences.Commands.CandidateExperienceDelete
{
    public class CandidateExperienceDeleteHandler : IRequestHandler<CandidateExperienceDeleteCommand, Unit>
    {
        private readonly ICandidateExperienceService _service;

        public CandidateExperienceDeleteHandler(ICandidateExperienceService service)
        {
            _service = service;
        }

        public async Task<Unit> Handle(CandidateExperienceDeleteCommand command, CancellationToken cancellationToken)
        {
            await _service.DeleteAsync(command.CandidateExperienceId);
            return Unit.Value;
        }
    }
}

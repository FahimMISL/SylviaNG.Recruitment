using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.CandidateSkills.Commands.CandidateSkillUpdate
{
    public class CandidateSkillUpdateHandler : IRequestHandler<CandidateSkillUpdateCommand, Unit>
    {
        private readonly ICandidateSkillService _service;

        public CandidateSkillUpdateHandler(ICandidateSkillService service)
        {
            _service = service;
        }

        public async Task<Unit> Handle(CandidateSkillUpdateCommand command, CancellationToken cancellationToken)
        {
            await _service.UpdateAsync(command.CandidateSkillId, command.Request);
            return Unit.Value;
        }
    }
}

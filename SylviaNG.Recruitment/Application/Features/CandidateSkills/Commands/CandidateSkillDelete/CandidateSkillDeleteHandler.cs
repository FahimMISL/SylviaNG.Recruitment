using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.CandidateSkills.Commands.CandidateSkillDelete
{
    public class CandidateSkillDeleteHandler : IRequestHandler<CandidateSkillDeleteCommand, Unit>
    {
        private readonly ICandidateSkillService _service;

        public CandidateSkillDeleteHandler(ICandidateSkillService service)
        {
            _service = service;
        }

        public async Task<Unit> Handle(CandidateSkillDeleteCommand command, CancellationToken cancellationToken)
        {
            await _service.DeleteAsync(command.CandidateSkillId);
            return Unit.Value;
        }
    }
}

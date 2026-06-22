using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.CandidateSkills.Commands.CandidateSkillCreate
{
    public class CandidateSkillCreateHandler : IRequestHandler<CandidateSkillCreateCommand, long>
    {
        private readonly ICandidateSkillService _service;

        public CandidateSkillCreateHandler(ICandidateSkillService service)
        {
            _service = service;
        }

        public async Task<long> Handle(CandidateSkillCreateCommand command, CancellationToken cancellationToken)
        {
            return await _service.CreateAsync(command.Request);
        }
    }
}

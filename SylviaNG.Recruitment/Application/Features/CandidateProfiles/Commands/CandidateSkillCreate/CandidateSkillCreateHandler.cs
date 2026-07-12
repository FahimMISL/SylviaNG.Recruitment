using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.CandidateProfiles.Commands.CandidateSkillCreate
{
    public class CandidateSkillCreateHandler : IRequestHandler<CandidateSkillCreateCommand, long>
    {
        private readonly ICandidateSkillService _candidateSkillService;

        public CandidateSkillCreateHandler(ICandidateSkillService candidateSkillService)
        {
            _candidateSkillService = candidateSkillService;
        }

        public async Task<long> Handle(CandidateSkillCreateCommand command, CancellationToken cancellationToken)
        {
            return await _candidateSkillService.CreateAsync(command.Request);
        }
    }
}

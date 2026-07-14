using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.CandidateProfiles.Commands.CandidateSkillDelete
{
    public class CandidateSkillDeleteHandler : IRequestHandler<CandidateSkillDeleteCommand, Unit>
    {
        private readonly ICandidateSkillService _candidateSkillService;

        public CandidateSkillDeleteHandler(ICandidateSkillService candidateSkillService)
        {
            _candidateSkillService = candidateSkillService;
        }

        public async Task<Unit> Handle(CandidateSkillDeleteCommand command, CancellationToken cancellationToken)
        {
            await _candidateSkillService.DeleteAsync(command.CandidateSkillId);
            return Unit.Value;
        }
    }
}

using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.TalentPools.Commands.TalentPoolCandidateRemove
{
    public class TalentPoolCandidateRemoveHandler : IRequestHandler<TalentPoolCandidateRemoveCommand>
    {
        private readonly ITalentPoolService _talentPoolService;

        public TalentPoolCandidateRemoveHandler(ITalentPoolService talentPoolService)
        {
            _talentPoolService = talentPoolService;
        }

        public async Task Handle(TalentPoolCandidateRemoveCommand command, CancellationToken cancellationToken)
        {
            await _talentPoolService.RemoveCandidateAsync(command.TalentPoolId, command.CandidateProfileId);
        }
    }
}

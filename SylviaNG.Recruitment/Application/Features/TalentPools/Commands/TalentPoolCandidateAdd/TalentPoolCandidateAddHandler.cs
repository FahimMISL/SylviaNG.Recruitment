using MediatR;
using SylviaNG.Recruitment.Application.Features.TalentPools.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.TalentPools.Commands.TalentPoolCandidateAdd
{
    public class TalentPoolCandidateAddHandler : IRequestHandler<TalentPoolCandidateAddCommand, TalentPoolCandidateAddResponse>
    {
        private readonly ITalentPoolService _talentPoolService;

        public TalentPoolCandidateAddHandler(ITalentPoolService talentPoolService)
        {
            _talentPoolService = talentPoolService;
        }

        public async Task<TalentPoolCandidateAddResponse> Handle(TalentPoolCandidateAddCommand command, CancellationToken cancellationToken)
        {
            return await _talentPoolService.AddCandidatesAsync(command.TalentPoolId, command.Request);
        }
    }
}

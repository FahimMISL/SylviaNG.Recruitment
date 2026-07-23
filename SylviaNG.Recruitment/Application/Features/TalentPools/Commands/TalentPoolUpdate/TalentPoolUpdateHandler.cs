using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.TalentPools.Commands.TalentPoolUpdate
{
    public class TalentPoolUpdateHandler : IRequestHandler<TalentPoolUpdateCommand>
    {
        private readonly ITalentPoolService _talentPoolService;

        public TalentPoolUpdateHandler(ITalentPoolService talentPoolService)
        {
            _talentPoolService = talentPoolService;
        }

        public async Task Handle(TalentPoolUpdateCommand command, CancellationToken cancellationToken)
        {
            await _talentPoolService.UpdateAsync(command.TalentPoolId, command.Request);
        }
    }
}

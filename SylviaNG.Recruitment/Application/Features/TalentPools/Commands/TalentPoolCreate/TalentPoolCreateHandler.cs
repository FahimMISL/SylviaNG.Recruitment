using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.TalentPools.Commands.TalentPoolCreate
{
    public class TalentPoolCreateHandler : IRequestHandler<TalentPoolCreateCommand, long>
    {
        private readonly ITalentPoolService _talentPoolService;

        public TalentPoolCreateHandler(ITalentPoolService talentPoolService)
        {
            _talentPoolService = talentPoolService;
        }

        public async Task<long> Handle(TalentPoolCreateCommand command, CancellationToken cancellationToken)
        {
            return await _talentPoolService.CreateAsync(command.Request);
        }
    }
}

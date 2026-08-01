using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.TalentPools.Commands.TalentPoolDelete
{
    public class TalentPoolDeleteHandler : IRequestHandler<TalentPoolDeleteCommand>
    {
        private readonly ITalentPoolService _talentPoolService;

        public TalentPoolDeleteHandler(ITalentPoolService talentPoolService)
        {
            _talentPoolService = talentPoolService;
        }

        public async Task Handle(TalentPoolDeleteCommand command, CancellationToken cancellationToken)
        {
            await _talentPoolService.DeleteAsync(command.TalentPoolId);
        }
    }
}

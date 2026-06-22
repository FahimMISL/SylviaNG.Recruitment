using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.TalentPools.Commands.TalentPoolUpdate
{
    public class TalentPoolUpdateHandler : IRequestHandler<TalentPoolUpdateCommand, Unit>
    {
        private readonly ITalentPoolService _service;

        public TalentPoolUpdateHandler(ITalentPoolService service)
        {
            _service = service;
        }

        public async Task<Unit> Handle(TalentPoolUpdateCommand command, CancellationToken cancellationToken)
        {
            await _service.UpdateAsync(command.TalentPoolId, command.Request);
            return Unit.Value;
        }
    }
}

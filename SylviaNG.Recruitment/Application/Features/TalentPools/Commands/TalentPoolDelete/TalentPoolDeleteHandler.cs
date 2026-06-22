using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.TalentPools.Commands.TalentPoolDelete
{
    public class TalentPoolDeleteHandler : IRequestHandler<TalentPoolDeleteCommand, Unit>
    {
        private readonly ITalentPoolService _service;

        public TalentPoolDeleteHandler(ITalentPoolService service)
        {
            _service = service;
        }

        public async Task<Unit> Handle(TalentPoolDeleteCommand command, CancellationToken cancellationToken)
        {
            await _service.DeleteAsync(command.TalentPoolId);
            return Unit.Value;
        }
    }
}

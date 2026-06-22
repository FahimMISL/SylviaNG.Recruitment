using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.FinalSelectionPools.Commands.FinalSelectionPoolUpdate
{
    public class FinalSelectionPoolUpdateHandler : IRequestHandler<FinalSelectionPoolUpdateCommand, Unit>
    {
        private readonly IFinalSelectionPoolService _service;

        public FinalSelectionPoolUpdateHandler(IFinalSelectionPoolService service)
        {
            _service = service;
        }

        public async Task<Unit> Handle(FinalSelectionPoolUpdateCommand command, CancellationToken cancellationToken)
        {
            await _service.UpdateAsync(command.FinalSelectionPoolId, command.Request);
            return Unit.Value;
        }
    }
}

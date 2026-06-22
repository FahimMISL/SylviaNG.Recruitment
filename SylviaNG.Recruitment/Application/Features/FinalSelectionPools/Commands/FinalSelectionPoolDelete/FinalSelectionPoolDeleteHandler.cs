using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.FinalSelectionPools.Commands.FinalSelectionPoolDelete
{
    public class FinalSelectionPoolDeleteHandler : IRequestHandler<FinalSelectionPoolDeleteCommand, Unit>
    {
        private readonly IFinalSelectionPoolService _service;

        public FinalSelectionPoolDeleteHandler(IFinalSelectionPoolService service)
        {
            _service = service;
        }

        public async Task<Unit> Handle(FinalSelectionPoolDeleteCommand command, CancellationToken cancellationToken)
        {
            await _service.DeleteAsync(command.FinalSelectionPoolId);
            return Unit.Value;
        }
    }
}

using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.FitmentDatas.Commands.FitmentDataDelete
{
    public class FitmentDataDeleteHandler : IRequestHandler<FitmentDataDeleteCommand, Unit>
    {
        private readonly IFitmentDataService _service;

        public FitmentDataDeleteHandler(IFitmentDataService service)
        {
            _service = service;
        }

        public async Task<Unit> Handle(FitmentDataDeleteCommand command, CancellationToken cancellationToken)
        {
            await _service.DeleteAsync(command.FitmentDataId);
            return Unit.Value;
        }
    }
}

using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.FitmentDatas.Commands.FitmentDataUpdate
{
    public class FitmentDataUpdateHandler : IRequestHandler<FitmentDataUpdateCommand, Unit>
    {
        private readonly IFitmentDataService _service;

        public FitmentDataUpdateHandler(IFitmentDataService service)
        {
            _service = service;
        }

        public async Task<Unit> Handle(FitmentDataUpdateCommand command, CancellationToken cancellationToken)
        {
            await _service.UpdateAsync(command.FitmentDataId, command.Request);
            return Unit.Value;
        }
    }
}

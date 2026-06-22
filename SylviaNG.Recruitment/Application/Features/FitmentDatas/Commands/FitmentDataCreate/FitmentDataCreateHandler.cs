using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.FitmentDatas.Commands.FitmentDataCreate
{
    public class FitmentDataCreateHandler : IRequestHandler<FitmentDataCreateCommand, long>
    {
        private readonly IFitmentDataService _service;

        public FitmentDataCreateHandler(IFitmentDataService service)
        {
            _service = service;
        }

        public async Task<long> Handle(FitmentDataCreateCommand command, CancellationToken cancellationToken)
        {
            return await _service.CreateAsync(command.Request);
        }
    }
}

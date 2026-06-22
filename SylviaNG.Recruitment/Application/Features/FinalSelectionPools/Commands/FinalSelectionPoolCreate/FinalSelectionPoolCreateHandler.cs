using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.FinalSelectionPools.Commands.FinalSelectionPoolCreate
{
    public class FinalSelectionPoolCreateHandler : IRequestHandler<FinalSelectionPoolCreateCommand, long>
    {
        private readonly IFinalSelectionPoolService _service;

        public FinalSelectionPoolCreateHandler(IFinalSelectionPoolService service)
        {
            _service = service;
        }

        public async Task<long> Handle(FinalSelectionPoolCreateCommand command, CancellationToken cancellationToken)
        {
            return await _service.CreateAsync(command.Request);
        }
    }
}

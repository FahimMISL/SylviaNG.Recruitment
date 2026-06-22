using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.ApplicationScreeningResults.Commands.ApplicationScreeningResultUpdate
{
    public class ApplicationScreeningResultUpdateHandler : IRequestHandler<ApplicationScreeningResultUpdateCommand, Unit>
    {
        private readonly IApplicationScreeningResultService _service;

        public ApplicationScreeningResultUpdateHandler(IApplicationScreeningResultService service)
        {
            _service = service;
        }

        public async Task<Unit> Handle(ApplicationScreeningResultUpdateCommand command, CancellationToken cancellationToken)
        {
            await _service.UpdateAsync(command.ApplicationScreeningResultId, command.Request);
            return Unit.Value;
        }
    }
}

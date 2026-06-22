using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.ApplicationScreeningResults.Commands.ApplicationScreeningResultDelete
{
    public class ApplicationScreeningResultDeleteHandler : IRequestHandler<ApplicationScreeningResultDeleteCommand, Unit>
    {
        private readonly IApplicationScreeningResultService _service;

        public ApplicationScreeningResultDeleteHandler(IApplicationScreeningResultService service)
        {
            _service = service;
        }

        public async Task<Unit> Handle(ApplicationScreeningResultDeleteCommand command, CancellationToken cancellationToken)
        {
            await _service.DeleteAsync(command.ApplicationScreeningResultId);
            return Unit.Value;
        }
    }
}

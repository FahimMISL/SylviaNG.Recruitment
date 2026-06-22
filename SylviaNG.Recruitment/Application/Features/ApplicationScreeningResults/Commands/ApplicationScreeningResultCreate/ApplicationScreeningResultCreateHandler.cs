using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.ApplicationScreeningResults.Commands.ApplicationScreeningResultCreate
{
    public class ApplicationScreeningResultCreateHandler : IRequestHandler<ApplicationScreeningResultCreateCommand, long>
    {
        private readonly IApplicationScreeningResultService _service;

        public ApplicationScreeningResultCreateHandler(IApplicationScreeningResultService service)
        {
            _service = service;
        }

        public async Task<long> Handle(ApplicationScreeningResultCreateCommand command, CancellationToken cancellationToken)
        {
            return await _service.CreateAsync(command.Request);
        }
    }
}

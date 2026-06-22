using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.ApplicationDuplicates.Commands.ApplicationDuplicateUpdate
{
    public class ApplicationDuplicateUpdateHandler : IRequestHandler<ApplicationDuplicateUpdateCommand, Unit>
    {
        private readonly IApplicationDuplicateService _service;

        public ApplicationDuplicateUpdateHandler(IApplicationDuplicateService service)
        {
            _service = service;
        }

        public async Task<Unit> Handle(ApplicationDuplicateUpdateCommand command, CancellationToken cancellationToken)
        {
            await _service.UpdateAsync(command.ApplicationDuplicateId, command.Request);
            return Unit.Value;
        }
    }
}

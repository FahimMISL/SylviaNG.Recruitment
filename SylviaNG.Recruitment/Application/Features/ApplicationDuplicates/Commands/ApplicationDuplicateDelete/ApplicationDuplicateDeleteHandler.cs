using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.ApplicationDuplicates.Commands.ApplicationDuplicateDelete
{
    public class ApplicationDuplicateDeleteHandler : IRequestHandler<ApplicationDuplicateDeleteCommand, Unit>
    {
        private readonly IApplicationDuplicateService _service;

        public ApplicationDuplicateDeleteHandler(IApplicationDuplicateService service)
        {
            _service = service;
        }

        public async Task<Unit> Handle(ApplicationDuplicateDeleteCommand command, CancellationToken cancellationToken)
        {
            await _service.DeleteAsync(command.ApplicationDuplicateId);
            return Unit.Value;
        }
    }
}

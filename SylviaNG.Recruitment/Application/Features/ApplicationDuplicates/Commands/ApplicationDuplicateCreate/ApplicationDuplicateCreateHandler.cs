using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.ApplicationDuplicates.Commands.ApplicationDuplicateCreate
{
    public class ApplicationDuplicateCreateHandler : IRequestHandler<ApplicationDuplicateCreateCommand, long>
    {
        private readonly IApplicationDuplicateService _service;

        public ApplicationDuplicateCreateHandler(IApplicationDuplicateService service)
        {
            _service = service;
        }

        public async Task<long> Handle(ApplicationDuplicateCreateCommand command, CancellationToken cancellationToken)
        {
            return await _service.CreateAsync(command.Request);
        }
    }
}

using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.ImpersonationLogs.Commands.ImpersonationLogCreate
{
    public class ImpersonationLogCreateHandler : IRequestHandler<ImpersonationLogCreateCommand, long>
    {
        private readonly IImpersonationLogService _service;

        public ImpersonationLogCreateHandler(IImpersonationLogService service)
        {
            _service = service;
        }

        public async Task<long> Handle(ImpersonationLogCreateCommand command, CancellationToken cancellationToken)
        {
            return await _service.CreateAsync(command.Request);
        }
    }
}

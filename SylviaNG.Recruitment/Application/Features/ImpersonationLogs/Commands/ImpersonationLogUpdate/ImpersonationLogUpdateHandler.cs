using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.ImpersonationLogs.Commands.ImpersonationLogUpdate
{
    public class ImpersonationLogUpdateHandler : IRequestHandler<ImpersonationLogUpdateCommand, Unit>
    {
        private readonly IImpersonationLogService _service;

        public ImpersonationLogUpdateHandler(IImpersonationLogService service)
        {
            _service = service;
        }

        public async Task<Unit> Handle(ImpersonationLogUpdateCommand command, CancellationToken cancellationToken)
        {
            await _service.UpdateAsync(command.ImpersonationLogId, command.Request);
            return Unit.Value;
        }
    }
}

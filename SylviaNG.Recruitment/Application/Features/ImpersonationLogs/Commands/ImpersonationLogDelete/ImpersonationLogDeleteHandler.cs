using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.ImpersonationLogs.Commands.ImpersonationLogDelete
{
    public class ImpersonationLogDeleteHandler : IRequestHandler<ImpersonationLogDeleteCommand, Unit>
    {
        private readonly IImpersonationLogService _service;

        public ImpersonationLogDeleteHandler(IImpersonationLogService service)
        {
            _service = service;
        }

        public async Task<Unit> Handle(ImpersonationLogDeleteCommand command, CancellationToken cancellationToken)
        {
            await _service.DeleteAsync(command.ImpersonationLogId);
            return Unit.Value;
        }
    }
}

using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.ExportRequests.Commands.ExportRequestUpdate
{
    public class ExportRequestUpdateHandler : IRequestHandler<ExportRequestUpdateCommand, Unit>
    {
        private readonly IExportRequestService _service;

        public ExportRequestUpdateHandler(IExportRequestService service)
        {
            _service = service;
        }

        public async Task<Unit> Handle(ExportRequestUpdateCommand command, CancellationToken cancellationToken)
        {
            await _service.UpdateAsync(command.ExportRequestId, command.Request);
            return Unit.Value;
        }
    }
}

using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.ExportRequests.Commands.ExportRequestDelete
{
    public class ExportRequestDeleteHandler : IRequestHandler<ExportRequestDeleteCommand, Unit>
    {
        private readonly IExportRequestService _service;

        public ExportRequestDeleteHandler(IExportRequestService service)
        {
            _service = service;
        }

        public async Task<Unit> Handle(ExportRequestDeleteCommand command, CancellationToken cancellationToken)
        {
            await _service.DeleteAsync(command.ExportRequestId);
            return Unit.Value;
        }
    }
}

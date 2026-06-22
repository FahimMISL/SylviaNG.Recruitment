using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.ExportRequests.Commands.ExportRequestCreate
{
    public class ExportRequestCreateHandler : IRequestHandler<ExportRequestCreateCommand, long>
    {
        private readonly IExportRequestService _service;

        public ExportRequestCreateHandler(IExportRequestService service)
        {
            _service = service;
        }

        public async Task<long> Handle(ExportRequestCreateCommand command, CancellationToken cancellationToken)
        {
            return await _service.CreateAsync(command.Request);
        }
    }
}

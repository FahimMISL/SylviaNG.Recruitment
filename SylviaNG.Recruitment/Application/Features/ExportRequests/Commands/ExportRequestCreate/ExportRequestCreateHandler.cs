using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.ExportRequests.Commands.ExportRequestCreate
{
    public class ExportRequestCreateHandler : IRequestHandler<ExportRequestCreateCommand, long>
    {
        private readonly IExportRequestService _exportRequestService;

        public ExportRequestCreateHandler(IExportRequestService exportRequestService)
        {
            _exportRequestService = exportRequestService;
        }

        public async Task<long> Handle(ExportRequestCreateCommand command, CancellationToken cancellationToken)
        {
            return await _exportRequestService.RequestCandidateListExportAsync(command.Request.Filter, command.Request.Format);
        }
    }
}

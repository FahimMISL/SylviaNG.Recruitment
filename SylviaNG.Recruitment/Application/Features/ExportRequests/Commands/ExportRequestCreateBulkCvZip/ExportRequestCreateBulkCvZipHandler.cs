using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.ExportRequests.Commands.ExportRequestCreateBulkCvZip
{
    public class ExportRequestCreateBulkCvZipHandler : IRequestHandler<ExportRequestCreateBulkCvZipCommand, long>
    {
        private readonly IExportRequestService _exportRequestService;

        public ExportRequestCreateBulkCvZipHandler(IExportRequestService exportRequestService)
        {
            _exportRequestService = exportRequestService;
        }

        public async Task<long> Handle(ExportRequestCreateBulkCvZipCommand command, CancellationToken cancellationToken)
        {
            return await _exportRequestService.RequestBulkCvZipExportAsync(command.JobApplicationIds);
        }
    }
}

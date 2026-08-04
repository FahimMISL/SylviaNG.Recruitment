using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.ExportRequests.Queries.ExportRequestDownload
{
    public class ExportRequestDownloadHandler : IRequestHandler<ExportRequestDownloadQuery, ExportRequestFileResponse>
    {
        private readonly IExportRequestService _exportRequestService;

        public ExportRequestDownloadHandler(IExportRequestService exportRequestService)
        {
            _exportRequestService = exportRequestService;
        }

        public async Task<ExportRequestFileResponse> Handle(ExportRequestDownloadQuery query, CancellationToken cancellationToken)
        {
            return await _exportRequestService.GetForDownloadAsync(query.ExportRequestId);
        }
    }
}

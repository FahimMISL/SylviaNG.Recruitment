using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.ExportRequests.Queries.ExportRequestDownload
{
    public class ExportRequestDownloadQuery : IRequest<ExportRequestFileResponse>
    {
        public long ExportRequestId { get; }

        public ExportRequestDownloadQuery(long exportRequestId)
        {
            ExportRequestId = exportRequestId;
        }
    }
}

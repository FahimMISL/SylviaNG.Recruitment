using MediatR;
using SylviaNG.Recruitment.Application.Features.ExportRequests.Models;

namespace SylviaNG.Recruitment.Application.Features.ExportRequests.Queries.ExportRequestGetById
{
    public class ExportRequestGetByIdQuery : IRequest<ExportRequestResponse>
    {
        public long ExportRequestId { get; set; }

        public ExportRequestGetByIdQuery(long exportRequestId)
        {
            ExportRequestId = exportRequestId;
        }
    }
}

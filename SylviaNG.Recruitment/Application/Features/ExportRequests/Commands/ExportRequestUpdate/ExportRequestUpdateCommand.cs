using MediatR;
using SylviaNG.Recruitment.Application.Features.ExportRequests.Models;

namespace SylviaNG.Recruitment.Application.Features.ExportRequests.Commands.ExportRequestUpdate
{
    public class ExportRequestUpdateCommand : IRequest<Unit>
    {
        public long ExportRequestId { get; set; }
        public ExportRequestUpdateRequest Request { get; set; }

        public ExportRequestUpdateCommand(long exportRequestId, ExportRequestUpdateRequest request)
        {
            ExportRequestId = exportRequestId;
            Request = request;
        }
    }
}

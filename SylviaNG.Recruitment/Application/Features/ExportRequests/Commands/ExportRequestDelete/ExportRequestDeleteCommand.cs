using MediatR;

namespace SylviaNG.Recruitment.Application.Features.ExportRequests.Commands.ExportRequestDelete
{
    public class ExportRequestDeleteCommand : IRequest<Unit>
    {
        public long ExportRequestId { get; set; }

        public ExportRequestDeleteCommand(long exportRequestId)
        {
            ExportRequestId = exportRequestId;
        }
    }
}

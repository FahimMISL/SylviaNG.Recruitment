using MediatR;
using SylviaNG.Recruitment.Application.Features.DocumentAcceptances.Models;

namespace SylviaNG.Recruitment.Application.Features.DocumentAcceptances.Commands.DocumentAcceptanceUpdate
{
    public class DocumentAcceptanceUpdateCommand : IRequest<Unit>
    {
        public long DocumentAcceptanceId { get; set; }
        public DocumentAcceptanceUpdateRequest Request { get; set; }

        public DocumentAcceptanceUpdateCommand(long documentAcceptanceId, DocumentAcceptanceUpdateRequest request)
        {
            DocumentAcceptanceId = documentAcceptanceId;
            Request = request;
        }
    }
}

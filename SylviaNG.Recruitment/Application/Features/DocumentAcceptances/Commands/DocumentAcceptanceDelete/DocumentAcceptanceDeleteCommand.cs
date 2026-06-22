using MediatR;

namespace SylviaNG.Recruitment.Application.Features.DocumentAcceptances.Commands.DocumentAcceptanceDelete
{
    public class DocumentAcceptanceDeleteCommand : IRequest<Unit>
    {
        public long DocumentAcceptanceId { get; set; }

        public DocumentAcceptanceDeleteCommand(long documentAcceptanceId)
        {
            DocumentAcceptanceId = documentAcceptanceId;
        }
    }
}

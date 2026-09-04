using MediatR;
using SylviaNG.Recruitment.Domain.Enums;

namespace SylviaNG.Recruitment.Application.Features.DocumentTracking.Commands.DocumentTrackingFollowUp
{
    public class DocumentTrackingFollowUpCommand : IRequest<Unit>
    {
        public DocumentTypeEnum DocumentType { get; set; }
        public long SourceId { get; set; }

        public DocumentTrackingFollowUpCommand(DocumentTypeEnum documentType, long sourceId)
        {
            DocumentType = documentType;
            SourceId = sourceId;
        }
    }
}

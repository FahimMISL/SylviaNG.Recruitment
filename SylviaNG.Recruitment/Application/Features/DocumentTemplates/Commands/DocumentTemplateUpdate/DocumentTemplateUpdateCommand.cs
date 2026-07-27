using MediatR;
using SylviaNG.Recruitment.Application.Features.DocumentTemplates.Models;

namespace SylviaNG.Recruitment.Application.Features.DocumentTemplates.Commands.DocumentTemplateUpdate
{
    public class DocumentTemplateUpdateCommand : IRequest<Unit>
    {
        public long DocumentTemplateId { get; set; }
        public DocumentTemplateUpdateRequest Request { get; set; }

        public DocumentTemplateUpdateCommand(long documentTemplateId, DocumentTemplateUpdateRequest request)
        {
            DocumentTemplateId = documentTemplateId;
            Request = request;
        }
    }
}

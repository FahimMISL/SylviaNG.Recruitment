using MediatR;
using SylviaNG.Recruitment.Application.Features.DocumentTemplateVersions.Models;

namespace SylviaNG.Recruitment.Application.Features.DocumentTemplateVersions.Commands.DocumentTemplateVersionUpdate
{
    public class DocumentTemplateVersionUpdateCommand : IRequest<Unit>
    {
        public long DocumentTemplateVersionId { get; set; }
        public DocumentTemplateVersionUpdateRequest Request { get; set; }

        public DocumentTemplateVersionUpdateCommand(long documentTemplateVersionId, DocumentTemplateVersionUpdateRequest request)
        {
            DocumentTemplateVersionId = documentTemplateVersionId;
            Request = request;
        }
    }
}

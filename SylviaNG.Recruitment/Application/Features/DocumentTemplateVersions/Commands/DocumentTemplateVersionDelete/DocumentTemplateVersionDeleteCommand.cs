using MediatR;

namespace SylviaNG.Recruitment.Application.Features.DocumentTemplateVersions.Commands.DocumentTemplateVersionDelete
{
    public class DocumentTemplateVersionDeleteCommand : IRequest<Unit>
    {
        public long DocumentTemplateVersionId { get; set; }

        public DocumentTemplateVersionDeleteCommand(long documentTemplateVersionId)
        {
            DocumentTemplateVersionId = documentTemplateVersionId;
        }
    }
}

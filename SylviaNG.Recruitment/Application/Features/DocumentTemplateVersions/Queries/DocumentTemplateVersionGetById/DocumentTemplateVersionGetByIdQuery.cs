using MediatR;
using SylviaNG.Recruitment.Application.Features.DocumentTemplateVersions.Models;

namespace SylviaNG.Recruitment.Application.Features.DocumentTemplateVersions.Queries.DocumentTemplateVersionGetById
{
    public class DocumentTemplateVersionGetByIdQuery : IRequest<DocumentTemplateVersionResponse>
    {
        public long DocumentTemplateVersionId { get; set; }

        public DocumentTemplateVersionGetByIdQuery(long documentTemplateVersionId)
        {
            DocumentTemplateVersionId = documentTemplateVersionId;
        }
    }
}

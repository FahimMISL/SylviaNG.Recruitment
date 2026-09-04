using MediatR;
using SylviaNG.Recruitment.Application.Features.DocumentTemplates.Models;

namespace SylviaNG.Recruitment.Application.Features.DocumentTemplates.Queries.DocumentTemplateGetVersions
{
    public class DocumentTemplateGetVersionsQuery : IRequest<List<DocumentTemplateVersionResponse>>
    {
        public long DocumentTemplateId { get; set; }

        public DocumentTemplateGetVersionsQuery(long documentTemplateId)
        {
            DocumentTemplateId = documentTemplateId;
        }
    }
}

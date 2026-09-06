using MediatR;
using SylviaNG.Recruitment.Application.Features.DocumentTemplates.Models;

namespace SylviaNG.Recruitment.Application.Features.DocumentTemplates.Queries.DocumentTemplateGetById
{
    public class DocumentTemplateGetByIdQuery : IRequest<DocumentTemplateResponse>
    {
        public long DocumentTemplateId { get; set; }

        public DocumentTemplateGetByIdQuery(long documentTemplateId)
        {
            DocumentTemplateId = documentTemplateId;
        }
    }
}

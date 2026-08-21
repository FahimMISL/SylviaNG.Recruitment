using MediatR;
using SylviaNG.Recruitment.Application.Features.DocumentTemplates.Models;

namespace SylviaNG.Recruitment.Application.Features.DocumentTemplates.Queries.DocumentTemplatePreview
{
    public class DocumentTemplatePreviewQuery : IRequest<DocumentTemplatePreviewResponse>
    {
        public DocumentTemplatePreviewRequest Request { get; set; }

        public DocumentTemplatePreviewQuery(DocumentTemplatePreviewRequest request)
        {
            Request = request;
        }
    }
}

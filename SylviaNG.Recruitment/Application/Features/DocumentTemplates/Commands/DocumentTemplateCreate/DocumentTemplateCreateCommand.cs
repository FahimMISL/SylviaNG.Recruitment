using MediatR;
using SylviaNG.Recruitment.Application.Features.DocumentTemplates.Models;

namespace SylviaNG.Recruitment.Application.Features.DocumentTemplates.Commands.DocumentTemplateCreate
{
    public class DocumentTemplateCreateCommand : IRequest<long>
    {
        public DocumentTemplateCreateRequest Request { get; set; }

        public DocumentTemplateCreateCommand(DocumentTemplateCreateRequest request)
        {
            Request = request;
        }
    }
}

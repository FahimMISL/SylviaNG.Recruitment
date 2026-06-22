using MediatR;
using SylviaNG.Recruitment.Application.Features.DocumentTemplateVersions.Models;

namespace SylviaNG.Recruitment.Application.Features.DocumentTemplateVersions.Commands.DocumentTemplateVersionCreate
{
    public class DocumentTemplateVersionCreateCommand : IRequest<long>
    {
        public DocumentTemplateVersionCreateRequest Request { get; set; }

        public DocumentTemplateVersionCreateCommand(DocumentTemplateVersionCreateRequest request)
        {
            Request = request;
        }
    }
}

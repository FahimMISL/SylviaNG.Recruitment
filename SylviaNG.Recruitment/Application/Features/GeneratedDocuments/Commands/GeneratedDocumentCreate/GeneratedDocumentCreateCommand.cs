using MediatR;
using SylviaNG.Recruitment.Application.Features.GeneratedDocuments.Models;

namespace SylviaNG.Recruitment.Application.Features.GeneratedDocuments.Commands.GeneratedDocumentCreate
{
    public class GeneratedDocumentCreateCommand : IRequest<long>
    {
        public GeneratedDocumentCreateRequest Request { get; set; }

        public GeneratedDocumentCreateCommand(GeneratedDocumentCreateRequest request)
        {
            Request = request;
        }
    }
}

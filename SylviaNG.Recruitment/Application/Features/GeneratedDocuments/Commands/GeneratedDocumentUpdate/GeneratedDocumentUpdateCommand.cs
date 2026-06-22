using MediatR;
using SylviaNG.Recruitment.Application.Features.GeneratedDocuments.Models;

namespace SylviaNG.Recruitment.Application.Features.GeneratedDocuments.Commands.GeneratedDocumentUpdate
{
    public class GeneratedDocumentUpdateCommand : IRequest<Unit>
    {
        public long GeneratedDocumentId { get; set; }
        public GeneratedDocumentUpdateRequest Request { get; set; }

        public GeneratedDocumentUpdateCommand(long generatedDocumentId, GeneratedDocumentUpdateRequest request)
        {
            GeneratedDocumentId = generatedDocumentId;
            Request = request;
        }
    }
}

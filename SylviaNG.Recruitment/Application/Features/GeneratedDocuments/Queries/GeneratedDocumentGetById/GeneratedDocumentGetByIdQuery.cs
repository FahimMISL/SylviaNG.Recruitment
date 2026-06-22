using MediatR;
using SylviaNG.Recruitment.Application.Features.GeneratedDocuments.Models;

namespace SylviaNG.Recruitment.Application.Features.GeneratedDocuments.Queries.GeneratedDocumentGetById
{
    public class GeneratedDocumentGetByIdQuery : IRequest<GeneratedDocumentResponse>
    {
        public long GeneratedDocumentId { get; set; }

        public GeneratedDocumentGetByIdQuery(long generatedDocumentId)
        {
            GeneratedDocumentId = generatedDocumentId;
        }
    }
}

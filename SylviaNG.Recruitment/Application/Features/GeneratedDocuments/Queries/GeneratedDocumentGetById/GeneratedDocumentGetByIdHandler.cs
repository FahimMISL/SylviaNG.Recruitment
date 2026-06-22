using MediatR;
using SylviaNG.Recruitment.Application.Features.GeneratedDocuments.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.GeneratedDocuments.Queries.GeneratedDocumentGetById
{
    public class GeneratedDocumentGetByIdHandler : IRequestHandler<GeneratedDocumentGetByIdQuery, GeneratedDocumentResponse>
    {
        private readonly IGeneratedDocumentService _service;

        public GeneratedDocumentGetByIdHandler(IGeneratedDocumentService service)
        {
            _service = service;
        }

        public async Task<GeneratedDocumentResponse> Handle(GeneratedDocumentGetByIdQuery query, CancellationToken cancellationToken)
        {
            return await _service.GetByIdAsync(query.GeneratedDocumentId);
        }
    }
}

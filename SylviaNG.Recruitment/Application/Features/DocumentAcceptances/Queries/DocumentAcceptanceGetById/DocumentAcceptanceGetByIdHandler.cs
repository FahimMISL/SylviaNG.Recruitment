using MediatR;
using SylviaNG.Recruitment.Application.Features.DocumentAcceptances.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.DocumentAcceptances.Queries.DocumentAcceptanceGetById
{
    public class DocumentAcceptanceGetByIdHandler : IRequestHandler<DocumentAcceptanceGetByIdQuery, DocumentAcceptanceResponse>
    {
        private readonly IDocumentAcceptanceService _service;

        public DocumentAcceptanceGetByIdHandler(IDocumentAcceptanceService service)
        {
            _service = service;
        }

        public async Task<DocumentAcceptanceResponse> Handle(DocumentAcceptanceGetByIdQuery query, CancellationToken cancellationToken)
        {
            return await _service.GetByIdAsync(query.DocumentAcceptanceId);
        }
    }
}

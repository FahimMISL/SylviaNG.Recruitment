using MediatR;
using SylviaNG.Recruitment.Application.Features.DocumentAcceptances.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.DocumentAcceptances.Queries.DocumentAcceptanceGetAll
{
    public class DocumentAcceptanceGetAllHandler : IRequestHandler<DocumentAcceptanceGetAllQuery, List<DocumentAcceptanceResponse>>
    {
        private readonly IDocumentAcceptanceService _service;

        public DocumentAcceptanceGetAllHandler(IDocumentAcceptanceService service)
        {
            _service = service;
        }

        public async Task<List<DocumentAcceptanceResponse>> Handle(DocumentAcceptanceGetAllQuery query, CancellationToken cancellationToken)
        {
            return await _service.GetAllAsync();
        }
    }
}

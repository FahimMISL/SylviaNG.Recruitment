using MediatR;
using SylviaNG.Recruitment.Application.Features.GeneratedDocuments.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.GeneratedDocuments.Queries.GeneratedDocumentGetAll
{
    public class GeneratedDocumentGetAllHandler : IRequestHandler<GeneratedDocumentGetAllQuery, List<GeneratedDocumentResponse>>
    {
        private readonly IGeneratedDocumentService _service;

        public GeneratedDocumentGetAllHandler(IGeneratedDocumentService service)
        {
            _service = service;
        }

        public async Task<List<GeneratedDocumentResponse>> Handle(GeneratedDocumentGetAllQuery query, CancellationToken cancellationToken)
        {
            return await _service.GetAllAsync();
        }
    }
}

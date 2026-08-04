using MediatR;
using SylviaNG.Recruitment.Application.Features.DocumentTracking.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.SharedKernel.Pagination;

namespace SylviaNG.Recruitment.Application.Features.DocumentTracking.Queries.DocumentTrackingGetAll
{
    public class DocumentTrackingGetAllHandler : IRequestHandler<DocumentTrackingGetAllQuery, PagedResult<DocumentTrackingItemResponse>>
    {
        private readonly IDocumentTrackingService _documentTrackingService;

        public DocumentTrackingGetAllHandler(IDocumentTrackingService documentTrackingService)
        {
            _documentTrackingService = documentTrackingService;
        }

        public async Task<PagedResult<DocumentTrackingItemResponse>> Handle(DocumentTrackingGetAllQuery query, CancellationToken cancellationToken)
        {
            return await _documentTrackingService.GetAllAsync(query.Filter);
        }
    }
}

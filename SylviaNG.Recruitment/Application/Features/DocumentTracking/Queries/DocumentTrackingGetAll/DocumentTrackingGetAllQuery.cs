using MediatR;
using SylviaNG.Recruitment.Application.Features.DocumentTracking.Models;
using SylviaNG.Recruitment.SharedKernel.Pagination;

namespace SylviaNG.Recruitment.Application.Features.DocumentTracking.Queries.DocumentTrackingGetAll
{
    public class DocumentTrackingGetAllQuery : IRequest<PagedResult<DocumentTrackingItemResponse>>
    {
        public DocumentTrackingFilterRequest Filter { get; }

        public DocumentTrackingGetAllQuery(DocumentTrackingFilterRequest filter)
        {
            Filter = filter;
        }
    }
}

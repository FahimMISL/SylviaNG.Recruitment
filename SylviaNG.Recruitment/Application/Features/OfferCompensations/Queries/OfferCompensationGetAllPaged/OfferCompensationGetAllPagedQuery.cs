using MediatR;
using SylviaNG.Recruitment.Application.Features.OfferCompensations.Models;
using SylviaNG.Recruitment.SharedKernel.Pagination;

namespace SylviaNG.Recruitment.Application.Features.OfferCompensations.Queries.OfferCompensationGetAllPaged
{
    public class OfferCompensationGetAllPagedQuery : IRequest<PagedResult<OfferCompensationResponse>>
    {
        public PagedRequest Request { get; set; }

        public OfferCompensationGetAllPagedQuery(PagedRequest request)
        {
            Request = request;
        }
    }
}

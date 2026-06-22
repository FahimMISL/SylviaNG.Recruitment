using MediatR;
using SylviaNG.Recruitment.Application.Features.OfferCompensations.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.SharedKernel.Pagination;

namespace SylviaNG.Recruitment.Application.Features.OfferCompensations.Queries.OfferCompensationGetAllPaged
{
    public class OfferCompensationGetAllPagedHandler : IRequestHandler<OfferCompensationGetAllPagedQuery, PagedResult<OfferCompensationResponse>>
    {
        private readonly IOfferCompensationService _offerCompensationService;

        public OfferCompensationGetAllPagedHandler(IOfferCompensationService offerCompensationService)
        {
            _offerCompensationService = offerCompensationService;
        }

        public async Task<PagedResult<OfferCompensationResponse>> Handle(OfferCompensationGetAllPagedQuery query, CancellationToken cancellationToken)
        {
            return await _offerCompensationService.GetPaginatedAsync(query.Request);
        }
    }
}

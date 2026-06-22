using MediatR;
using SylviaNG.Recruitment.Application.Features.OfferCompensations.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.OfferCompensations.Queries.OfferCompensationGetAll
{
    public class OfferCompensationGetAllHandler : IRequestHandler<OfferCompensationGetAllQuery, List<OfferCompensationResponse>>
    {
        private readonly IOfferCompensationService _service;

        public OfferCompensationGetAllHandler(IOfferCompensationService service)
        {
            _service = service;
        }

        public async Task<List<OfferCompensationResponse>> Handle(OfferCompensationGetAllQuery query, CancellationToken cancellationToken)
        {
            return await _service.GetAllAsync();
        }
    }
}

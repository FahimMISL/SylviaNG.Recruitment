using MediatR;
using SylviaNG.Recruitment.Application.Features.OfferCompensations.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.OfferCompensations.Queries.OfferCompensationGetById
{
    public class OfferCompensationGetByIdHandler : IRequestHandler<OfferCompensationGetByIdQuery, OfferCompensationResponse>
    {
        private readonly IOfferCompensationService _service;

        public OfferCompensationGetByIdHandler(IOfferCompensationService service)
        {
            _service = service;
        }

        public async Task<OfferCompensationResponse> Handle(OfferCompensationGetByIdQuery query, CancellationToken cancellationToken)
        {
            return await _service.GetByIdAsync(query.OfferCompensationId);
        }
    }
}

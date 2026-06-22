using MediatR;
using SylviaNG.Recruitment.Application.Features.OfferCompensations.Models;

namespace SylviaNG.Recruitment.Application.Features.OfferCompensations.Queries.OfferCompensationGetById
{
    public class OfferCompensationGetByIdQuery : IRequest<OfferCompensationResponse>
    {
        public long OfferCompensationId { get; set; }

        public OfferCompensationGetByIdQuery(long offerCompensationId)
        {
            OfferCompensationId = offerCompensationId;
        }
    }
}

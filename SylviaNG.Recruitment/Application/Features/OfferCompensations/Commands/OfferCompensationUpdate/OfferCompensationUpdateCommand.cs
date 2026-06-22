using MediatR;
using SylviaNG.Recruitment.Application.Features.OfferCompensations.Models;

namespace SylviaNG.Recruitment.Application.Features.OfferCompensations.Commands.OfferCompensationUpdate
{
    public class OfferCompensationUpdateCommand : IRequest<Unit>
    {
        public long OfferCompensationId { get; set; }
        public OfferCompensationUpdateRequest Request { get; set; }

        public OfferCompensationUpdateCommand(long offerCompensationId, OfferCompensationUpdateRequest request)
        {
            OfferCompensationId = offerCompensationId;
            Request = request;
        }
    }
}

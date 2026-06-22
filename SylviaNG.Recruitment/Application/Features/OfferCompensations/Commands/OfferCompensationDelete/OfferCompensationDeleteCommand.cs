using MediatR;

namespace SylviaNG.Recruitment.Application.Features.OfferCompensations.Commands.OfferCompensationDelete
{
    public class OfferCompensationDeleteCommand : IRequest<Unit>
    {
        public long OfferCompensationId { get; set; }

        public OfferCompensationDeleteCommand(long offerCompensationId)
        {
            OfferCompensationId = offerCompensationId;
        }
    }
}

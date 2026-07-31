using MediatR;
using SylviaNG.Recruitment.Application.Features.OfferLetters.Models;

namespace SylviaNG.Recruitment.Application.Features.OfferLetters.Commands.OfferLetterAccept
{
    public class OfferLetterAcceptCommand : IRequest<OfferLetterResponse>
    {
        public long OfferLetterId { get; set; }

        public OfferLetterAcceptCommand(long offerLetterId)
        {
            OfferLetterId = offerLetterId;
        }
    }
}

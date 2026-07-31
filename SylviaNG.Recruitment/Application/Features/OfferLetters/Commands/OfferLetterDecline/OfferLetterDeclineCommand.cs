using MediatR;
using SylviaNG.Recruitment.Application.Features.OfferLetters.Models;

namespace SylviaNG.Recruitment.Application.Features.OfferLetters.Commands.OfferLetterDecline
{
    public class OfferLetterDeclineCommand : IRequest<OfferLetterResponse>
    {
        public long OfferLetterId { get; set; }
        public OfferLetterDeclineRequest Request { get; set; }

        public OfferLetterDeclineCommand(long offerLetterId, OfferLetterDeclineRequest request)
        {
            OfferLetterId = offerLetterId;
            Request = request;
        }
    }
}

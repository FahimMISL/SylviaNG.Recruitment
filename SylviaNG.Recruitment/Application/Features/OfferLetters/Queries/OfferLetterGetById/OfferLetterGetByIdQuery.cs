using MediatR;
using SylviaNG.Recruitment.Application.Features.OfferLetters.Models;

namespace SylviaNG.Recruitment.Application.Features.OfferLetters.Queries.OfferLetterGetById
{
    public class OfferLetterGetByIdQuery : IRequest<OfferLetterResponse>
    {
        public long OfferLetterId { get; set; }

        public OfferLetterGetByIdQuery(long offerLetterId)
        {
            OfferLetterId = offerLetterId;
        }
    }
}

using MediatR;
using SylviaNG.Recruitment.Application.Features.OfferLetters.Models;

namespace SylviaNG.Recruitment.Application.Features.OfferLetters.Queries.OfferLetterGetByIdForCandidate
{
    public class OfferLetterGetByIdForCandidateQuery : IRequest<OfferLetterResponse>
    {
        public long OfferLetterId { get; set; }

        public OfferLetterGetByIdForCandidateQuery(long offerLetterId)
        {
            OfferLetterId = offerLetterId;
        }
    }
}

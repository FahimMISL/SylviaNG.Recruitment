using MediatR;
using SylviaNG.Recruitment.Application.Features.OfferLetters.Models;

namespace SylviaNG.Recruitment.Application.Features.OfferLetters.Queries.OfferLetterGetAllForCandidate
{
    public class OfferLetterGetAllForCandidateQuery : IRequest<List<OfferLetterResponse>>
    {
    }
}

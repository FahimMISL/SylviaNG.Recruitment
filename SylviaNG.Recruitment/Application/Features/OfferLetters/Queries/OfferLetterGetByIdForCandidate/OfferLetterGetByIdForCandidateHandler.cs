using MediatR;
using SylviaNG.Recruitment.Application.Features.OfferLetters.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.OfferLetters.Queries.OfferLetterGetByIdForCandidate
{
    public class OfferLetterGetByIdForCandidateHandler : IRequestHandler<OfferLetterGetByIdForCandidateQuery, OfferLetterResponse>
    {
        private readonly IOfferLetterService _offerLetterService;

        public OfferLetterGetByIdForCandidateHandler(IOfferLetterService offerLetterService)
        {
            _offerLetterService = offerLetterService;
        }

        public async Task<OfferLetterResponse> Handle(OfferLetterGetByIdForCandidateQuery query, CancellationToken cancellationToken)
        {
            return await _offerLetterService.GetByIdForCandidateAsync(query.OfferLetterId);
        }
    }
}

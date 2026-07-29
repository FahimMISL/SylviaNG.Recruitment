using MediatR;
using SylviaNG.Recruitment.Application.Features.OfferLetters.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.OfferLetters.Queries.OfferLetterGetAllForCandidate
{
    public class OfferLetterGetAllForCandidateHandler : IRequestHandler<OfferLetterGetAllForCandidateQuery, List<OfferLetterResponse>>
    {
        private readonly IOfferLetterService _offerLetterService;

        public OfferLetterGetAllForCandidateHandler(IOfferLetterService offerLetterService)
        {
            _offerLetterService = offerLetterService;
        }

        public async Task<List<OfferLetterResponse>> Handle(OfferLetterGetAllForCandidateQuery query, CancellationToken cancellationToken)
        {
            return await _offerLetterService.GetAllForCandidateAsync();
        }
    }
}

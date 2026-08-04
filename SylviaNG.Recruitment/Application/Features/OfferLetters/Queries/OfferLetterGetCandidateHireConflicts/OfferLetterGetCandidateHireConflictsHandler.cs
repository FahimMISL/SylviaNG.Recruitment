using MediatR;
using SylviaNG.Recruitment.Application.Features.OfferLetters.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.OfferLetters.Queries.OfferLetterGetCandidateHireConflicts
{
    public class OfferLetterGetCandidateHireConflictsHandler : IRequestHandler<OfferLetterGetCandidateHireConflictsQuery, List<CandidateHireConflictResponse>>
    {
        private readonly IOfferLetterService _offerLetterService;

        public OfferLetterGetCandidateHireConflictsHandler(IOfferLetterService offerLetterService)
        {
            _offerLetterService = offerLetterService;
        }

        public async Task<List<CandidateHireConflictResponse>> Handle(OfferLetterGetCandidateHireConflictsQuery query, CancellationToken cancellationToken)
        {
            return await _offerLetterService.GetCandidateHireConflictsAsync(query.JobApplicationId);
        }
    }
}

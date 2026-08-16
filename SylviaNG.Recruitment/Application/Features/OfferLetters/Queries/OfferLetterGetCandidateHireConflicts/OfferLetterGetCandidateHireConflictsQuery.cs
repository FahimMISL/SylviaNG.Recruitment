using MediatR;
using SylviaNG.Recruitment.Application.Features.OfferLetters.Models;

namespace SylviaNG.Recruitment.Application.Features.OfferLetters.Queries.OfferLetterGetCandidateHireConflicts
{
    public class OfferLetterGetCandidateHireConflictsQuery : IRequest<List<CandidateHireConflictResponse>>
    {
        public long JobApplicationId { get; set; }

        public OfferLetterGetCandidateHireConflictsQuery(long jobApplicationId)
        {
            JobApplicationId = jobApplicationId;
        }
    }
}

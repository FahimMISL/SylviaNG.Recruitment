using MediatR;
using SylviaNG.Recruitment.Application.Features.OfferLetters.Models;

namespace SylviaNG.Recruitment.Application.Features.OfferLetters.Queries.OfferLetterGetAll
{
    public class OfferLetterGetAllQuery : IRequest<List<OfferLetterResponse>>
    {
        public long? JobApplicationId { get; set; }

        public OfferLetterGetAllQuery(long? jobApplicationId)
        {
            JobApplicationId = jobApplicationId;
        }
    }
}

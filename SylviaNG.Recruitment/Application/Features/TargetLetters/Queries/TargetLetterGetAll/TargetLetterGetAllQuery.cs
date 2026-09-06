using MediatR;
using SylviaNG.Recruitment.Application.Features.TargetLetters.Models;

namespace SylviaNG.Recruitment.Application.Features.TargetLetters.Queries.TargetLetterGetAll
{
    public class TargetLetterGetAllQuery : IRequest<List<TargetLetterResponse>>
    {
        public long? JobApplicationId { get; set; }

        public TargetLetterGetAllQuery(long? jobApplicationId)
        {
            JobApplicationId = jobApplicationId;
        }
    }
}

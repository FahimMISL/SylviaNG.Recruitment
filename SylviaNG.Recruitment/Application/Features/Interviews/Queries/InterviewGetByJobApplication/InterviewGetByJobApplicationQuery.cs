using MediatR;
using SylviaNG.Recruitment.Application.Features.Interviews.Models;

namespace SylviaNG.Recruitment.Application.Features.Interviews.Queries.InterviewGetByJobApplication
{
    public class InterviewGetByJobApplicationQuery : IRequest<List<InterviewResponse>>
    {
        public long JobApplicationId { get; set; }

        public InterviewGetByJobApplicationQuery(long jobApplicationId)
        {
            JobApplicationId = jobApplicationId;
        }
    }
}

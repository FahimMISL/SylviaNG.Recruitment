using MediatR;
using SylviaNG.Recruitment.Application.Features.InterviewEvaluations.Models;

namespace SylviaNG.Recruitment.Application.Features.InterviewEvaluations.Queries.InterviewEvaluationGetByInterview
{
    public class InterviewEvaluationGetByInterviewQuery : IRequest<List<InterviewEvaluationResponse>>
    {
        public long InterviewId { get; set; }

        public InterviewEvaluationGetByInterviewQuery(long interviewId)
        {
            InterviewId = interviewId;
        }
    }
}

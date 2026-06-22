using MediatR;
using SylviaNG.Recruitment.Application.Features.InterviewEvaluations.Models;

namespace SylviaNG.Recruitment.Application.Features.InterviewEvaluations.Queries.InterviewEvaluationGetById
{
    public class InterviewEvaluationGetByIdQuery : IRequest<InterviewEvaluationResponse>
    {
        public long InterviewEvaluationId { get; set; }

        public InterviewEvaluationGetByIdQuery(long interviewEvaluationId)
        {
            InterviewEvaluationId = interviewEvaluationId;
        }
    }
}

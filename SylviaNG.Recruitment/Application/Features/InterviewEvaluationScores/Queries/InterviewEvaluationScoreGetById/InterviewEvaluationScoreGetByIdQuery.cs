using MediatR;
using SylviaNG.Recruitment.Application.Features.InterviewEvaluationScores.Models;

namespace SylviaNG.Recruitment.Application.Features.InterviewEvaluationScores.Queries.InterviewEvaluationScoreGetById
{
    public class InterviewEvaluationScoreGetByIdQuery : IRequest<InterviewEvaluationScoreResponse>
    {
        public long InterviewEvaluationScoreId { get; set; }

        public InterviewEvaluationScoreGetByIdQuery(long interviewEvaluationScoreId)
        {
            InterviewEvaluationScoreId = interviewEvaluationScoreId;
        }
    }
}

using MediatR;
using SylviaNG.Recruitment.Application.Features.InterviewEvaluationScores.Models;

namespace SylviaNG.Recruitment.Application.Features.InterviewEvaluationScores.Commands.InterviewEvaluationScoreUpdate
{
    public class InterviewEvaluationScoreUpdateCommand : IRequest<Unit>
    {
        public long InterviewEvaluationScoreId { get; set; }
        public InterviewEvaluationScoreUpdateRequest Request { get; set; }

        public InterviewEvaluationScoreUpdateCommand(long interviewEvaluationScoreId, InterviewEvaluationScoreUpdateRequest request)
        {
            InterviewEvaluationScoreId = interviewEvaluationScoreId;
            Request = request;
        }
    }
}

using MediatR;

namespace SylviaNG.Recruitment.Application.Features.InterviewEvaluationScores.Commands.InterviewEvaluationScoreDelete
{
    public class InterviewEvaluationScoreDeleteCommand : IRequest<Unit>
    {
        public long InterviewEvaluationScoreId { get; set; }

        public InterviewEvaluationScoreDeleteCommand(long interviewEvaluationScoreId)
        {
            InterviewEvaluationScoreId = interviewEvaluationScoreId;
        }
    }
}

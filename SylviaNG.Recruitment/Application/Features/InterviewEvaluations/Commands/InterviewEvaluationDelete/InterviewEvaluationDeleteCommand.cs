using MediatR;

namespace SylviaNG.Recruitment.Application.Features.InterviewEvaluations.Commands.InterviewEvaluationDelete
{
    public class InterviewEvaluationDeleteCommand : IRequest<Unit>
    {
        public long InterviewEvaluationId { get; set; }

        public InterviewEvaluationDeleteCommand(long interviewEvaluationId)
        {
            InterviewEvaluationId = interviewEvaluationId;
        }
    }
}

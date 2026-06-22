using MediatR;
using SylviaNG.Recruitment.Application.Features.InterviewEvaluationScores.Models;

namespace SylviaNG.Recruitment.Application.Features.InterviewEvaluationScores.Commands.InterviewEvaluationScoreCreate
{
    public class InterviewEvaluationScoreCreateCommand : IRequest<long>
    {
        public InterviewEvaluationScoreCreateRequest Request { get; set; }

        public InterviewEvaluationScoreCreateCommand(InterviewEvaluationScoreCreateRequest request)
        {
            Request = request;
        }
    }
}

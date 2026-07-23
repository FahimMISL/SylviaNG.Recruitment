using MediatR;
using SylviaNG.Recruitment.Application.Features.InterviewEvaluations.Models;

namespace SylviaNG.Recruitment.Application.Features.InterviewEvaluations.Commands.InterviewEvaluationUpdate
{
    public class InterviewEvaluationUpdateCommand : IRequest
    {
        public long InterviewEvaluationId { get; set; }
        public InterviewEvaluationUpdateRequest Request { get; set; }

        public InterviewEvaluationUpdateCommand(long interviewEvaluationId, InterviewEvaluationUpdateRequest request)
        {
            InterviewEvaluationId = interviewEvaluationId;
            Request = request;
        }
    }
}

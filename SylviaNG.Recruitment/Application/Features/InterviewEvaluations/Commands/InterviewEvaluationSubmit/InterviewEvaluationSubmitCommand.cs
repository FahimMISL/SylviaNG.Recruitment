using MediatR;
using SylviaNG.Recruitment.Application.Features.InterviewEvaluations.Models;

namespace SylviaNG.Recruitment.Application.Features.InterviewEvaluations.Commands.InterviewEvaluationSubmit
{
    public class InterviewEvaluationSubmitCommand : IRequest<long>
    {
        public long InterviewId { get; set; }
        public InterviewEvaluationSubmitRequest Request { get; set; }

        public InterviewEvaluationSubmitCommand(long interviewId, InterviewEvaluationSubmitRequest request)
        {
            InterviewId = interviewId;
            Request = request;
        }
    }
}

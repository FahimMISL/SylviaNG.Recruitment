using MediatR;
using SylviaNG.Recruitment.Application.Features.InterviewEvaluations.Models;

namespace SylviaNG.Recruitment.Application.Features.InterviewEvaluations.Commands.InterviewEvaluationCreate
{
    public class InterviewEvaluationCreateCommand : IRequest<long>
    {
        public InterviewEvaluationCreateRequest Request { get; set; }

        public InterviewEvaluationCreateCommand(InterviewEvaluationCreateRequest request)
        {
            Request = request;
        }
    }
}

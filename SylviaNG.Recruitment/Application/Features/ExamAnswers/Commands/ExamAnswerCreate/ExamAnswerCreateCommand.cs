using MediatR;
using SylviaNG.Recruitment.Application.Features.ExamAnswers.Models;

namespace SylviaNG.Recruitment.Application.Features.ExamAnswers.Commands.ExamAnswerCreate
{
    public class ExamAnswerCreateCommand : IRequest<long>
    {
        public ExamAnswerCreateRequest Request { get; set; }

        public ExamAnswerCreateCommand(ExamAnswerCreateRequest request)
        {
            Request = request;
        }
    }
}

using MediatR;
using SylviaNG.Recruitment.Application.Features.ExamQuestions.Models;

namespace SylviaNG.Recruitment.Application.Features.ExamQuestions.Commands.ExamQuestionCreate
{
    public class ExamQuestionCreateCommand : IRequest<long>
    {
        public ExamQuestionCreateRequest Request { get; set; }

        public ExamQuestionCreateCommand(ExamQuestionCreateRequest request)
        {
            Request = request;
        }
    }
}

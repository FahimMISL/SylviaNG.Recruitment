using MediatR;
using SylviaNG.Recruitment.Application.Features.ExamQuestions.Models;

namespace SylviaNG.Recruitment.Application.Features.ExamQuestions.Commands.ExamQuestionUpdate
{
    public class ExamQuestionUpdateCommand : IRequest<Unit>
    {
        public long ExamQuestionId { get; set; }
        public ExamQuestionUpdateRequest Request { get; set; }

        public ExamQuestionUpdateCommand(long examQuestionId, ExamQuestionUpdateRequest request)
        {
            ExamQuestionId = examQuestionId;
            Request = request;
        }
    }
}

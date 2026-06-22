using MediatR;
using SylviaNG.Recruitment.Application.Features.ExamAnswers.Models;

namespace SylviaNG.Recruitment.Application.Features.ExamAnswers.Commands.ExamAnswerUpdate
{
    public class ExamAnswerUpdateCommand : IRequest<Unit>
    {
        public long ExamAnswerId { get; set; }
        public ExamAnswerUpdateRequest Request { get; set; }

        public ExamAnswerUpdateCommand(long examAnswerId, ExamAnswerUpdateRequest request)
        {
            ExamAnswerId = examAnswerId;
            Request = request;
        }
    }
}

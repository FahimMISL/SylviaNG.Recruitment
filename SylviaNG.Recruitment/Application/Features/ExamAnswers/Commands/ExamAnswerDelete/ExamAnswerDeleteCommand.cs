using MediatR;

namespace SylviaNG.Recruitment.Application.Features.ExamAnswers.Commands.ExamAnswerDelete
{
    public class ExamAnswerDeleteCommand : IRequest<Unit>
    {
        public long ExamAnswerId { get; set; }

        public ExamAnswerDeleteCommand(long examAnswerId)
        {
            ExamAnswerId = examAnswerId;
        }
    }
}

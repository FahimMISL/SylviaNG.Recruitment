using MediatR;

namespace SylviaNG.Recruitment.Application.Features.Exams.Commands.ExamDelete
{
    public class ExamDeleteCommand : IRequest<Unit>
    {
        public long ExamId { get; set; }

        public ExamDeleteCommand(long examId)
        {
            ExamId = examId;
        }
    }
}

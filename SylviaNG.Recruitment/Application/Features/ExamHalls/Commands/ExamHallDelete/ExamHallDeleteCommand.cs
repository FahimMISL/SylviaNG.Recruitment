using MediatR;

namespace SylviaNG.Recruitment.Application.Features.ExamHalls.Commands.ExamHallDelete
{
    public class ExamHallDeleteCommand : IRequest<Unit>
    {
        public long ExamHallId { get; set; }

        public ExamHallDeleteCommand(long examHallId)
        {
            ExamHallId = examHallId;
        }
    }
}

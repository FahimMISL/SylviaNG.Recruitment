using MediatR;

namespace SylviaNG.Recruitment.Application.Features.ExamHalls.Commands.ExamHallSetActiveStatus
{
    public class ExamHallSetActiveStatusCommand : IRequest<Unit>
    {
        public long ExamHallId { get; set; }
        public bool IsActive { get; set; }

        public ExamHallSetActiveStatusCommand(long examHallId, bool isActive)
        {
            ExamHallId = examHallId;
            IsActive = isActive;
        }
    }
}

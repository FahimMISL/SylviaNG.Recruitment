using MediatR;

namespace SylviaNG.Recruitment.Application.Features.ExamQuestions.Commands.ExamQuestionSetActiveStatus
{
    public class ExamQuestionSetActiveStatusCommand : IRequest<Unit>
    {
        public long ExamQuestionId { get; set; }
        public bool IsActive { get; set; }

        public ExamQuestionSetActiveStatusCommand(long examQuestionId, bool isActive)
        {
            ExamQuestionId = examQuestionId;
            IsActive = isActive;
        }
    }
}

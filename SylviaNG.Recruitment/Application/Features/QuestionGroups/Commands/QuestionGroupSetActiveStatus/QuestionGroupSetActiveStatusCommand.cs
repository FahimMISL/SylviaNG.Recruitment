using MediatR;

namespace SylviaNG.Recruitment.Application.Features.QuestionGroups.Commands.QuestionGroupSetActiveStatus
{
    public class QuestionGroupSetActiveStatusCommand : IRequest<Unit>
    {
        public long QuestionGroupId { get; set; }
        public bool IsActive { get; set; }

        public QuestionGroupSetActiveStatusCommand(long questionGroupId, bool isActive)
        {
            QuestionGroupId = questionGroupId;
            IsActive = isActive;
        }
    }
}

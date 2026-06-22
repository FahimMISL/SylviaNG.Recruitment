using MediatR;

namespace SylviaNG.Recruitment.Application.Features.QuestionGroups.Commands.QuestionGroupDelete
{
    public class QuestionGroupDeleteCommand : IRequest<Unit>
    {
        public long QuestionGroupId { get; set; }

        public QuestionGroupDeleteCommand(long questionGroupId)
        {
            QuestionGroupId = questionGroupId;
        }
    }
}

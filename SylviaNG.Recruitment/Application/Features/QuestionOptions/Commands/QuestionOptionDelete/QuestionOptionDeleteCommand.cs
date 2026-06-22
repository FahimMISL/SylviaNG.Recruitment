using MediatR;

namespace SylviaNG.Recruitment.Application.Features.QuestionOptions.Commands.QuestionOptionDelete
{
    public class QuestionOptionDeleteCommand : IRequest<Unit>
    {
        public long QuestionOptionId { get; set; }

        public QuestionOptionDeleteCommand(long questionOptionId)
        {
            QuestionOptionId = questionOptionId;
        }
    }
}

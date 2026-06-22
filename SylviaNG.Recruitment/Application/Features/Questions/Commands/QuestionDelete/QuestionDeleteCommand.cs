using MediatR;

namespace SylviaNG.Recruitment.Application.Features.Questions.Commands.QuestionDelete
{
    public class QuestionDeleteCommand : IRequest<Unit>
    {
        public long QuestionId { get; set; }

        public QuestionDeleteCommand(long questionId)
        {
            QuestionId = questionId;
        }
    }
}

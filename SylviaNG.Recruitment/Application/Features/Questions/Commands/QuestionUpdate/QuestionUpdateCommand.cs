using MediatR;
using SylviaNG.Recruitment.Application.Features.Questions.Models;

namespace SylviaNG.Recruitment.Application.Features.Questions.Commands.QuestionUpdate
{
    public class QuestionUpdateCommand : IRequest<Unit>
    {
        public long QuestionId { get; set; }
        public QuestionUpdateRequest Request { get; set; }

        public QuestionUpdateCommand(long questionId, QuestionUpdateRequest request)
        {
            QuestionId = questionId;
            Request = request;
        }
    }
}

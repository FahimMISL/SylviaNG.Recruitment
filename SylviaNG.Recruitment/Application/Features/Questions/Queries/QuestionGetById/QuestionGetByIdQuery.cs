using MediatR;
using SylviaNG.Recruitment.Application.Features.Questions.Models;

namespace SylviaNG.Recruitment.Application.Features.Questions.Queries.QuestionGetById
{
    public class QuestionGetByIdQuery : IRequest<QuestionResponse>
    {
        public long QuestionId { get; set; }

        public QuestionGetByIdQuery(long questionId)
        {
            QuestionId = questionId;
        }
    }
}

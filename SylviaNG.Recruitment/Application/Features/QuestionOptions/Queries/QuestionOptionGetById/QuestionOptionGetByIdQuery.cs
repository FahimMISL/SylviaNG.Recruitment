using MediatR;
using SylviaNG.Recruitment.Application.Features.QuestionOptions.Models;

namespace SylviaNG.Recruitment.Application.Features.QuestionOptions.Queries.QuestionOptionGetById
{
    public class QuestionOptionGetByIdQuery : IRequest<QuestionOptionResponse>
    {
        public long QuestionOptionId { get; set; }

        public QuestionOptionGetByIdQuery(long questionOptionId)
        {
            QuestionOptionId = questionOptionId;
        }
    }
}

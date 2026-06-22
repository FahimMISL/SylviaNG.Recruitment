using MediatR;
using SylviaNG.Recruitment.Application.Features.Questions.Models;

namespace SylviaNG.Recruitment.Application.Features.Questions.Queries.QuestionGetAll
{
    public class QuestionGetAllQuery : IRequest<List<QuestionResponse>>
    {
    }
}

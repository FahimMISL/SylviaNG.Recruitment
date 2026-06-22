using MediatR;
using SylviaNG.Recruitment.Application.Features.QuestionOptions.Models;

namespace SylviaNG.Recruitment.Application.Features.QuestionOptions.Queries.QuestionOptionGetAll
{
    public class QuestionOptionGetAllQuery : IRequest<List<QuestionOptionResponse>>
    {
    }
}

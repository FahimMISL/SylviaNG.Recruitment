using MediatR;
using SylviaNG.Recruitment.Application.Features.QuestionGroups.Models;

namespace SylviaNG.Recruitment.Application.Features.QuestionGroups.Queries.QuestionGroupGetAll
{
    public class QuestionGroupGetAllQuery : IRequest<List<QuestionGroupResponse>>
    {
    }
}

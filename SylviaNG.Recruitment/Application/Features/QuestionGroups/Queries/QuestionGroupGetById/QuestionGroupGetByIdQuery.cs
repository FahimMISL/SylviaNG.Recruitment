using MediatR;
using SylviaNG.Recruitment.Application.Features.QuestionGroups.Models;

namespace SylviaNG.Recruitment.Application.Features.QuestionGroups.Queries.QuestionGroupGetById
{
    public class QuestionGroupGetByIdQuery : IRequest<QuestionGroupResponse>
    {
        public long QuestionGroupId { get; set; }

        public QuestionGroupGetByIdQuery(long questionGroupId)
        {
            QuestionGroupId = questionGroupId;
        }
    }
}

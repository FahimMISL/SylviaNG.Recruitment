using MediatR;
using SylviaNG.Recruitment.Application.Features.QuestionGroups.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.QuestionGroups.Queries.QuestionGroupGetAll
{
    public class QuestionGroupGetAllHandler : IRequestHandler<QuestionGroupGetAllQuery, List<QuestionGroupResponse>>
    {
        private readonly IQuestionGroupService _service;

        public QuestionGroupGetAllHandler(IQuestionGroupService service)
        {
            _service = service;
        }

        public async Task<List<QuestionGroupResponse>> Handle(QuestionGroupGetAllQuery query, CancellationToken cancellationToken)
        {
            return await _service.GetAllAsync();
        }
    }
}

using MediatR;
using SylviaNG.Recruitment.Application.Features.QuestionGroups.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.QuestionGroups.Queries.QuestionGroupGetById
{
    public class QuestionGroupGetByIdHandler : IRequestHandler<QuestionGroupGetByIdQuery, QuestionGroupResponse>
    {
        private readonly IQuestionGroupService _service;

        public QuestionGroupGetByIdHandler(IQuestionGroupService service)
        {
            _service = service;
        }

        public async Task<QuestionGroupResponse> Handle(QuestionGroupGetByIdQuery query, CancellationToken cancellationToken)
        {
            return await _service.GetByIdAsync(query.QuestionGroupId);
        }
    }
}

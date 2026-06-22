using MediatR;
using SylviaNG.Recruitment.Application.Features.QuestionGroups.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.SharedKernel.Pagination;

namespace SylviaNG.Recruitment.Application.Features.QuestionGroups.Queries.QuestionGroupGetAllPaged
{
    public class QuestionGroupGetAllPagedHandler : IRequestHandler<QuestionGroupGetAllPagedQuery, PagedResult<QuestionGroupResponse>>
    {
        private readonly IQuestionGroupService _questionGroupService;

        public QuestionGroupGetAllPagedHandler(IQuestionGroupService questionGroupService)
        {
            _questionGroupService = questionGroupService;
        }

        public async Task<PagedResult<QuestionGroupResponse>> Handle(QuestionGroupGetAllPagedQuery query, CancellationToken cancellationToken)
        {
            return await _questionGroupService.GetPaginatedAsync(query.Request);
        }
    }
}

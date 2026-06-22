using MediatR;
using SylviaNG.Recruitment.Application.Features.QuestionOptions.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.SharedKernel.Pagination;

namespace SylviaNG.Recruitment.Application.Features.QuestionOptions.Queries.QuestionOptionGetAllPaged
{
    public class QuestionOptionGetAllPagedHandler : IRequestHandler<QuestionOptionGetAllPagedQuery, PagedResult<QuestionOptionResponse>>
    {
        private readonly IQuestionOptionService _questionOptionService;

        public QuestionOptionGetAllPagedHandler(IQuestionOptionService questionOptionService)
        {
            _questionOptionService = questionOptionService;
        }

        public async Task<PagedResult<QuestionOptionResponse>> Handle(QuestionOptionGetAllPagedQuery query, CancellationToken cancellationToken)
        {
            return await _questionOptionService.GetPaginatedAsync(query.Request);
        }
    }
}

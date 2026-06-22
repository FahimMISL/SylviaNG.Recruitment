using MediatR;
using SylviaNG.Recruitment.Application.Features.Questions.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.SharedKernel.Pagination;

namespace SylviaNG.Recruitment.Application.Features.Questions.Queries.QuestionGetAllPaged
{
    public class QuestionGetAllPagedHandler : IRequestHandler<QuestionGetAllPagedQuery, PagedResult<QuestionResponse>>
    {
        private readonly IQuestionService _questionService;

        public QuestionGetAllPagedHandler(IQuestionService questionService)
        {
            _questionService = questionService;
        }

        public async Task<PagedResult<QuestionResponse>> Handle(QuestionGetAllPagedQuery query, CancellationToken cancellationToken)
        {
            return await _questionService.GetPaginatedAsync(query.Request);
        }
    }
}

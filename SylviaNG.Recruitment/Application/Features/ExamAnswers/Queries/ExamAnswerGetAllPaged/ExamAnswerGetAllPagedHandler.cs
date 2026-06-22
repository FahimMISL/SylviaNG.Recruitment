using MediatR;
using SylviaNG.Recruitment.Application.Features.ExamAnswers.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.SharedKernel.Pagination;

namespace SylviaNG.Recruitment.Application.Features.ExamAnswers.Queries.ExamAnswerGetAllPaged
{
    public class ExamAnswerGetAllPagedHandler : IRequestHandler<ExamAnswerGetAllPagedQuery, PagedResult<ExamAnswerResponse>>
    {
        private readonly IExamAnswerService _examAnswerService;

        public ExamAnswerGetAllPagedHandler(IExamAnswerService examAnswerService)
        {
            _examAnswerService = examAnswerService;
        }

        public async Task<PagedResult<ExamAnswerResponse>> Handle(ExamAnswerGetAllPagedQuery query, CancellationToken cancellationToken)
        {
            return await _examAnswerService.GetPaginatedAsync(query.Request);
        }
    }
}

using MediatR;
using SylviaNG.Recruitment.Application.Features.ExamQuestions.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.SharedKernel.Pagination;

namespace SylviaNG.Recruitment.Application.Features.ExamQuestions.Queries.ExamQuestionGetAllPaged
{
    public class ExamQuestionGetAllPagedHandler : IRequestHandler<ExamQuestionGetAllPagedQuery, PagedResult<ExamQuestionResponse>>
    {
        private readonly IExamQuestionService _examQuestionService;

        public ExamQuestionGetAllPagedHandler(IExamQuestionService examQuestionService)
        {
            _examQuestionService = examQuestionService;
        }

        public async Task<PagedResult<ExamQuestionResponse>> Handle(ExamQuestionGetAllPagedQuery query, CancellationToken cancellationToken)
        {
            return await _examQuestionService.GetPaginatedAsync(query.Request, query.QuestionGroupId, query.QuestionType, query.DifficultyLevel, query.IsActive);
        }
    }
}

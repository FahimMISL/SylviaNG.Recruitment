using MediatR;
using SylviaNG.Recruitment.Application.Features.Exams.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.SharedKernel.Pagination;

namespace SylviaNG.Recruitment.Application.Features.Exams.Queries.ExamGetAllPaged
{
    public class ExamGetAllPagedHandler : IRequestHandler<ExamGetAllPagedQuery, PagedResult<ExamResponse>>
    {
        private readonly IExamService _examService;

        public ExamGetAllPagedHandler(IExamService examService)
        {
            _examService = examService;
        }

        public async Task<PagedResult<ExamResponse>> Handle(ExamGetAllPagedQuery query, CancellationToken cancellationToken)
        {
            return await _examService.GetPagedAsync(query.Request, query.JobPostingId, query.ExamType, query.IsActive);
        }
    }
}

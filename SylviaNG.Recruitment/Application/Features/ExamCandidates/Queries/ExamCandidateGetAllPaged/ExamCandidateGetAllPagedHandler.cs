using MediatR;
using SylviaNG.Recruitment.Application.Features.ExamCandidates.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.SharedKernel.Pagination;

namespace SylviaNG.Recruitment.Application.Features.ExamCandidates.Queries.ExamCandidateGetAllPaged
{
    public class ExamCandidateGetAllPagedHandler : IRequestHandler<ExamCandidateGetAllPagedQuery, PagedResult<ExamCandidateResponse>>
    {
        private readonly IExamCandidateService _examCandidateService;

        public ExamCandidateGetAllPagedHandler(IExamCandidateService examCandidateService)
        {
            _examCandidateService = examCandidateService;
        }

        public async Task<PagedResult<ExamCandidateResponse>> Handle(ExamCandidateGetAllPagedQuery query, CancellationToken cancellationToken)
        {
            return await _examCandidateService.GetPaginatedAsync(query.Request);
        }
    }
}

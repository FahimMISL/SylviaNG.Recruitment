using MediatR;
using SylviaNG.Recruitment.Application.Features.ExamHalls.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.SharedKernel.Pagination;

namespace SylviaNG.Recruitment.Application.Features.ExamHalls.Queries.ExamHallGetAllPaged
{
    public class ExamHallGetAllPagedHandler : IRequestHandler<ExamHallGetAllPagedQuery, PagedResult<ExamHallResponse>>
    {
        private readonly IExamHallService _examHallService;

        public ExamHallGetAllPagedHandler(IExamHallService examHallService)
        {
            _examHallService = examHallService;
        }

        public async Task<PagedResult<ExamHallResponse>> Handle(ExamHallGetAllPagedQuery query, CancellationToken cancellationToken)
        {
            return await _examHallService.GetPaginatedAsync(query.Request);
        }
    }
}

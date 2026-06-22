using MediatR;
using SylviaNG.Recruitment.Application.Features.ExamSeatPlans.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.SharedKernel.Pagination;

namespace SylviaNG.Recruitment.Application.Features.ExamSeatPlans.Queries.ExamSeatPlanGetAllPaged
{
    public class ExamSeatPlanGetAllPagedHandler : IRequestHandler<ExamSeatPlanGetAllPagedQuery, PagedResult<ExamSeatPlanResponse>>
    {
        private readonly IExamSeatPlanService _examSeatPlanService;

        public ExamSeatPlanGetAllPagedHandler(IExamSeatPlanService examSeatPlanService)
        {
            _examSeatPlanService = examSeatPlanService;
        }

        public async Task<PagedResult<ExamSeatPlanResponse>> Handle(ExamSeatPlanGetAllPagedQuery query, CancellationToken cancellationToken)
        {
            return await _examSeatPlanService.GetPaginatedAsync(query.Request);
        }
    }
}

using MediatR;
using SylviaNG.Recruitment.Application.Features.ExamSeatPlans.Models;
using SylviaNG.Recruitment.SharedKernel.Pagination;

namespace SylviaNG.Recruitment.Application.Features.ExamSeatPlans.Queries.ExamSeatPlanGetAllPaged
{
    public class ExamSeatPlanGetAllPagedQuery : IRequest<PagedResult<ExamSeatPlanResponse>>
    {
        public PagedRequest Request { get; set; }

        public ExamSeatPlanGetAllPagedQuery(PagedRequest request)
        {
            Request = request;
        }
    }
}

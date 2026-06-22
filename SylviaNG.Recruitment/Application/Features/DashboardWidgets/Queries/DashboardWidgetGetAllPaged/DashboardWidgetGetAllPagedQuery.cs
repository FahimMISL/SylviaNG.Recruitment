using MediatR;
using SylviaNG.Recruitment.Application.Features.DashboardWidgets.Models;
using SylviaNG.Recruitment.SharedKernel.Pagination;

namespace SylviaNG.Recruitment.Application.Features.DashboardWidgets.Queries.DashboardWidgetGetAllPaged
{
    public class DashboardWidgetGetAllPagedQuery : IRequest<PagedResult<DashboardWidgetResponse>>
    {
        public PagedRequest Request { get; set; }

        public DashboardWidgetGetAllPagedQuery(PagedRequest request)
        {
            Request = request;
        }
    }
}

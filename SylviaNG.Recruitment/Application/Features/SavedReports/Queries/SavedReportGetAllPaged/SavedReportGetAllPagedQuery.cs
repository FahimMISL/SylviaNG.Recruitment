using MediatR;
using SylviaNG.Recruitment.Application.Features.SavedReports.Models;
using SylviaNG.Recruitment.SharedKernel.Pagination;

namespace SylviaNG.Recruitment.Application.Features.SavedReports.Queries.SavedReportGetAllPaged
{
    public class SavedReportGetAllPagedQuery : IRequest<PagedResult<SavedReportResponse>>
    {
        public PagedRequest Request { get; set; }

        public SavedReportGetAllPagedQuery(PagedRequest request)
        {
            Request = request;
        }
    }
}

using MediatR;
using SylviaNG.Recruitment.Application.Features.ExportRequests.Models;
using SylviaNG.Recruitment.SharedKernel.Pagination;

namespace SylviaNG.Recruitment.Application.Features.ExportRequests.Queries.ExportRequestGetAllPaged
{
    public class ExportRequestGetAllPagedQuery : IRequest<PagedResult<ExportRequestResponse>>
    {
        public PagedRequest Request { get; set; }

        public ExportRequestGetAllPagedQuery(PagedRequest request)
        {
            Request = request;
        }
    }
}

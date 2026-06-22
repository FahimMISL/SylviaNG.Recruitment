using MediatR;
using SylviaNG.Recruitment.Application.Features.Requisitions.Models;
using SylviaNG.Recruitment.SharedKernel.Pagination;

namespace SylviaNG.Recruitment.Application.Features.Requisitions.Queries.RequisitionGetAllPaged
{
    public class RequisitionGetAllPagedQuery : IRequest<PagedResult<RequisitionResponse>>
    {
        public PagedRequest Request { get; set; }

        public RequisitionGetAllPagedQuery(PagedRequest request)
        {
            Request = request;
        }
    }
}

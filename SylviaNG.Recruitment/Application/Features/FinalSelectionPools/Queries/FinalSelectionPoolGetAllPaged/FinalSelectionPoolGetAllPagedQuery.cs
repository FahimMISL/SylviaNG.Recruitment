using MediatR;
using SylviaNG.Recruitment.Application.Features.FinalSelectionPools.Models;
using SylviaNG.Recruitment.SharedKernel.Pagination;

namespace SylviaNG.Recruitment.Application.Features.FinalSelectionPools.Queries.FinalSelectionPoolGetAllPaged
{
    public class FinalSelectionPoolGetAllPagedQuery : IRequest<PagedResult<FinalSelectionPoolResponse>>
    {
        public PagedRequest Request { get; set; }

        public FinalSelectionPoolGetAllPagedQuery(PagedRequest request)
        {
            Request = request;
        }
    }
}

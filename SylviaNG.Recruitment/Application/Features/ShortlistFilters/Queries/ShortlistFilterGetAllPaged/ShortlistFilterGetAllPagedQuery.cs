using MediatR;
using SylviaNG.Recruitment.Application.Features.ShortlistFilters.Models;
using SylviaNG.Recruitment.SharedKernel.Pagination;

namespace SylviaNG.Recruitment.Application.Features.ShortlistFilters.Queries.ShortlistFilterGetAllPaged
{
    public class ShortlistFilterGetAllPagedQuery : IRequest<PagedResult<ShortlistFilterResponse>>
    {
        public PagedRequest Request { get; set; }

        public ShortlistFilterGetAllPagedQuery(PagedRequest request)
        {
            Request = request;
        }
    }
}

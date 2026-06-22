using MediatR;
using SylviaNG.Recruitment.Application.Features.ShortlistFilterCriterias.Models;
using SylviaNG.Recruitment.SharedKernel.Pagination;

namespace SylviaNG.Recruitment.Application.Features.ShortlistFilterCriterias.Queries.ShortlistFilterCriteriaGetAllPaged
{
    public class ShortlistFilterCriteriaGetAllPagedQuery : IRequest<PagedResult<ShortlistFilterCriteriaResponse>>
    {
        public PagedRequest Request { get; set; }

        public ShortlistFilterCriteriaGetAllPagedQuery(PagedRequest request)
        {
            Request = request;
        }
    }
}

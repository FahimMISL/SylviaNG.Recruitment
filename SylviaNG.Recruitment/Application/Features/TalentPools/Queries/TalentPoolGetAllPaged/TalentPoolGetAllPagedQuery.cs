using MediatR;
using SylviaNG.Recruitment.Application.Features.TalentPools.Models;
using SylviaNG.Recruitment.SharedKernel.Pagination;

namespace SylviaNG.Recruitment.Application.Features.TalentPools.Queries.TalentPoolGetAllPaged
{
    public class TalentPoolGetAllPagedQuery : IRequest<PagedResult<TalentPoolResponse>>
    {
        public PagedRequest Request { get; set; }

        public TalentPoolGetAllPagedQuery(PagedRequest request)
        {
            Request = request;
        }
    }
}

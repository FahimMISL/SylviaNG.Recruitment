using MediatR;
using SylviaNG.Recruitment.Application.Features.Referrals.Models;
using SylviaNG.Recruitment.SharedKernel.Pagination;

namespace SylviaNG.Recruitment.Application.Features.Referrals.Queries.ReferralGetAllPaged
{
    public class ReferralGetAllPagedQuery : IRequest<PagedResult<ReferralResponse>>
    {
        public PagedRequest Request { get; set; }

        public ReferralGetAllPagedQuery(PagedRequest request)
        {
            Request = request;
        }
    }
}

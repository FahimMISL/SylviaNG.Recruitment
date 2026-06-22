using MediatR;
using SylviaNG.Recruitment.Application.Features.Referrals.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.SharedKernel.Pagination;

namespace SylviaNG.Recruitment.Application.Features.Referrals.Queries.ReferralGetAllPaged
{
    public class ReferralGetAllPagedHandler : IRequestHandler<ReferralGetAllPagedQuery, PagedResult<ReferralResponse>>
    {
        private readonly IReferralService _referralService;

        public ReferralGetAllPagedHandler(IReferralService referralService)
        {
            _referralService = referralService;
        }

        public async Task<PagedResult<ReferralResponse>> Handle(ReferralGetAllPagedQuery query, CancellationToken cancellationToken)
        {
            return await _referralService.GetPaginatedAsync(query.Request);
        }
    }
}

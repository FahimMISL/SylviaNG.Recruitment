using MediatR;
using SylviaNG.Recruitment.Application.Features.ReferralDuplicates.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.SharedKernel.Pagination;

namespace SylviaNG.Recruitment.Application.Features.ReferralDuplicates.Queries.ReferralDuplicateGetAllPaged
{
    public class ReferralDuplicateGetAllPagedHandler : IRequestHandler<ReferralDuplicateGetAllPagedQuery, PagedResult<ReferralDuplicateResponse>>
    {
        private readonly IReferralDuplicateService _referralDuplicateService;

        public ReferralDuplicateGetAllPagedHandler(IReferralDuplicateService referralDuplicateService)
        {
            _referralDuplicateService = referralDuplicateService;
        }

        public async Task<PagedResult<ReferralDuplicateResponse>> Handle(ReferralDuplicateGetAllPagedQuery query, CancellationToken cancellationToken)
        {
            return await _referralDuplicateService.GetPaginatedAsync(query.Request);
        }
    }
}

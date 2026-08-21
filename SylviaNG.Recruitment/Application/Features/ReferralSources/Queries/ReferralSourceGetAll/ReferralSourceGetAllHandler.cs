using MediatR;
using SylviaNG.Recruitment.Application.Features.ReferralSources.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.ReferralSources.Queries.ReferralSourceGetAll
{
    public class ReferralSourceGetAllHandler : IRequestHandler<ReferralSourceGetAllQuery, List<ReferralSourceResponse>>
    {
        private readonly IReferralSourceService _referralSourceService;

        public ReferralSourceGetAllHandler(IReferralSourceService referralSourceService)
        {
            _referralSourceService = referralSourceService;
        }

        public async Task<List<ReferralSourceResponse>> Handle(ReferralSourceGetAllQuery query, CancellationToken cancellationToken)
        {
            return await _referralSourceService.GetAllAsync();
        }
    }
}

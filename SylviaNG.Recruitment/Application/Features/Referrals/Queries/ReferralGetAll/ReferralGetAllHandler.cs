using MediatR;
using SylviaNG.Recruitment.Application.Features.Referrals.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.Referrals.Queries.ReferralGetAll
{
    public class ReferralGetAllHandler : IRequestHandler<ReferralGetAllQuery, List<ReferralResponse>>
    {
        private readonly IReferralService _service;

        public ReferralGetAllHandler(IReferralService service)
        {
            _service = service;
        }

        public async Task<List<ReferralResponse>> Handle(ReferralGetAllQuery query, CancellationToken cancellationToken)
        {
            return await _service.GetAllAsync();
        }
    }
}

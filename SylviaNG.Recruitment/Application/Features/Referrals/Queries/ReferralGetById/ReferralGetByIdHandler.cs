using MediatR;
using SylviaNG.Recruitment.Application.Features.Referrals.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.Referrals.Queries.ReferralGetById
{
    public class ReferralGetByIdHandler : IRequestHandler<ReferralGetByIdQuery, ReferralResponse>
    {
        private readonly IReferralService _service;

        public ReferralGetByIdHandler(IReferralService service)
        {
            _service = service;
        }

        public async Task<ReferralResponse> Handle(ReferralGetByIdQuery query, CancellationToken cancellationToken)
        {
            return await _service.GetByIdAsync(query.ReferralId);
        }
    }
}

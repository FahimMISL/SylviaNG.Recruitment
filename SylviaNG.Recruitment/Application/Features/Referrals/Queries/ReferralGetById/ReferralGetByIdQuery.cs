using MediatR;
using SylviaNG.Recruitment.Application.Features.Referrals.Models;

namespace SylviaNG.Recruitment.Application.Features.Referrals.Queries.ReferralGetById
{
    public class ReferralGetByIdQuery : IRequest<ReferralResponse>
    {
        public long ReferralId { get; set; }

        public ReferralGetByIdQuery(long referralId)
        {
            ReferralId = referralId;
        }
    }
}

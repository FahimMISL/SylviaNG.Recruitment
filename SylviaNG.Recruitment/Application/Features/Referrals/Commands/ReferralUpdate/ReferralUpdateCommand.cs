using MediatR;
using SylviaNG.Recruitment.Application.Features.Referrals.Models;

namespace SylviaNG.Recruitment.Application.Features.Referrals.Commands.ReferralUpdate
{
    public class ReferralUpdateCommand : IRequest<Unit>
    {
        public long ReferralId { get; set; }
        public ReferralUpdateRequest Request { get; set; }

        public ReferralUpdateCommand(long referralId, ReferralUpdateRequest request)
        {
            ReferralId = referralId;
            Request = request;
        }
    }
}

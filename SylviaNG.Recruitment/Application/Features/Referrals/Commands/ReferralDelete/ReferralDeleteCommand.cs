using MediatR;

namespace SylviaNG.Recruitment.Application.Features.Referrals.Commands.ReferralDelete
{
    public class ReferralDeleteCommand : IRequest<Unit>
    {
        public long ReferralId { get; set; }

        public ReferralDeleteCommand(long referralId)
        {
            ReferralId = referralId;
        }
    }
}

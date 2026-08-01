using MediatR;

namespace SylviaNG.Recruitment.Application.Features.ReferralSources.Commands.ReferralSourceDelete
{
    public class ReferralSourceDeleteCommand : IRequest<Unit>
    {
        public long ReferralSourceId { get; set; }

        public ReferralSourceDeleteCommand(long referralSourceId)
        {
            ReferralSourceId = referralSourceId;
        }
    }
}

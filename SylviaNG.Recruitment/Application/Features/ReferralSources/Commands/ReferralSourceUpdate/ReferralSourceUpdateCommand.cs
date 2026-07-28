using MediatR;
using SylviaNG.Recruitment.Application.Features.ReferralSources.Models;

namespace SylviaNG.Recruitment.Application.Features.ReferralSources.Commands.ReferralSourceUpdate
{
    public class ReferralSourceUpdateCommand : IRequest<Unit>
    {
        public long ReferralSourceId { get; set; }
        public ReferralSourceUpdateRequest Request { get; set; }

        public ReferralSourceUpdateCommand(long referralSourceId, ReferralSourceUpdateRequest request)
        {
            ReferralSourceId = referralSourceId;
            Request = request;
        }
    }
}

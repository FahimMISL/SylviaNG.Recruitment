using MediatR;
using SylviaNG.Recruitment.Application.Features.ReferralDuplicates.Models;

namespace SylviaNG.Recruitment.Application.Features.ReferralDuplicates.Commands.ReferralDuplicateUpdate
{
    public class ReferralDuplicateUpdateCommand : IRequest<Unit>
    {
        public long ReferralDuplicateId { get; set; }
        public ReferralDuplicateUpdateRequest Request { get; set; }

        public ReferralDuplicateUpdateCommand(long referralDuplicateId, ReferralDuplicateUpdateRequest request)
        {
            ReferralDuplicateId = referralDuplicateId;
            Request = request;
        }
    }
}

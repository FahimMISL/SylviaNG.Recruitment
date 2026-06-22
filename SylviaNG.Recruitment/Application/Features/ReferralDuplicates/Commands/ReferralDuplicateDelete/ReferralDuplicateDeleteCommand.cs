using MediatR;

namespace SylviaNG.Recruitment.Application.Features.ReferralDuplicates.Commands.ReferralDuplicateDelete
{
    public class ReferralDuplicateDeleteCommand : IRequest<Unit>
    {
        public long ReferralDuplicateId { get; set; }

        public ReferralDuplicateDeleteCommand(long referralDuplicateId)
        {
            ReferralDuplicateId = referralDuplicateId;
        }
    }
}

using MediatR;
using SylviaNG.Recruitment.Application.Features.ReferralDuplicates.Models;

namespace SylviaNG.Recruitment.Application.Features.ReferralDuplicates.Queries.ReferralDuplicateGetById
{
    public class ReferralDuplicateGetByIdQuery : IRequest<ReferralDuplicateResponse>
    {
        public long ReferralDuplicateId { get; set; }

        public ReferralDuplicateGetByIdQuery(long referralDuplicateId)
        {
            ReferralDuplicateId = referralDuplicateId;
        }
    }
}

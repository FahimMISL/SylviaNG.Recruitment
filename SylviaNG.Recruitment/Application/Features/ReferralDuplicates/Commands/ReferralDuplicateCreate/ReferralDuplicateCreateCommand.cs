using MediatR;
using SylviaNG.Recruitment.Application.Features.ReferralDuplicates.Models;

namespace SylviaNG.Recruitment.Application.Features.ReferralDuplicates.Commands.ReferralDuplicateCreate
{
    public class ReferralDuplicateCreateCommand : IRequest<long>
    {
        public ReferralDuplicateCreateRequest Request { get; set; }

        public ReferralDuplicateCreateCommand(ReferralDuplicateCreateRequest request)
        {
            Request = request;
        }
    }
}

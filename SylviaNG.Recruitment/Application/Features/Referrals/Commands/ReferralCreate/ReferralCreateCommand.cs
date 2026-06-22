using MediatR;
using SylviaNG.Recruitment.Application.Features.Referrals.Models;

namespace SylviaNG.Recruitment.Application.Features.Referrals.Commands.ReferralCreate
{
    public class ReferralCreateCommand : IRequest<long>
    {
        public ReferralCreateRequest Request { get; set; }

        public ReferralCreateCommand(ReferralCreateRequest request)
        {
            Request = request;
        }
    }
}

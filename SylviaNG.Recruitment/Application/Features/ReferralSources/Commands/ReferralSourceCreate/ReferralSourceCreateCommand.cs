using MediatR;
using SylviaNG.Recruitment.Application.Features.ReferralSources.Models;

namespace SylviaNG.Recruitment.Application.Features.ReferralSources.Commands.ReferralSourceCreate
{
    public class ReferralSourceCreateCommand : IRequest<long>
    {
        public ReferralSourceCreateRequest Request { get; set; }

        public ReferralSourceCreateCommand(ReferralSourceCreateRequest request)
        {
            Request = request;
        }
    }
}

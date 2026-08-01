using MediatR;
using SylviaNG.Recruitment.Application.Features.WaiverRules.Models;

namespace SylviaNG.Recruitment.Application.Features.WaiverRules.Commands.WaiverRuleCreate
{
    public class WaiverRuleCreateCommand : IRequest<long>
    {
        public WaiverRuleCreateRequest Request { get; set; }

        public WaiverRuleCreateCommand(WaiverRuleCreateRequest request)
        {
            Request = request;
        }
    }
}

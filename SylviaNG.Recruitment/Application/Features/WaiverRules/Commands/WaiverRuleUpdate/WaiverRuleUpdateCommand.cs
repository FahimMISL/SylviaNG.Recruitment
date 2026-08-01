using MediatR;
using SylviaNG.Recruitment.Application.Features.WaiverRules.Models;

namespace SylviaNG.Recruitment.Application.Features.WaiverRules.Commands.WaiverRuleUpdate
{
    public class WaiverRuleUpdateCommand : IRequest<Unit>
    {
        public long WaiverRuleId { get; set; }
        public WaiverRuleUpdateRequest Request { get; set; }

        public WaiverRuleUpdateCommand(long waiverRuleId, WaiverRuleUpdateRequest request)
        {
            WaiverRuleId = waiverRuleId;
            Request = request;
        }
    }
}

using MediatR;

namespace SylviaNG.Recruitment.Application.Features.WaiverRules.Commands.WaiverRuleDelete
{
    public class WaiverRuleDeleteCommand : IRequest<Unit>
    {
        public long WaiverRuleId { get; set; }

        public WaiverRuleDeleteCommand(long waiverRuleId)
        {
            WaiverRuleId = waiverRuleId;
        }
    }
}

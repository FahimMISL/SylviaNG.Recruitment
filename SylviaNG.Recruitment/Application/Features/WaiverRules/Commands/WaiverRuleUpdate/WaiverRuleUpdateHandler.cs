using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.WaiverRules.Commands.WaiverRuleUpdate
{
    public class WaiverRuleUpdateHandler : IRequestHandler<WaiverRuleUpdateCommand, Unit>
    {
        private readonly IWaiverRuleService _waiverRuleService;

        public WaiverRuleUpdateHandler(IWaiverRuleService waiverRuleService)
        {
            _waiverRuleService = waiverRuleService;
        }

        public async Task<Unit> Handle(WaiverRuleUpdateCommand command, CancellationToken cancellationToken)
        {
            await _waiverRuleService.UpdateAsync(command.WaiverRuleId, command.Request);
            return Unit.Value;
        }
    }
}

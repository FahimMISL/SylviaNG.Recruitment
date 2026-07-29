using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.WaiverRules.Commands.WaiverRuleDelete
{
    public class WaiverRuleDeleteHandler : IRequestHandler<WaiverRuleDeleteCommand, Unit>
    {
        private readonly IWaiverRuleService _waiverRuleService;

        public WaiverRuleDeleteHandler(IWaiverRuleService waiverRuleService)
        {
            _waiverRuleService = waiverRuleService;
        }

        public async Task<Unit> Handle(WaiverRuleDeleteCommand command, CancellationToken cancellationToken)
        {
            await _waiverRuleService.DeleteAsync(command.WaiverRuleId);
            return Unit.Value;
        }
    }
}

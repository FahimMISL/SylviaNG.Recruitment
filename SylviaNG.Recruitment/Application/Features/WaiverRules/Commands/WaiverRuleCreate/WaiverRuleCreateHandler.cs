using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.WaiverRules.Commands.WaiverRuleCreate
{
    public class WaiverRuleCreateHandler : IRequestHandler<WaiverRuleCreateCommand, long>
    {
        private readonly IWaiverRuleService _waiverRuleService;

        public WaiverRuleCreateHandler(IWaiverRuleService waiverRuleService)
        {
            _waiverRuleService = waiverRuleService;
        }

        public async Task<long> Handle(WaiverRuleCreateCommand command, CancellationToken cancellationToken)
        {
            return await _waiverRuleService.CreateAsync(command.Request);
        }
    }
}

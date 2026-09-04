using MediatR;
using SylviaNG.Recruitment.Application.Features.WaiverRules.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.WaiverRules.Queries.WaiverRuleGetAll
{
    public class WaiverRuleGetAllHandler : IRequestHandler<WaiverRuleGetAllQuery, List<WaiverRuleResponse>>
    {
        private readonly IWaiverRuleService _waiverRuleService;

        public WaiverRuleGetAllHandler(IWaiverRuleService waiverRuleService)
        {
            _waiverRuleService = waiverRuleService;
        }

        public async Task<List<WaiverRuleResponse>> Handle(WaiverRuleGetAllQuery query, CancellationToken cancellationToken)
        {
            return await _waiverRuleService.GetAllAsync();
        }
    }
}

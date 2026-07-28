using MediatR;
using SylviaNG.Recruitment.Application.Features.TalentPools.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.TalentPools.Queries.TalentPoolLookup
{
    public class TalentPoolLookupHandler : IRequestHandler<TalentPoolLookupQuery, List<TalentPoolLookupResponse>>
    {
        private readonly ITalentPoolService _talentPoolService;

        public TalentPoolLookupHandler(ITalentPoolService talentPoolService)
        {
            _talentPoolService = talentPoolService;
        }

        public async Task<List<TalentPoolLookupResponse>> Handle(TalentPoolLookupQuery query, CancellationToken cancellationToken)
        {
            return await _talentPoolService.GetLookupAsync();
        }
    }
}

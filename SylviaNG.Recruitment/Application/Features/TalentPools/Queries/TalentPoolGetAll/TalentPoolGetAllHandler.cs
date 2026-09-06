using MediatR;
using SylviaNG.Recruitment.Application.Features.TalentPools.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.TalentPools.Queries.TalentPoolGetAll
{
    public class TalentPoolGetAllHandler : IRequestHandler<TalentPoolGetAllQuery, List<TalentPoolResponse>>
    {
        private readonly ITalentPoolService _talentPoolService;

        public TalentPoolGetAllHandler(ITalentPoolService talentPoolService)
        {
            _talentPoolService = talentPoolService;
        }

        public async Task<List<TalentPoolResponse>> Handle(TalentPoolGetAllQuery query, CancellationToken cancellationToken)
        {
            return await _talentPoolService.GetAllAsync(query.JobPostingId);
        }
    }
}

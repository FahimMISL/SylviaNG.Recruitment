using MediatR;
using SylviaNG.Recruitment.Application.Features.TalentPools.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.TalentPools.Queries.TalentPoolGetById
{
    public class TalentPoolGetByIdHandler : IRequestHandler<TalentPoolGetByIdQuery, TalentPoolDetailResponse>
    {
        private readonly ITalentPoolService _talentPoolService;

        public TalentPoolGetByIdHandler(ITalentPoolService talentPoolService)
        {
            _talentPoolService = talentPoolService;
        }

        public async Task<TalentPoolDetailResponse> Handle(TalentPoolGetByIdQuery query, CancellationToken cancellationToken)
        {
            return await _talentPoolService.GetByIdAsync(query.TalentPoolId);
        }
    }
}

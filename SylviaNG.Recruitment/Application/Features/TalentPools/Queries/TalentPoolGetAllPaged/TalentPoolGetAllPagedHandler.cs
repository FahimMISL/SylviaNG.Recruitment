using MediatR;
using SylviaNG.Recruitment.Application.Features.TalentPools.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.SharedKernel.Pagination;

namespace SylviaNG.Recruitment.Application.Features.TalentPools.Queries.TalentPoolGetAllPaged
{
    public class TalentPoolGetAllPagedHandler : IRequestHandler<TalentPoolGetAllPagedQuery, PagedResult<TalentPoolResponse>>
    {
        private readonly ITalentPoolService _talentPoolService;

        public TalentPoolGetAllPagedHandler(ITalentPoolService talentPoolService)
        {
            _talentPoolService = talentPoolService;
        }

        public async Task<PagedResult<TalentPoolResponse>> Handle(TalentPoolGetAllPagedQuery query, CancellationToken cancellationToken)
        {
            return await _talentPoolService.GetPaginatedAsync(query.Request);
        }
    }
}

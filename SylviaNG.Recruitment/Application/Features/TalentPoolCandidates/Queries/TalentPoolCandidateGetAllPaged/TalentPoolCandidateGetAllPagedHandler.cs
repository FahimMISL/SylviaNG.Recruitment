using MediatR;
using SylviaNG.Recruitment.Application.Features.TalentPoolCandidates.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.SharedKernel.Pagination;

namespace SylviaNG.Recruitment.Application.Features.TalentPoolCandidates.Queries.TalentPoolCandidateGetAllPaged
{
    public class TalentPoolCandidateGetAllPagedHandler : IRequestHandler<TalentPoolCandidateGetAllPagedQuery, PagedResult<TalentPoolCandidateResponse>>
    {
        private readonly ITalentPoolCandidateService _talentPoolCandidateService;

        public TalentPoolCandidateGetAllPagedHandler(ITalentPoolCandidateService talentPoolCandidateService)
        {
            _talentPoolCandidateService = talentPoolCandidateService;
        }

        public async Task<PagedResult<TalentPoolCandidateResponse>> Handle(TalentPoolCandidateGetAllPagedQuery query, CancellationToken cancellationToken)
        {
            return await _talentPoolCandidateService.GetPaginatedAsync(query.Request);
        }
    }
}

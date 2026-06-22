using MediatR;
using SylviaNG.Recruitment.Application.Features.CandidateSkills.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.SharedKernel.Pagination;

namespace SylviaNG.Recruitment.Application.Features.CandidateSkills.Queries.CandidateSkillGetAllPaged
{
    public class CandidateSkillGetAllPagedHandler : IRequestHandler<CandidateSkillGetAllPagedQuery, PagedResult<CandidateSkillResponse>>
    {
        private readonly ICandidateSkillService _candidateSkillService;

        public CandidateSkillGetAllPagedHandler(ICandidateSkillService candidateSkillService)
        {
            _candidateSkillService = candidateSkillService;
        }

        public async Task<PagedResult<CandidateSkillResponse>> Handle(CandidateSkillGetAllPagedQuery query, CancellationToken cancellationToken)
        {
            return await _candidateSkillService.GetPaginatedAsync(query.Request);
        }
    }
}

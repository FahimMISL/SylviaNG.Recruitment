using MediatR;
using SylviaNG.Recruitment.Application.Features.CandidateExperiences.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.SharedKernel.Pagination;

namespace SylviaNG.Recruitment.Application.Features.CandidateExperiences.Queries.CandidateExperienceGetAllPaged
{
    public class CandidateExperienceGetAllPagedHandler : IRequestHandler<CandidateExperienceGetAllPagedQuery, PagedResult<CandidateExperienceResponse>>
    {
        private readonly ICandidateExperienceService _candidateExperienceService;

        public CandidateExperienceGetAllPagedHandler(ICandidateExperienceService candidateExperienceService)
        {
            _candidateExperienceService = candidateExperienceService;
        }

        public async Task<PagedResult<CandidateExperienceResponse>> Handle(CandidateExperienceGetAllPagedQuery query, CancellationToken cancellationToken)
        {
            return await _candidateExperienceService.GetPaginatedAsync(query.Request);
        }
    }
}

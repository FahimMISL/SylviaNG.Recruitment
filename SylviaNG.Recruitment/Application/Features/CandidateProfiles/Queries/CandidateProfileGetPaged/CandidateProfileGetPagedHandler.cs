using MediatR;
using SylviaNG.Recruitment.Application.Features.CandidateProfiles.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.SharedKernel.Pagination;

namespace SylviaNG.Recruitment.Application.Features.CandidateProfiles.Queries.CandidateProfileGetPaged
{
    public class CandidateProfileGetPagedHandler : IRequestHandler<CandidateProfileGetPagedQuery, PagedResult<CandidateProfileSummaryResponse>>
    {
        private readonly ICandidateProfileService _candidateProfileService;

        public CandidateProfileGetPagedHandler(ICandidateProfileService candidateProfileService)
        {
            _candidateProfileService = candidateProfileService;
        }

        public async Task<PagedResult<CandidateProfileSummaryResponse>> Handle(CandidateProfileGetPagedQuery query, CancellationToken cancellationToken)
        {
            return await _candidateProfileService.GetPagedAsync(query.Request);
        }
    }
}

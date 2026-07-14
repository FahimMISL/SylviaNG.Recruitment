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
            // PagedRequest.SearchTerm only takes effect when SearchProperties is set (see
            // PaginationExtensions.ApplySearch) - the frontend has no way to supply this, so it's
            // set here for the fields HR actually searches candidates by (US-034 AC1 depends on this).
            query.Request.SearchProperties = new[] { "FullName", "Email" };

            return await _candidateProfileService.GetPagedAsync(query.Request);
        }
    }
}

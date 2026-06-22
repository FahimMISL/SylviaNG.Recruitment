using MediatR;
using SylviaNG.Recruitment.Application.Features.Candidates.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.SharedKernel.Pagination;

namespace SylviaNG.Recruitment.Application.Features.Candidates.Queries.CandidateGetAllPaged
{
    public class CandidateGetAllPagedHandler : IRequestHandler<CandidateGetAllPagedQuery, PagedResult<CandidateResponse>>
    {
        private readonly ICandidateService _candidateService;

        public CandidateGetAllPagedHandler(ICandidateService candidateService)
        {
            _candidateService = candidateService;
        }

        public async Task<PagedResult<CandidateResponse>> Handle(CandidateGetAllPagedQuery query, CancellationToken cancellationToken)
        {
            return await _candidateService.GetPaginatedAsync(query.Request);
        }
    }
}

using MediatR;
using SylviaNG.Recruitment.Application.Features.CandidateEducations.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.SharedKernel.Pagination;

namespace SylviaNG.Recruitment.Application.Features.CandidateEducations.Queries.CandidateEducationGetAllPaged
{
    public class CandidateEducationGetAllPagedHandler : IRequestHandler<CandidateEducationGetAllPagedQuery, PagedResult<CandidateEducationResponse>>
    {
        private readonly ICandidateEducationService _candidateEducationService;

        public CandidateEducationGetAllPagedHandler(ICandidateEducationService candidateEducationService)
        {
            _candidateEducationService = candidateEducationService;
        }

        public async Task<PagedResult<CandidateEducationResponse>> Handle(CandidateEducationGetAllPagedQuery query, CancellationToken cancellationToken)
        {
            return await _candidateEducationService.GetPaginatedAsync(query.Request);
        }
    }
}

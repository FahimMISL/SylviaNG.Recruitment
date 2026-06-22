using MediatR;
using SylviaNG.Recruitment.Application.Features.CandidateCertifications.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.SharedKernel.Pagination;

namespace SylviaNG.Recruitment.Application.Features.CandidateCertifications.Queries.CandidateCertificationGetAllPaged
{
    public class CandidateCertificationGetAllPagedHandler : IRequestHandler<CandidateCertificationGetAllPagedQuery, PagedResult<CandidateCertificationResponse>>
    {
        private readonly ICandidateCertificationService _candidateCertificationService;

        public CandidateCertificationGetAllPagedHandler(ICandidateCertificationService candidateCertificationService)
        {
            _candidateCertificationService = candidateCertificationService;
        }

        public async Task<PagedResult<CandidateCertificationResponse>> Handle(CandidateCertificationGetAllPagedQuery query, CancellationToken cancellationToken)
        {
            return await _candidateCertificationService.GetPaginatedAsync(query.Request);
        }
    }
}

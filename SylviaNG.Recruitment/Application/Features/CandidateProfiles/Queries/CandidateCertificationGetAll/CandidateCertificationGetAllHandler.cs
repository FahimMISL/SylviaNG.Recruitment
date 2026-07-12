using MediatR;
using SylviaNG.Recruitment.Application.Features.CandidateProfiles.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.CandidateProfiles.Queries.CandidateCertificationGetAll
{
    public class CandidateCertificationGetAllHandler : IRequestHandler<CandidateCertificationGetAllQuery, List<CandidateCertificationResponse>>
    {
        private readonly ICandidateCertificationService _candidateCertificationService;

        public CandidateCertificationGetAllHandler(ICandidateCertificationService candidateCertificationService)
        {
            _candidateCertificationService = candidateCertificationService;
        }

        public async Task<List<CandidateCertificationResponse>> Handle(CandidateCertificationGetAllQuery query, CancellationToken cancellationToken)
        {
            return await _candidateCertificationService.GetAllForCurrentCandidateAsync();
        }
    }
}

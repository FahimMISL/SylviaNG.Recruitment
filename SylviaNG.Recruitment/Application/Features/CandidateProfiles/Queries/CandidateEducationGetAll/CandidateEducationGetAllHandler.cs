using MediatR;
using SylviaNG.Recruitment.Application.Features.CandidateProfiles.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.CandidateProfiles.Queries.CandidateEducationGetAll
{
    public class CandidateEducationGetAllHandler : IRequestHandler<CandidateEducationGetAllQuery, List<CandidateEducationResponse>>
    {
        private readonly ICandidateEducationService _candidateEducationService;

        public CandidateEducationGetAllHandler(ICandidateEducationService candidateEducationService)
        {
            _candidateEducationService = candidateEducationService;
        }

        public async Task<List<CandidateEducationResponse>> Handle(CandidateEducationGetAllQuery query, CancellationToken cancellationToken)
        {
            return await _candidateEducationService.GetAllForCurrentCandidateAsync();
        }
    }
}

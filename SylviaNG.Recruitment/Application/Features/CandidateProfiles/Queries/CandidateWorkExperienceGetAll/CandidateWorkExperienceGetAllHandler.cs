using MediatR;
using SylviaNG.Recruitment.Application.Features.CandidateProfiles.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.CandidateProfiles.Queries.CandidateWorkExperienceGetAll
{
    public class CandidateWorkExperienceGetAllHandler : IRequestHandler<CandidateWorkExperienceGetAllQuery, List<CandidateWorkExperienceResponse>>
    {
        private readonly ICandidateWorkExperienceService _candidateWorkExperienceService;

        public CandidateWorkExperienceGetAllHandler(ICandidateWorkExperienceService candidateWorkExperienceService)
        {
            _candidateWorkExperienceService = candidateWorkExperienceService;
        }

        public async Task<List<CandidateWorkExperienceResponse>> Handle(CandidateWorkExperienceGetAllQuery query, CancellationToken cancellationToken)
        {
            return await _candidateWorkExperienceService.GetAllForCurrentCandidateAsync();
        }
    }
}

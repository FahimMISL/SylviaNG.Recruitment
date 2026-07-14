using MediatR;
using SylviaNG.Recruitment.Application.Features.CandidateProfiles.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.CandidateProfiles.Queries.CandidateSkillGetAll
{
    public class CandidateSkillGetAllHandler : IRequestHandler<CandidateSkillGetAllQuery, List<CandidateSkillResponse>>
    {
        private readonly ICandidateSkillService _candidateSkillService;

        public CandidateSkillGetAllHandler(ICandidateSkillService candidateSkillService)
        {
            _candidateSkillService = candidateSkillService;
        }

        public async Task<List<CandidateSkillResponse>> Handle(CandidateSkillGetAllQuery query, CancellationToken cancellationToken)
        {
            return await _candidateSkillService.GetAllForCurrentCandidateAsync();
        }
    }
}

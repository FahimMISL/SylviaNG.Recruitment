using MediatR;
using SylviaNG.Recruitment.Application.Features.CandidateSkills.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.CandidateSkills.Queries.CandidateSkillGetAll
{
    public class CandidateSkillGetAllHandler : IRequestHandler<CandidateSkillGetAllQuery, List<CandidateSkillResponse>>
    {
        private readonly ICandidateSkillService _service;

        public CandidateSkillGetAllHandler(ICandidateSkillService service)
        {
            _service = service;
        }

        public async Task<List<CandidateSkillResponse>> Handle(CandidateSkillGetAllQuery query, CancellationToken cancellationToken)
        {
            return await _service.GetAllAsync();
        }
    }
}

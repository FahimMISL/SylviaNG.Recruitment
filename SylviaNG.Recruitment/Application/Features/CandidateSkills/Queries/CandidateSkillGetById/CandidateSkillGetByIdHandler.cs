using MediatR;
using SylviaNG.Recruitment.Application.Features.CandidateSkills.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.CandidateSkills.Queries.CandidateSkillGetById
{
    public class CandidateSkillGetByIdHandler : IRequestHandler<CandidateSkillGetByIdQuery, CandidateSkillResponse>
    {
        private readonly ICandidateSkillService _service;

        public CandidateSkillGetByIdHandler(ICandidateSkillService service)
        {
            _service = service;
        }

        public async Task<CandidateSkillResponse> Handle(CandidateSkillGetByIdQuery query, CancellationToken cancellationToken)
        {
            return await _service.GetByIdAsync(query.CandidateSkillId);
        }
    }
}

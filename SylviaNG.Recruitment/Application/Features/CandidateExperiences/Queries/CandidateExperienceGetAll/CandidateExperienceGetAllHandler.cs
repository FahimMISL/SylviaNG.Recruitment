using MediatR;
using SylviaNG.Recruitment.Application.Features.CandidateExperiences.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.CandidateExperiences.Queries.CandidateExperienceGetAll
{
    public class CandidateExperienceGetAllHandler : IRequestHandler<CandidateExperienceGetAllQuery, List<CandidateExperienceResponse>>
    {
        private readonly ICandidateExperienceService _service;

        public CandidateExperienceGetAllHandler(ICandidateExperienceService service)
        {
            _service = service;
        }

        public async Task<List<CandidateExperienceResponse>> Handle(CandidateExperienceGetAllQuery query, CancellationToken cancellationToken)
        {
            return await _service.GetAllAsync();
        }
    }
}

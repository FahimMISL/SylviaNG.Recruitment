using MediatR;
using SylviaNG.Recruitment.Application.Features.CandidateExperiences.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.CandidateExperiences.Queries.CandidateExperienceGetById
{
    public class CandidateExperienceGetByIdHandler : IRequestHandler<CandidateExperienceGetByIdQuery, CandidateExperienceResponse>
    {
        private readonly ICandidateExperienceService _service;

        public CandidateExperienceGetByIdHandler(ICandidateExperienceService service)
        {
            _service = service;
        }

        public async Task<CandidateExperienceResponse> Handle(CandidateExperienceGetByIdQuery query, CancellationToken cancellationToken)
        {
            return await _service.GetByIdAsync(query.CandidateExperienceId);
        }
    }
}

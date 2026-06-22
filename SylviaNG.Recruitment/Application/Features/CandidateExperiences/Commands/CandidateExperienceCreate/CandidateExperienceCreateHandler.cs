using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.CandidateExperiences.Commands.CandidateExperienceCreate
{
    public class CandidateExperienceCreateHandler : IRequestHandler<CandidateExperienceCreateCommand, long>
    {
        private readonly ICandidateExperienceService _service;

        public CandidateExperienceCreateHandler(ICandidateExperienceService service)
        {
            _service = service;
        }

        public async Task<long> Handle(CandidateExperienceCreateCommand command, CancellationToken cancellationToken)
        {
            return await _service.CreateAsync(command.Request);
        }
    }
}

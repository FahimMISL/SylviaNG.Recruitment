using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.CandidateProfiles.Commands.CandidateWorkExperienceCreate
{
    public class CandidateWorkExperienceCreateHandler : IRequestHandler<CandidateWorkExperienceCreateCommand, long>
    {
        private readonly ICandidateWorkExperienceService _candidateWorkExperienceService;

        public CandidateWorkExperienceCreateHandler(ICandidateWorkExperienceService candidateWorkExperienceService)
        {
            _candidateWorkExperienceService = candidateWorkExperienceService;
        }

        public async Task<long> Handle(CandidateWorkExperienceCreateCommand command, CancellationToken cancellationToken)
        {
            return await _candidateWorkExperienceService.CreateAsync(command.Request);
        }
    }
}

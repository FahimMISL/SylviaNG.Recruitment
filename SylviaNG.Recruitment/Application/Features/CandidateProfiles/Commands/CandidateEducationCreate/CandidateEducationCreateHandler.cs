using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.CandidateProfiles.Commands.CandidateEducationCreate
{
    public class CandidateEducationCreateHandler : IRequestHandler<CandidateEducationCreateCommand, long>
    {
        private readonly ICandidateEducationService _candidateEducationService;

        public CandidateEducationCreateHandler(ICandidateEducationService candidateEducationService)
        {
            _candidateEducationService = candidateEducationService;
        }

        public async Task<long> Handle(CandidateEducationCreateCommand command, CancellationToken cancellationToken)
        {
            return await _candidateEducationService.CreateAsync(command.Request);
        }
    }
}

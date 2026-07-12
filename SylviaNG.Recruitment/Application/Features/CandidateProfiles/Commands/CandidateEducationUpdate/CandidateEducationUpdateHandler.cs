using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.CandidateProfiles.Commands.CandidateEducationUpdate
{
    public class CandidateEducationUpdateHandler : IRequestHandler<CandidateEducationUpdateCommand, Unit>
    {
        private readonly ICandidateEducationService _candidateEducationService;

        public CandidateEducationUpdateHandler(ICandidateEducationService candidateEducationService)
        {
            _candidateEducationService = candidateEducationService;
        }

        public async Task<Unit> Handle(CandidateEducationUpdateCommand command, CancellationToken cancellationToken)
        {
            await _candidateEducationService.UpdateAsync(command.CandidateEducationId, command.Request);
            return Unit.Value;
        }
    }
}

using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.CandidateProfiles.Commands.CandidateEducationDelete
{
    public class CandidateEducationDeleteHandler : IRequestHandler<CandidateEducationDeleteCommand, Unit>
    {
        private readonly ICandidateEducationService _candidateEducationService;

        public CandidateEducationDeleteHandler(ICandidateEducationService candidateEducationService)
        {
            _candidateEducationService = candidateEducationService;
        }

        public async Task<Unit> Handle(CandidateEducationDeleteCommand command, CancellationToken cancellationToken)
        {
            await _candidateEducationService.DeleteAsync(command.CandidateEducationId);
            return Unit.Value;
        }
    }
}

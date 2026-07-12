using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.CandidateProfiles.Commands.CandidateCertificationUpdate
{
    public class CandidateCertificationUpdateHandler : IRequestHandler<CandidateCertificationUpdateCommand, Unit>
    {
        private readonly ICandidateCertificationService _candidateCertificationService;

        public CandidateCertificationUpdateHandler(ICandidateCertificationService candidateCertificationService)
        {
            _candidateCertificationService = candidateCertificationService;
        }

        public async Task<Unit> Handle(CandidateCertificationUpdateCommand command, CancellationToken cancellationToken)
        {
            await _candidateCertificationService.UpdateAsync(command.CandidateCertificationId, command.Request);
            return Unit.Value;
        }
    }
}

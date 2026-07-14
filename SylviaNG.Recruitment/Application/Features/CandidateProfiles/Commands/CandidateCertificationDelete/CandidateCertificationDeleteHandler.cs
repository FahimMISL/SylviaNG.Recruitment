using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.CandidateProfiles.Commands.CandidateCertificationDelete
{
    public class CandidateCertificationDeleteHandler : IRequestHandler<CandidateCertificationDeleteCommand, Unit>
    {
        private readonly ICandidateCertificationService _candidateCertificationService;

        public CandidateCertificationDeleteHandler(ICandidateCertificationService candidateCertificationService)
        {
            _candidateCertificationService = candidateCertificationService;
        }

        public async Task<Unit> Handle(CandidateCertificationDeleteCommand command, CancellationToken cancellationToken)
        {
            await _candidateCertificationService.DeleteAsync(command.CandidateCertificationId);
            return Unit.Value;
        }
    }
}

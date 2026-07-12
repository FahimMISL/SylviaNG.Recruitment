using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.CandidateProfiles.Commands.CandidateCertificationCreate
{
    public class CandidateCertificationCreateHandler : IRequestHandler<CandidateCertificationCreateCommand, long>
    {
        private readonly ICandidateCertificationService _candidateCertificationService;

        public CandidateCertificationCreateHandler(ICandidateCertificationService candidateCertificationService)
        {
            _candidateCertificationService = candidateCertificationService;
        }

        public async Task<long> Handle(CandidateCertificationCreateCommand command, CancellationToken cancellationToken)
        {
            return await _candidateCertificationService.CreateAsync(command.Request);
        }
    }
}

using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.CandidateCertifications.Commands.CandidateCertificationUpdate
{
    public class CandidateCertificationUpdateHandler : IRequestHandler<CandidateCertificationUpdateCommand, Unit>
    {
        private readonly ICandidateCertificationService _service;

        public CandidateCertificationUpdateHandler(ICandidateCertificationService service)
        {
            _service = service;
        }

        public async Task<Unit> Handle(CandidateCertificationUpdateCommand command, CancellationToken cancellationToken)
        {
            await _service.UpdateAsync(command.CandidateCertificationId, command.Request);
            return Unit.Value;
        }
    }
}

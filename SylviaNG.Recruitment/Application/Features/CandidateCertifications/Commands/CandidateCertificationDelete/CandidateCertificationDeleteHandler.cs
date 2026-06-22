using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.CandidateCertifications.Commands.CandidateCertificationDelete
{
    public class CandidateCertificationDeleteHandler : IRequestHandler<CandidateCertificationDeleteCommand, Unit>
    {
        private readonly ICandidateCertificationService _service;

        public CandidateCertificationDeleteHandler(ICandidateCertificationService service)
        {
            _service = service;
        }

        public async Task<Unit> Handle(CandidateCertificationDeleteCommand command, CancellationToken cancellationToken)
        {
            await _service.DeleteAsync(command.CandidateCertificationId);
            return Unit.Value;
        }
    }
}

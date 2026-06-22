using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.CandidateCertifications.Commands.CandidateCertificationCreate
{
    public class CandidateCertificationCreateHandler : IRequestHandler<CandidateCertificationCreateCommand, long>
    {
        private readonly ICandidateCertificationService _service;

        public CandidateCertificationCreateHandler(ICandidateCertificationService service)
        {
            _service = service;
        }

        public async Task<long> Handle(CandidateCertificationCreateCommand command, CancellationToken cancellationToken)
        {
            return await _service.CreateAsync(command.Request);
        }
    }
}

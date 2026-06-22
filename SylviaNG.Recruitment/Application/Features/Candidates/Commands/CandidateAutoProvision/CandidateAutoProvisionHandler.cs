using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.Candidates.Commands.CandidateAutoProvision
{
    public class CandidateAutoProvisionHandler : IRequestHandler<CandidateAutoProvisionCommand, long>
    {
        private readonly ICandidateService _service;

        public CandidateAutoProvisionHandler(ICandidateService service)
        {
            _service = service;
        }

        public async Task<long> Handle(CandidateAutoProvisionCommand command, CancellationToken cancellationToken)
        {
            return await _service.AutoProvisionAsync(command.Request);
        }
    }
}

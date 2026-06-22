using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.Candidates.Commands.CandidateUpdate
{
    public class CandidateUpdateHandler : IRequestHandler<CandidateUpdateCommand, Unit>
    {
        private readonly ICandidateService _service;

        public CandidateUpdateHandler(ICandidateService service)
        {
            _service = service;
        }

        public async Task<Unit> Handle(CandidateUpdateCommand command, CancellationToken cancellationToken)
        {
            await _service.UpdateAsync(command.CandidateId, command.Request);
            return Unit.Value;
        }
    }
}

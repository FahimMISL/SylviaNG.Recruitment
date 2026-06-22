using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.TalentPoolCandidates.Commands.TalentPoolCandidateUpdate
{
    public class TalentPoolCandidateUpdateHandler : IRequestHandler<TalentPoolCandidateUpdateCommand, Unit>
    {
        private readonly ITalentPoolCandidateService _service;

        public TalentPoolCandidateUpdateHandler(ITalentPoolCandidateService service)
        {
            _service = service;
        }

        public async Task<Unit> Handle(TalentPoolCandidateUpdateCommand command, CancellationToken cancellationToken)
        {
            await _service.UpdateAsync(command.TalentPoolCandidateId, command.Request);
            return Unit.Value;
        }
    }
}

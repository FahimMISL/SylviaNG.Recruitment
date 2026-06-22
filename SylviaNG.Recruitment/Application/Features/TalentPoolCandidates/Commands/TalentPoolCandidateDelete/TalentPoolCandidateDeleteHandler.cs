using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.TalentPoolCandidates.Commands.TalentPoolCandidateDelete
{
    public class TalentPoolCandidateDeleteHandler : IRequestHandler<TalentPoolCandidateDeleteCommand, Unit>
    {
        private readonly ITalentPoolCandidateService _service;

        public TalentPoolCandidateDeleteHandler(ITalentPoolCandidateService service)
        {
            _service = service;
        }

        public async Task<Unit> Handle(TalentPoolCandidateDeleteCommand command, CancellationToken cancellationToken)
        {
            await _service.DeleteAsync(command.TalentPoolCandidateId);
            return Unit.Value;
        }
    }
}

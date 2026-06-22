using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.Candidates.Commands.CandidateDelete
{
    public class CandidateDeleteHandler : IRequestHandler<CandidateDeleteCommand, Unit>
    {
        private readonly ICandidateService _service;

        public CandidateDeleteHandler(ICandidateService service)
        {
            _service = service;
        }

        public async Task<Unit> Handle(CandidateDeleteCommand command, CancellationToken cancellationToken)
        {
            await _service.DeleteAsync(command.CandidateId);
            return Unit.Value;
        }
    }
}

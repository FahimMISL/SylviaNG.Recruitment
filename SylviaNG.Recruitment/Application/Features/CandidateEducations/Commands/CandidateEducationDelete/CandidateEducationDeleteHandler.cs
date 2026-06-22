using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.CandidateEducations.Commands.CandidateEducationDelete
{
    public class CandidateEducationDeleteHandler : IRequestHandler<CandidateEducationDeleteCommand, Unit>
    {
        private readonly ICandidateEducationService _service;

        public CandidateEducationDeleteHandler(ICandidateEducationService service)
        {
            _service = service;
        }

        public async Task<Unit> Handle(CandidateEducationDeleteCommand command, CancellationToken cancellationToken)
        {
            await _service.DeleteAsync(command.CandidateEducationId);
            return Unit.Value;
        }
    }
}

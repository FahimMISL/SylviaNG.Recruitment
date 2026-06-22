using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.CandidateEducations.Commands.CandidateEducationUpdate
{
    public class CandidateEducationUpdateHandler : IRequestHandler<CandidateEducationUpdateCommand, Unit>
    {
        private readonly ICandidateEducationService _service;

        public CandidateEducationUpdateHandler(ICandidateEducationService service)
        {
            _service = service;
        }

        public async Task<Unit> Handle(CandidateEducationUpdateCommand command, CancellationToken cancellationToken)
        {
            await _service.UpdateAsync(command.CandidateEducationId, command.Request);
            return Unit.Value;
        }
    }
}

using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.Nominees.Commands.NomineeUpdate
{
    public class NomineeUpdateHandler : IRequestHandler<NomineeUpdateCommand, Unit>
    {
        private readonly INomineeService _service;

        public NomineeUpdateHandler(INomineeService service)
        {
            _service = service;
        }

        public async Task<Unit> Handle(NomineeUpdateCommand command, CancellationToken cancellationToken)
        {
            await _service.UpdateAsync(command.NomineeId, command.Request);
            return Unit.Value;
        }
    }
}

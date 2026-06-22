using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.Nominees.Commands.NomineeDelete
{
    public class NomineeDeleteHandler : IRequestHandler<NomineeDeleteCommand, Unit>
    {
        private readonly INomineeService _service;

        public NomineeDeleteHandler(INomineeService service)
        {
            _service = service;
        }

        public async Task<Unit> Handle(NomineeDeleteCommand command, CancellationToken cancellationToken)
        {
            await _service.DeleteAsync(command.NomineeId);
            return Unit.Value;
        }
    }
}

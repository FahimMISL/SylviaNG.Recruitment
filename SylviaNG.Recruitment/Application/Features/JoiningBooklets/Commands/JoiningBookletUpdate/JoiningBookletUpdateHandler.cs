using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.JoiningBooklets.Commands.JoiningBookletUpdate
{
    public class JoiningBookletUpdateHandler : IRequestHandler<JoiningBookletUpdateCommand, Unit>
    {
        private readonly IJoiningBookletService _service;

        public JoiningBookletUpdateHandler(IJoiningBookletService service)
        {
            _service = service;
        }

        public async Task<Unit> Handle(JoiningBookletUpdateCommand command, CancellationToken cancellationToken)
        {
            await _service.UpdateAsync(command.JoiningBookletId, command.Request);
            return Unit.Value;
        }
    }
}

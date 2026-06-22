using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.JoiningBooklets.Commands.JoiningBookletDelete
{
    public class JoiningBookletDeleteHandler : IRequestHandler<JoiningBookletDeleteCommand, Unit>
    {
        private readonly IJoiningBookletService _service;

        public JoiningBookletDeleteHandler(IJoiningBookletService service)
        {
            _service = service;
        }

        public async Task<Unit> Handle(JoiningBookletDeleteCommand command, CancellationToken cancellationToken)
        {
            await _service.DeleteAsync(command.JoiningBookletId);
            return Unit.Value;
        }
    }
}

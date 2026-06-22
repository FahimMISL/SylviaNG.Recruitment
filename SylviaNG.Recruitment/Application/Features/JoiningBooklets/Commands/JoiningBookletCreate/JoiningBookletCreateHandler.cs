using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.JoiningBooklets.Commands.JoiningBookletCreate
{
    public class JoiningBookletCreateHandler : IRequestHandler<JoiningBookletCreateCommand, long>
    {
        private readonly IJoiningBookletService _service;

        public JoiningBookletCreateHandler(IJoiningBookletService service)
        {
            _service = service;
        }

        public async Task<long> Handle(JoiningBookletCreateCommand command, CancellationToken cancellationToken)
        {
            return await _service.CreateAsync(command.Request);
        }
    }
}

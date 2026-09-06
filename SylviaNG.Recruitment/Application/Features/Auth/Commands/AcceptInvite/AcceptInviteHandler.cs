using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.Auth.Commands.AcceptInvite
{
    public class AcceptInviteHandler : IRequestHandler<AcceptInviteCommand>
    {
        private readonly IAuthService _authService;

        public AcceptInviteHandler(IAuthService authService)
        {
            _authService = authService;
        }

        public async Task Handle(AcceptInviteCommand command, CancellationToken cancellationToken)
        {
            await _authService.AcceptInviteAsync(command.Request);
        }
    }
}

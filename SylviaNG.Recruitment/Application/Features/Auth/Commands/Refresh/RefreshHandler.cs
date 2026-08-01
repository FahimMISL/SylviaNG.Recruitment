using MediatR;
using SylviaNG.Recruitment.Application.Features.Auth.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.Auth.Commands.Refresh
{
    public class RefreshHandler : IRequestHandler<RefreshCommand, LoginResponse>
    {
        private readonly IAuthService _authService;

        public RefreshHandler(IAuthService authService)
        {
            _authService = authService;
        }

        public async Task<LoginResponse> Handle(RefreshCommand command, CancellationToken cancellationToken)
        {
            return await _authService.RefreshAsync(command.Request.RefreshToken);
        }
    }
}

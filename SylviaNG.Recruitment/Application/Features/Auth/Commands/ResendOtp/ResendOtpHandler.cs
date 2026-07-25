using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.Auth.Commands.ResendOtp
{
    public class ResendOtpHandler : IRequestHandler<ResendOtpCommand>
    {
        private readonly IAuthService _authService;

        public ResendOtpHandler(IAuthService authService)
        {
            _authService = authService;
        }

        public async Task Handle(ResendOtpCommand command, CancellationToken cancellationToken)
        {
            await _authService.ResendOtpAsync(command.Request);
        }
    }
}

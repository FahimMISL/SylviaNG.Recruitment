using MediatR;
using SylviaNG.Recruitment.Application.Features.Auth.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.Auth.Commands.VerifyOtp
{
    public class VerifyOtpHandler : IRequestHandler<VerifyOtpCommand, LoginResponse>
    {
        private readonly IAuthService _authService;

        public VerifyOtpHandler(IAuthService authService)
        {
            _authService = authService;
        }

        public async Task<LoginResponse> Handle(VerifyOtpCommand command, CancellationToken cancellationToken)
        {
            return await _authService.VerifyOtpAsync(command.Request);
        }
    }
}

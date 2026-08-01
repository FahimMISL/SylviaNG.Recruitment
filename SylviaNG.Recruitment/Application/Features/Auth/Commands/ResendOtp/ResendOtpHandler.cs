using MediatR;
using SylviaNG.Recruitment.Application.Features.Auth.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.Auth.Commands.ResendOtp
{
    public class ResendOtpHandler : IRequestHandler<ResendOtpCommand, ResendOtpResponse>
    {
        private readonly IAuthService _authService;

        public ResendOtpHandler(IAuthService authService)
        {
            _authService = authService;
        }

        public async Task<ResendOtpResponse> Handle(ResendOtpCommand command, CancellationToken cancellationToken)
        {
            return await _authService.ResendOtpAsync(command.Request);
        }
    }
}

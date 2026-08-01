using MediatR;
using SylviaNG.Recruitment.Application.Features.Auth.Models;

namespace SylviaNG.Recruitment.Application.Features.Auth.Commands.VerifyOtp
{
    public class VerifyOtpCommand : IRequest<LoginResponse>
    {
        public VerifyOtpRequest Request { get; set; }

        public VerifyOtpCommand(VerifyOtpRequest request)
        {
            Request = request;
        }
    }
}

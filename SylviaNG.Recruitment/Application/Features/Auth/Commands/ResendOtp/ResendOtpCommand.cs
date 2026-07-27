using MediatR;
using SylviaNG.Recruitment.Application.Features.Auth.Models;

namespace SylviaNG.Recruitment.Application.Features.Auth.Commands.ResendOtp
{
    public class ResendOtpCommand : IRequest
    {
        public ResendOtpRequest Request { get; set; }

        public ResendOtpCommand(ResendOtpRequest request)
        {
            Request = request;
        }
    }
}

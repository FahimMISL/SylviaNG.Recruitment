using MediatR;
using SylviaNG.Recruitment.Application.Features.Auth.Models;

namespace SylviaNG.Recruitment.Application.Features.Auth.Commands.ForgotPassword
{
    public class ForgotPasswordCommand : IRequest<ForgotPasswordResponse>
    {
        public ForgotPasswordRequest Request { get; set; }

        public ForgotPasswordCommand(ForgotPasswordRequest request)
        {
            Request = request;
        }
    }
}

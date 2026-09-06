using MediatR;
using SylviaNG.Recruitment.Application.Features.Auth.Models;

namespace SylviaNG.Recruitment.Application.Features.Auth.Commands.ResetPassword
{
    public class ResetPasswordCommand : IRequest
    {
        public ResetPasswordRequest Request { get; set; }

        public ResetPasswordCommand(ResetPasswordRequest request)
        {
            Request = request;
        }
    }
}

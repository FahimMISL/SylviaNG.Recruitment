using MediatR;
using SylviaNG.Recruitment.Application.Features.Auth.Models;

namespace SylviaNG.Recruitment.Application.Features.Auth.Commands.Register
{
    public class RegisterCommand : IRequest<RegisterResponse>
    {
        public RegisterRequest Request { get; set; }

        public RegisterCommand(RegisterRequest request)
        {
            Request = request;
        }
    }
}

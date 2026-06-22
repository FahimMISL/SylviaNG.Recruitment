using MediatR;
using SylviaNG.Recruitment.Application.Features.Users.Models;

namespace SylviaNG.Recruitment.Application.Features.Users.Commands.UserCreate
{
    public class UserCreateCommand : IRequest<long>
    {
        public UserCreateRequest Request { get; set; }

        public UserCreateCommand(UserCreateRequest request)
        {
            Request = request;
        }
    }
}

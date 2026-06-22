using MediatR;
using SylviaNG.Recruitment.Application.Features.Users.Models;

namespace SylviaNG.Recruitment.Application.Features.Users.Commands.UserUpdate
{
    public class UserUpdateCommand : IRequest<Unit>
    {
        public long UserId { get; set; }
        public UserUpdateRequest Request { get; set; }

        public UserUpdateCommand(long userId, UserUpdateRequest request)
        {
            UserId = userId;
            Request = request;
        }
    }
}

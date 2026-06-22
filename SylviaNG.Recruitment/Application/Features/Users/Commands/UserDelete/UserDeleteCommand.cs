using MediatR;

namespace SylviaNG.Recruitment.Application.Features.Users.Commands.UserDelete
{
    public class UserDeleteCommand : IRequest<Unit>
    {
        public long UserId { get; set; }

        public UserDeleteCommand(long userId)
        {
            UserId = userId;
        }
    }
}

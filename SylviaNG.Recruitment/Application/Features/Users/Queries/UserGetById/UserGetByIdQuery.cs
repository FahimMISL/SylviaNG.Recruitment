using MediatR;
using SylviaNG.Recruitment.Application.Features.Users.Models;

namespace SylviaNG.Recruitment.Application.Features.Users.Queries.UserGetById
{
    public class UserGetByIdQuery : IRequest<UserResponse>
    {
        public long UserId { get; set; }

        public UserGetByIdQuery(long userId)
        {
            UserId = userId;
        }
    }
}

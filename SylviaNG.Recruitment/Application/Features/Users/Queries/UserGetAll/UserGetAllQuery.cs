using MediatR;
using SylviaNG.Recruitment.Application.Features.Users.Models;

namespace SylviaNG.Recruitment.Application.Features.Users.Queries.UserGetAll
{
    public class UserGetAllQuery : IRequest<List<UserResponse>>
    {
    }
}

using MediatR;
using SylviaNG.Recruitment.Application.Features.Users.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.Users.Queries.UserGetById
{
    public class UserGetByIdHandler : IRequestHandler<UserGetByIdQuery, UserResponse>
    {
        private readonly IUserService _userService;

        public UserGetByIdHandler(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<UserResponse> Handle(UserGetByIdQuery query, CancellationToken cancellationToken)
        {
            return await _userService.GetByIdAsync(query.UserId);
        }
    }
}

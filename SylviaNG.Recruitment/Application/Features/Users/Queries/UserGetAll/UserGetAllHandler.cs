using MediatR;
using SylviaNG.Recruitment.Application.Features.Users.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.Users.Queries.UserGetAll
{
    public class UserGetAllHandler : IRequestHandler<UserGetAllQuery, List<UserResponse>>
    {
        private readonly IUserService _userService;

        public UserGetAllHandler(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<List<UserResponse>> Handle(UserGetAllQuery query, CancellationToken cancellationToken)
        {
            return await _userService.GetAllAsync();
        }
    }
}

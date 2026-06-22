using MediatR;
using SylviaNG.Recruitment.Application.Features.Users.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.SharedKernel.Pagination;

namespace SylviaNG.Recruitment.Application.Features.Users.Queries.UserGetAllPaged
{
    public class UserGetAllPagedHandler : IRequestHandler<UserGetAllPagedQuery, PagedResult<UserResponse>>
    {
        private readonly IUserService _userService;

        public UserGetAllPagedHandler(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<PagedResult<UserResponse>> Handle(UserGetAllPagedQuery query, CancellationToken cancellationToken)
        {
            return await _userService.GetPaginatedAsync(query.Request);
        }
    }
}

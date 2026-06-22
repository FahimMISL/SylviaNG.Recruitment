using MediatR;
using SylviaNG.Recruitment.Application.Features.Users.Models;
using SylviaNG.Recruitment.SharedKernel.Pagination;

namespace SylviaNG.Recruitment.Application.Features.Users.Queries.UserGetAllPaged
{
    public class UserGetAllPagedQuery : IRequest<PagedResult<UserResponse>>
    {
        public PagedRequest Request { get; set; }

        public UserGetAllPagedQuery(PagedRequest request)
        {
            Request = request;
        }
    }
}

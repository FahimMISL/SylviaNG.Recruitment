using MediatR;
using SylviaNG.Recruitment.Application.Features.Roles.Models;

namespace SylviaNG.Recruitment.Application.Features.Roles.Queries.RoleGetById
{
    public class RoleGetByIdQuery : IRequest<RoleResponse>
    {
        public long RoleId { get; set; }

        public RoleGetByIdQuery(long roleId)
        {
            RoleId = roleId;
        }
    }
}

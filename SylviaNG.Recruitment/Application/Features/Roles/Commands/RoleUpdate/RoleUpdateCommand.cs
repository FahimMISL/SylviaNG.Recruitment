using MediatR;
using SylviaNG.Recruitment.Application.Features.Roles.Models;

namespace SylviaNG.Recruitment.Application.Features.Roles.Commands.RoleUpdate
{
    public class RoleUpdateCommand : IRequest<Unit>
    {
        public long RoleId { get; set; }
        public RoleUpdateRequest Request { get; set; }

        public RoleUpdateCommand(long roleId, RoleUpdateRequest request)
        {
            RoleId = roleId;
            Request = request;
        }
    }
}

using MediatR;

namespace SylviaNG.Recruitment.Application.Features.Roles.Commands.RoleDelete
{
    public class RoleDeleteCommand : IRequest<Unit>
    {
        public long RoleId { get; set; }

        public RoleDeleteCommand(long roleId)
        {
            RoleId = roleId;
        }
    }
}

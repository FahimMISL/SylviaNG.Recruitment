using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.Roles.Commands.RoleDelete
{
    public class RoleDeleteHandler : IRequestHandler<RoleDeleteCommand, Unit>
    {
        private readonly IRoleService _roleService;

        public RoleDeleteHandler(IRoleService roleService)
        {
            _roleService = roleService;
        }

        public async Task<Unit> Handle(RoleDeleteCommand command, CancellationToken cancellationToken)
        {
            await _roleService.DeleteAsync(command.RoleId);
            return Unit.Value;
        }
    }
}

using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.Roles.Commands.RoleUpdate
{
    public class RoleUpdateHandler : IRequestHandler<RoleUpdateCommand, Unit>
    {
        private readonly IRoleService _roleService;

        public RoleUpdateHandler(IRoleService roleService)
        {
            _roleService = roleService;
        }

        public async Task<Unit> Handle(RoleUpdateCommand command, CancellationToken cancellationToken)
        {
            await _roleService.UpdateAsync(command.RoleId, command.Request);
            return Unit.Value;
        }
    }
}
